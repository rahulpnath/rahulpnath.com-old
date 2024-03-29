---
layout: post
title: "HTTP Strict Transport Security (STS or HSTS)"
comments: true
categories: 
- Security
tags: 
date: 2018-07-10
completedDate: 2018-07-07 18:22:33 +1000
keywords: 
description: Get your website to load only over HTTPS.
primaryImage: hsts_tofu.png
---

_This article is part of a series of articles - [Ok I have got HTTPS! What Next?](/blog/ok-i-have-got-https-what-next/). In this post, we explore how to use HSTS security header and the issues it solves._

When you enter a domain name in the browser without specifying the protocol (HTTP or HTTPS) the browser by default sends the first request over HTTP. For a server that supports only HTTPS, when it sees such a request, it redirects the request over to HTTPS. The server responds to the client with a _302 Redirect_, redirecting the client to HTTPS, from which on the browser starts requesting over HTTPS. As you can see here, the very first request that the client makes is over an insecure channel (HTTP), so is also vulnerable to attacks. You could be prone to man-in-the-middle (MITM) attack, and someone could spoof that request and point you to a different site, inject malicious scripts, etc. **The first insecure HTTP request is made everytime you enter the domain name in the browser or make an explicit call over HTTP.**

<img src="{{site.images_root}}/hsts_tofu.png" alt="Trust on First Use" class="center">

> _The HTTP **Strict-Transport-Security** response header (often abbreviated as HSTS) lets a website tell browsers that it should only be accessed using HTTPS, instead of using HTTP_

By using the [HTTP Strict Transport Security](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Strict-Transport-Security) (HSTS) header on your response headers, you are instructing the browser to make calls over HTTPS instead of HTTP for your site.

```text Syntax
Strict-Transport-Security: max-age=<expire-time>
Strict-Transport-Security: max-age=<expire-time>; includeSubDomains
Strict-Transport-Security: max-age=<expire-time>; preload
```

There are a few directives that you can set on the header which determines how the browser uses the header. By just setting the header with a _max-age_ (required) directive, you tell the browser the _time in seconds that the browser should remember that a site is only to be accessed using HTTPS_. By default, the setting affects only the current subdomain. Additionally, you can set the _includeSubDomains_ directive to apply this rule to all subdomain of the site. **Before including all subdomains make sure those are served over HTTPS as well so that you do end up blocking your other sites on the same domain (if any).**

As you can see with the HSTS header specified, the browser now only makes one insecure request (the one that it makes everytime the cache expires or the very first request). Once it has established a successful connection with the server, all further requests are over HTTPS for the _max_age_ (cache expiry) set. **With the HSTS header the surface area of the attack gets reduced to just one request as compared to all initial requests going over HTTP (when we did not have the HSTS header).**

To verify the HSTS header setting has been applied for your website open your browser in Incognito/In-Private browsing mode. It is to make sure that the browser acts as if it is seeing the site for the very first time (as HSTS header caches do not get shared across regular/incognito sessions)

> _The HSTS header settings do not get shared across between the regular and incognito browsing session (at least in Chrome and think this is the same for other browsers as well)._

Open the Developer tools window and monitor the Network requests made by the browser. Request your website over HTTP (either explicitly or just entering the domain name) in this case my blog [http://rahulpnath.com](http://rahulpnath.com). As you can see the very first request go over HTTP and the server returns a _301 Moved Permanently_ status with the https version of the site. For any subsequent requests over HTTP, the browser returns a _307 Internal Redirect_. This redirect happens within the boundary of your browser and redirects to the HTTPS site. You can use [Fiddler](https://rahulpnath.com/blog/fiddler-free-web-debugging-proxy/) to verify that this request does not cross the browser boundary. (The request does not get to Fiddler.)

<img src="{{site.images_root}}/hsts_without_preload.png" alt="HSTS without preload" class ="center" />

We could still argue that there is still a potential threat with the very first request sent over HTTP which is still vulnerable to MITM attack. To solve that we can use the _preload_ directive and submit out domain to an [HSTS Preload list](https://hstspreload.org/), which when successfully added, propagates to the source code of Browsers.

> _Most major browsers (Chrome, Firefox, Opera, Safari, IE 11 and Edge) also have HSTS preload lists based on the Chrome list._

The Browsers hardcode these domains from the preload approved list into their source code (e.g., here is [Chrome's list](https://chromium.googlesource.com/chromium/src/net/+/master/http/transport_security_state_static.json)) and gets shipped with their releases. You can check for the preloaded site in the browser as well. Again for chrome navigate to _chrome://net-internals/#hsts_ and query for the HSTS domain.

<img src="{{site.images_root}}/hsts_preload_static_json.png" alt="HSTS preloaded site hardcoded" class ="center" />

If STS is not set at all or you have not made the very first request to the server (when preload is false) querying for the domain returns 'Not Found.' Below are two variations that you can see depending on whether you have _preload_ set or not. The _dynamic\_\*\_\*_ indicates that STS is set after the first load of the site and the _static\_\*\_\*_ indicates that it is set from the preload list.

> _If you are wondering why this blog does not have \_static\_\*\_\*_ set it is because the preload list that it is part of has not yet made into a stable version of Chrome. However, the preload site does show that it is currently preloaded (probably in a beta version at the time of writing).\_

<img src="{{site.images_root}}/chrome_hsts_preload.png" alt="Verifying HSTS preload" class ="center" />

With the preload set and your domain hardcoded into the preload list and available as part of the browser version you are on, any request made over HTTP is redirected internally (307) to HTTPS without even going to the server. It means **we have entirely got rid of the first untrusted HTTP request.**

<img src="{{site.images_root}}/hsts_internal_redirect_scott.png" alt="HSTS preload request flow" class ="center" />

Have you already got HSTS set on your site?
