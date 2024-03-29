---
layout: post
title: "Tip of the Week: Postman - Chaining Requests to Speed Up Manual API Tests"
comments: true
categories: 
- TipOW
tags: 
date: 2017-12-11
completedDate: 2017-12-09 16:14:48 +1000
keywords: 
description: Pass data from response of one API to another API request.
primaryImage: 
---

I was recently playing around with [MessageMedia API](https://www.messagemedia.com.au/) trying to [send SMS and get the status of the SMS sent](https://developers.messagemedia.com/code/messages-api-documentation/). Sending the SMS and getting the status of the last sent SMS always happened in succession when testing it manually. Once I send the message, I waited for the API response, grabbed the message id from the response and used that to form the get status request.

[Postman](https://www.getpostman.com/) is a useful tool if you are building or testing APIs. It allows to create, send and manage API requests. 

<img src="{{site.images_root}}/postman_chaining_requests.png" alt="Postman Chaining Requests" />

I added two requests and saved it to a collection in Postman - one to Send Message and other to Get Message status. I have created an environment variable for holding the message id. For the request that sends a message, the below Test snippet is added. It parses the response body of the request and extracts the message id of the last send message. This is then saved to the environment variable. The [Test](https://www.getpostman.com/docs/postman/scripts/test_scripts) snippet is always run after performing the request.

``` javascript
var jsonData = JSON.parse(responseBody);
postman.setEnvironmentVariable("messageId", jsonData.messages[0].message_id);
tests["Success"]= true;
```

The Get message request uses the messageId from the environment variables to construct its URL. The URL looks like below. 

{% raw %}
``` text
https://api.messagemedia.com/v1/messages/{{messageId}}
```
{% endraw %}

When executing this request, it fetches the messageId from the environment variable, which is set by the previous request. You no longer have to copy message id manually and use it in the URL. This is how we chain the data from one request to another request. Chaining requests is also useful in automated testing using Postman. Hope this helps!