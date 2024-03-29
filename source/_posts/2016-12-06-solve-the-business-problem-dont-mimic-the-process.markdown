---
layout: post
title: "Solve the Business Problem, Don't Mimic The Process"
comments: true
categories: 
- Thoughts
- Programming
tags: 
date: 2016-12-06
completedDate: 2016-11-24 05:00:40 +1100
keywords: 
description: Describes on the importance of seeing the computing problems attached to a business problems when automating business processes.
primaryImage: business_problem_automation.jpg
---
<a href="http://www.dhc-gmbh.com/en/kompetenzen/business-process-management/"><img class="center" alt="Business" src="{{ site.images_root}}/business_problem_automation.jpg"/></a>

While working with clients I often get into conversations with the domain experts and people involved directly with the business. The discussions usually happen around what the process they are currently doing and how to automate those. Knowing the business process is helpful but getting influenced by that to design the solution is often not effective. Recently our team was in a conversation with a domain expert for a new feature request. 

<div class="alert alert-warning">
The series of events mentioned below were modified to fit into the conversational style used in the post.
</div>

> ***Domain Expert** We need to charge customers a processing fee if they pay using an electronic payment method. Depending on the type of card (mastercard, visa etc) the processing charge percentage differs. The processing fees are always charged in the subsequent billing period after their current payment. For e.g. if a customer pays 1000$ for the month of November, then his December bill will have 2% card processing charge in the December invoice* 

> ***Team** That sounds easy, think we have enough details to get started on this. Thank you.*

> ***Domain Expert** Perfect. Ahhh... Before you go, I think this can be a [hangfire job](http://hangfire.io/) that runs on 29 every month, a few days before the billing date, 3rd, and generate these charges for the client. This is what we do manually at present. (And walks off)*

> ***Team** Discussing amongst themselves the team agreed that creating a recurring job is the way to go. Based on the assumption that this job will be run only once a month, the job was to read all the invoices from 29th of the previous month till 28th of the current month and charge the clients. The meeting was dismissed and off went everyone busy to get the new feature out*

### Business has Exceptions

Problems started coming up the immediate month of feature deployment. Below is the sequence of events that happened.

  - **29th** : Nice work, team! The processing charges have been applied as expected.
  - **30th** : Some of the invoices have wrong data. We have deleted them. Can you run the job?
  - **2nd** : A few of our clients (as usual) paid late and we need to charge their processing fees. Can you run the job?
  - **15th** : One of our clients is ending tomorrow, so we need to send them an invoice and it should include the processing fees for their last payment. Can you run the job?

But wait! We had decided that we will run this job only once a month and that is the only time we need to process the charges. We cannot run that job over and over again. 

> *What I’ve noticed over the years is that our users find very creative ways to achieve their business objectives despite the limitations of the system that they’re working with. We developers ultimately see these as requirements, but they are better interpreted as **workarounds**.*

> ***- [Udi Dahan](http://udidahan.com/2013/04/28/queries-patterns-and-search-food-for-thought/)***

The business was right when it said that *'This is what we do manually at present.* What they did not say though is that there were always exceptions. And in these cases, they did the same process, but just for those exceptions. Business process mostly will be around the majority of the cases and the exceptions always get handled ad-hoc. So for the business it's always that which takes a good part of their time that matters more. 

### Finding the Way Out

The problem, in this case, was that the team modeled the solution exactly as the business did manually. Think kind of a solution is most likely to fail in case of exceptions. The human brain can easily deal with these exceptions. But for a program to solve it that way it needs to be told so, which implies that there need to be alternate flow paths defined. So with the improved understanding of these exception cases, the team does another analysis through the problem. After some discussion the team re-defines their original problem statement - *We need to be able to run the job any number of times and it should have the same effect. 

> *A payment should get *one and only one* processing charge associated, no matter however times it is seen by the job.* 

With the new implementation, we decided to maintain a list of payments (a strong identifier that does not change) we have seen and processed. So every time a payment is seen, it is matched to see if it is already processed. If a charge is not already applied, a charge is applied and added to the list of processed payments. This ensures that they can run the job anytime. The team added in capability to specify the time range to look for invoices. By default, this ranged from 29th - 28th. The team also added in a way to void out payment charges applied, so that whenever the invoices changed then can just clear that off and re-run the job. These changes gave the flexibility to meet the businesses exception cases.  
   

### Idempotent

> *The term [idempotent](http://www.enterpriseintegrationpatterns.com/patterns/messaging/IdempotentReceiver.html) is used in mathematics to describe a function that produces the same result if it is applied to itself, i.e. f(x) = f(f(x)). In Messaging this concept translates into a message that has the same effect whether it is received once or multiple times. This means that a message can safely be re-sent without causing any problems even if the receiver receives duplicates of the same message.*

Being idempotent is what we missed with the first implementation. There was an assumed 'idempotency' that the job will be run only once a month. But this constraint is not something that the code had control of and something it could enforce. The job was also not idempotent at the granular level that it was affecting - payments. Asserting idempotency at the batch level fails when we want to re-run batches (when exceptions like the wrong invoice happens). Idempotency should be enforced at the unit level of change, which is what maintaining a list of processed payments helps with. Any payment that is not processed before will get processed now when the job is run. We can also ensure that the payment will only be processed at most once.

This is just an example where we fail to see beyond the business problem and also see the computing problems accompanying it. Not always will it be easy and fast to rewrite the code. Even if we fail to see these problems the business will eventually make us to. But it is when we can see the computing problems that accompanies a business problem that we start becoming better developers. Applying basic computing principles, probing the domain expert during discussions, sitting with domain experts while they work etc. are all good ways to start seeing the untold business processes. Hope this helps the next time you are into a meeting with domain expert or solving a business problem.