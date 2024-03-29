---
layout: post
title: "Enable Cross-Origin Requests (CORS) in ASP.Net Web API Using CorsOptions"
comments: true
categories:
- Web Api
tags: 
date: 2018-08-14
completedDate: 2018-08-14 06:52:18 +1000
keywords: 
description: Don't use Cors.AllowAll - Set it up correctly!
primaryImage: cors.png
---

I was setting up an API at one of the client's place recently and found that currently, they allow any origin to hit their API by setting the [CorsOptions.AllowAll](<https://msdn.microsoft.com/en-us/library/dn450212(v=vs.113).aspx>) options. In this post, we will look at how to set the CORS options and restrict it to only the domains that you want your API to be accessed from.

### What is Cross-Origin Resource Sharing (CORS)

Cross-Origin Resource Sharing is a way to relax the browsers [Same-Origin Policy](https://developer.mozilla.org/en-US/docs/Web/Security/Same-origin_policy) whereby to tell a browser to let a web application running at one origin (domain) have permission to access selected resources from a server at a different origin. By specifying the CORS header you instruct the browser to allow all allowed domains to access your resource. Most of the time for the API endpoints you want to be explicit on the hosts that can access your API. By setting CORS, you are only restricting/allowing cross-domain access originating from a browser. **Setting CORS should not be mistaken for a Security feature** whereby you are restricting access from any other sources. Any requests that are formed outside of the browser like using Postman, Fiddler, etc. can still make to your API and you need appropriate authorization/authentication to make sure you are not exposing data to unintended people.

<img src="{{site.images_root}}/cors.png" alt="Cross-Origin Request" class="center" >

### Enabling in Web API

In Web API there are multiple ways that you can set CORS.

- [Microsoft.Owin.Cors](https://www.nuget.org/packages/Microsoft.Owin.Cors/)
- [Microsoft.AspNet.WebApi.Cors](https://www.nuget.org/packages/Microsoft.AspNet.WebApi.Cors)  
  _This adds *System.Web.Http.Cors.dll* assembly to your project so can be a bit of confusion if you are looking around for the DLL in the solution_

In the below snippet I am using the _Microsoft.Owin.Cors_ pipeline to setup CORS for the API. The code first reads the application configuration file to get a list of semicolon (;) separated hostnames which are added to the list of allowed origins in the [CorsPolicy](<https://docs.microsoft.com/en-us/previous-versions/aspnet/web-frameworks/dn726408(v=vs.118)>). By setting the corsOptions with _UseCors_ extension method, the policy gets applied to all the requests coming through the website.

```csharp
var allowedOriginsConfig = ConfigurationManager.AppSettings["origins"];
var allowedOrigins = allowedOriginsConfig
    .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

var corsPolicy = new CorsPolicy()
{
    AllowAnyHeader = true,
    AllowAnyMethod = true,
    SupportsCredentials = true
};
foreach (var origin in allowedOrigins)
    corsPolicy.Origins.Add(origin);

var policyProvider = new CorsPolicyProvider()
{
    PolicyResolver = (context) => Task.FromResult(corsPolicy)
};
var corsOptions = new CorsOptions()
{
    PolicyProvider = policyProvider
};

app.UseCors(corsOptions);
```

### Setting Multiple CORS Policy

If you want to have different CORS policies based on different Controllers/route path, you can use the _Map_ function to set up the CorsOptions for specific route paths. In the below example we apply a different CorsOptions to all routes that match _'/api/SpecificController'_ and defaults to another for all other requests.

```csharp
app.Map(
    "/api/SpecificController",
    (appbuilder) => appbuilder.UseCors(corsOptions2));
...
app.UseCors(corsOptions1);
```

### CORS ≠ Security

CORS is a way to relax the Cross-Origin Policy and in no way should be seen as a security feature. By setting CORS headers what we are saying is to allow all the additional domains in the headers also to be able to access the resource from a browser environment. However setting this, does not restrict access to your API's from other sources like Postman, Fiddler or from any non-browser environments. Even within browser environments, older versions of Flash allows modifying and spoofing of request headers. Ensure that you are using CORS for the correct reasons and not assume that it is providing you security against unauthorized access.

Hope this allows you to setup CORS on your API's!
