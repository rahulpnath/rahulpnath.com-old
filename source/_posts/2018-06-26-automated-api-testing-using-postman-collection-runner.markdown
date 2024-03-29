---
layout: post
title: "Automated API Testing Using Postman Collection Runner"
comments: true
categories: 
- Testing
tags: 
date: 2018-06-26
completedDate: 2018-06-26 12:06:02 +1000
keywords: 
description: Quick and easy way to test your API.
primaryImage: postman_collection_runner.png
---

A while back we looked at how we can use [Postman to chain multiple requests to speed up out Manual API Testing](https://rahulpnath.com/blog/postman-chaining-requests-to-speed-up-manual-api-tests/). For those who are not familiar with [Postman](https://www.getpostman.com/), it is an application that assists in API testing and development, which I see as sitting a level top of a tool like [Fiddler](https://rahulpnath.com/blog/fiddler-free-web-debugging-proxy/).

In this post, we will see how we can use Postman to test some basic CRUD operations over an API using a feature called [Postman Runner](http://blog.getpostman.com/2016/11/22/postmans-new-collection-runner/). Using this still involves some manual intervention. However, we can automate them using a combination of different tools.

### Setting Up the API

To start with I create a simple API endpoint using the out of the box Web API project from Visual Studio 2017. It is a Values Controller which stores key-value pairs to which you can send GET, POST, DELETE requests. Below is the API implementation. It is a simple in-memory implementation and does not use any persistent store. However, the tests would not change much even if the store was to be persistent. The importance here is not in the implementation of the API, but how you can use Postman to add some quick tests.

```csharp ValuesController
public class ValuesController : ApiController
{
    static Dictionary<int, string> values = new Dictionary<int, string>();

    public IEnumerable<string> Get()
    {
        return values.Values;
    }

    public IHttpActionResult Get(int id)
    {
        if (values.ContainsKey(id))
            return Ok(values[id]);

        return NotFound();
    }

    public IHttpActionResult Post(int id, [FromBody]string value)
    {
        values[id] = value;
        return Ok();
    }

    public IHttpActionResult Delete(int id)
    {
        if (!values.ContainsKey(id))
            return NotFound();

        values.Remove(id);
        return Ok();
    }
}
```

### Setting Up Postman

To start with we will create a new Collection in Postman to hold our tests for the Values Controller - I have named it '_Values CRUD - Test_'. The collection is a container for all the API requests that we are going to write. First, we will add all the request definitions into postman which we can later reorder for the tests.

<img src="{{site.images_root}}/postman_request.png" alt="Postman Request" class="center" />

> The _**{{ValuesUrl}}/{{ValueId}}**_ in the URL are parameters defined as part of the selected Environment. Environments in Postman allow you to switch between different application environments like Development, Test, Production. You can configure different values for each environment and Postman will send the requests as per the configuration.

Below are the environment variables for my local environment. You can define as many environments as you want and switch between them.

<img src="{{site.images_root}}/postman_environment.png" alt="Postman Environment" class="center" />

Now that I have all the request definitions for the API added let's add some tests to verify our API functionality.

### Writing The First Test

Postman allows executing scripts before and after running API requests. We did see this in the [API Chaining post](https://rahulpnath.com/blog/postman-chaining-requests-to-speed-up-manual-api-tests/) where we grabbed the _messageId_ from the POST request and added it to the environment variable for use in the subsequent requests. Similarly, we can also add scripts to verify that the API request returns expected results, status code, etc.

Let's first write a simple test on our GET API request that it returns a 200 OK response when called. The below test uses the [Postmans PM API](https://www.getpostman.com/docs/v6/postman/scripts/postman_sandbox_api_reference) to assert that status code of the response is 200. Check the _[Response Assertion API in test scripts](https://www.getpostman.com/docs/v6/postman/scripts/postman_sandbox_api_reference#response-assertion-api-in-test-scripts)_ to see the other assertion options available like _pm.response.to.have.status_. The tests are under the Tests section similar to where wrote the scripts to chain API requests. When executing the API request, the Tests tab shows the successful test run for the particular request.

```javascript 200 Status Code
pm.test("Status code is 200", function() {
  pm.response.to.have.status(200);
});
```

<img src="{{site.images_root}}/postman_tests.png" alt="Postman Tests" class="center" />

Similarly, you can also write _Pre-request Script_ to set variables or perform any other operation. Below I am setting the _Value_ environment variable to "Test". You could generate a random value here or set a random id, or set an identifier that does not already exists. It's test/application specific, so leave it you to decide what works best for you.

```javascript Pre-request Script.
pm.environment.set("Value", "Test");
```

### Collection Runner

The collection runner allows you to manage multiple API requests and run them as a set. Once completed it shows a summary of all the tests included within each request and details of tests that passed/failed in the run. You can target the Runner to run against your environment of choice.

<img src="{{site.images_root}}/postman_collection_runner.png" alt="Postman Collection Runner" class="center" />

Running these tests still involves some manual effort of selecting environments and running them. However using [Newman](https://www.npmjs.com/package/newman), you can run Postman Collections from the command line, which means even in your build pipeline.

Using Postman, we can quickly test our API's across multiple environments. The Collection Runner also shows an excellent visual summary of the tests and helps us in API development. However, I found these tests to violate the [DRY principle](https://en.wikipedia.org/wiki/Don%27t_repeat_yourself). You need to repeat the same API request structure if you have to use them in a different context. Like in the example above I had to create two _Get Value By Id_ requests to test for the value existing and also for when it does not exists. You could use some conditional looping inside the scripts, but then that makes your tests complicated and gets into the loop of how to test your tests. Postman does allow you to export the API request to the language of your choice. So once you have the basic schema, you can export them and write tests that compose them. I find Postman tests and the Runner a quick way to start testing your API endpoints and then for more complicated cases use a stronger programming language. Having the tests in Postman also allows us to have an API spec in place and can be useful to play around with the API.
