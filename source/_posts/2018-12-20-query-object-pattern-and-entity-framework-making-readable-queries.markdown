---
layout: post
title: "Query Object Pattern and Entity Framework - Making Readable Queries"
comments: true
categories: 
- Entity Framework
- Programming
tags: 
date: 2018-12-20
completedDate: 2018-12-17 08:40:44 +1000
keywords: 
description: Using a Query Object to contain large query criteria and iterating over the query to make it more readable.
primaryImage: refactoring.jpg
---

Search is a common requirement for most of the applications that we build today. Searching for data often includes multiple fields, data types, and data from multiple tables (especially when using a relational database). I was recently building a Search page which involved searching for Orders - users needed the ability to search by different criteria such as the employee who created the order, orders for a customer, orders between particular dates, order status, an address of delivery. Order criteria are optional, and they allow to narrow down on your search with additional parameters. We were building an API endpoint to query this data based on the parameters using EF Core backed by Azure SQL.

In this post, we go through the code iterations that I made to improve on the readability of the query and keep it contained in a single place. The intention is to create a Query Object like structure that contains all query logic and keep it centralized and readable. 


> *A [Query Object](https://martinfowler.com/eaaCatalog/queryObject.html) is an interpreter [Gang of Four], that is, a structure of objects that can form itself into a SQL query. You can create this query by referring to classes and fields rather than tables and columns. In this way, those who write the queries can do so independently of the database schema, and changes to the schema can be localized in a single place.*

``` csharp Query Object capturing the Search Criteria
public class OrderSummaryQuery
{
    public int? CustomerId { get; set; }
    public DateRange DateRange { get; set; }
    public string Employee { get; set; }
    public string Address { get; set;}
    public OrderStatus OrderStatus { get; set; }
}
```
I have removed the final projection in all the queries below to keep the code to a minimum. We will go through all the iterations to make the code more readable, keeping the generated SQL query efficient as possible.

### Iteration 1 - Crude Form

Let's start with the crudest form of the query stating all possible combinations of the query. Since all properties are nullable, check if a value exists before using it in the query. 

``` csharp
(from order in _context.Order
join od in _context.OrderDelivery on order.Id equals od.OrderId
join customer in _context.Customer on order.CustomerId equals customer.Id
where order.Status == OrderStatus.Quote &&
      order.Active == true &&
      (query.Employee == null || 
      (order.CreatedBy == query.Employee || customer.Employee == query.Employee)) &&
      (!query.CustomerId.HasValue ||
      customer.Id == query.CustomerId.Value) &&
      (query.DateRange == null || 
      order.Created >= query.DateRange.StartDate && order.Created <= query.DateRange.EndDate))
```

### Iteration 2 - Separating into Multiple Lines

With all those explicit AND (&&) clauses the query is hard to understand and keep up. Splitting them into multiple where clauses make it more cleaner and keep each search criteria independent. The end SQL query that gets generated remains the same in this case.

> [Aesthetics of code](https://rahulpnath.com/blog/left-align-your-code-for-better-readability/) is as important as the code you write. Aligning is an important part that contributes to the overall aesthetics of code.

``` csharp
from order in _context.Order
join od in _context.OrderDelivery on order.Id equals od.OrderId
join customer in _context.Customer on order.CustomerId equals customer.Id
where order.Status == orderStatus && order.Active == true
where query.Employee == null ||
      order.CreatedBy == query.Employee || customer.Employee == query.Employee
where !query.CustomerId.HasValue || customer.Id == query.CustomerId.Value
where query.DateRange == null ||
      (order.Created >= query.DateRange.StartDate && order.Created <= query.DateRange.EndDate)
```

### Iteration 3 - Refactor to Expressions

Now that each criterion is independently visible let's make each of the *where* clause more readable. Refactoring them into C# class functions makes the generated SQL inefficient, as EF cannot transform C# functions into SQL.  Such conditions in a standard C# function gets evaluated on the client site, after retrieving all data from the server. Depending on the size of your data, this is something you need [to be aware of](https://docs.microsoft.com/en-us/ef/core/querying/client-eval#client-evaluation-performance-issues). 

However, if you use [Expressions](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/expressions-in-linq-to-entities-queries) those get transformed to evaluate on the server. Since all of the conditions on our where clauses can be represented as an Expression, let's move those to the Query object class as properties returning Expressions. Since we need data from multiple tables, the intermediate projection *OrderSummaryQueryResult* helps to work with data from the multiple tables. All our expressions take the *OrderSummaryQueryResult* projection and perform the appropriate conditions on them.

``` csharp   
public class OrderSummaryQuery
{
    public Expression<Func<OrderSummaryQueryResult, bool>> BelongsToUser
    {
        get
        {
            return (a) => Employee == null ||
                      a.Order.CreatedBy == Employee || a.Customer.Employee == Employee;
        }
    }

    public Expression<Func<OrderSummaryQueryResult, bool>> IsActiveOrder...
    public Expression<Func<OrderSummaryQueryResult, bool>> ForCustomer...
    public Expression<Func<OrderSummaryQueryResult, bool>> InDateRange...
}
```
``` csharp Refactored to use Expressions 
(from order in _context.Order
 join od in _context.OrderDelivery on order.Id equals od.OrderId
 join customer in _context.Customer on order.CustomerId equals customer.Id
 select new OrderSummaryQueryResult() 
    { Customer = customer, Order =    order, OrderDelivery = od })
.Where(query.IsActiveOrder)
.Where(query.BelongsToUser)
.Where(query.ForCustomer)
.Where(query.InDateRange)
```

``` sql Generated SQL when order status and employee name is set
SELECT [customer].[Name] AS [Customer], [order].[OrderNumber] AS [Number],
       [od].[Address], [order].[Created] AS [CreatedDate]
FROM [Order] AS [order]
INNER JOIN [OrderDelivery] AS [od] ON [order].[Id] = [od].[OrderId]
INNER JOIN [Customer] AS [customer] ON [order].[CustomerId] = [customer].[Id]
WHERE (([order].[Active] = 1) AND ([order].[Status] = @__OrderStatus_0)) AND 
      (([order].[CreatedBy] = @__employee_1) OR ([customer].[Employee] = @__employee_2))
```

<div class="alert alert-warning">
If you use constructor initialization for intermediate projection, *OrderSummaryQueryResult* the where clauses gets executed on the client side. So use the object initializer syntax to create the intermediate projection.
</div>

### Iteration 4 - Refactoring to Extension method

After the last iteration, we have a query that is easy to read and understand. We also have all queries consolidated within the query object, and it acts as a one place holding all the queries. However, something still felt not right, and I had a quick chat with my friend [Bappi](https://twitter.com/zpbappi), and we refined it further. The above query has too many where clauses and it was just repeating for each of the filters. To encapsulate this further, I moved all the filter expressions to be returned as an Enumerable and wrote an extension method, *ApplyAllFilters*, to execute them all.

``` csharp Expose one property for all the filters   
public class OrderSummaryQuery
{
    public IEnumerable<Expression<Func<OrderSummaryQueryResult, bool>>> AllFilters
    {
        get
        {
            yield return IsActiveOrderStatus;
            yield return BelongsToUser;
            yield return BelongsToCustomer;
            yield return FallsInDateRange;
        }
    }

    private Expression<Func<OrderSummaryQueryResult, bool>> BelongsToUser...
    private Expression<Func<OrderSummaryQueryResult, bool>> IsActiveOrder...
    private Expression<Func<OrderSummaryQueryResult, bool>> ForCustomer...
    private Expression<Func<OrderSummaryQueryResult, bool>> InDateRange...
}

... 

// Extension Method on IQueryable
{
    public static IQueryable<T> ApplyAllFilters<T>(
        this IQueryable<T> queryable,
        IEnumerable<Expression<Func<T, bool>>> filters)
    {
        foreach (var filter in filters)
            queryable = queryable.Where(filter);

        return queryable;
    }
}
```

``` csharp
{
    (from order in _context.Order
    join od in orderDeliveries on order.Id equals od.OrderId
    join customer in _context.Customer on order.CustomerId equals customer.Id
    select new OrderSummaryQueryResult() { Customer = customer, Order = order, OrderDelivery = od })
    .ApplyAllFilters(query.AllFilters)
    
    ...
}
```
The search query is much more readable than what we started with in Iteration 1. One thing you should always be careful about with EF is making sure that the generated SQL is optimized and you are across what gets executed on the server and the client. Using a SQL Profiler or [configure logging](https://docs.microsoft.com/en-us/ef/core/miscellaneous/logging) to see the generated SQL. You can also [configure to throw an exception](https://docs.microsoft.com/en-us/ef/core/querying/client-eval#optional-behavior-throw-an-exception-for-client-evaluation) (in your development environment) for client evaluation. 

Hope this helps to write cleaner and readable queries. Sound off in the comments if you have thoughts on refining this further or of any other patterns that you use. 