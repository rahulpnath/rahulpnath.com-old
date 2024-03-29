---
layout: post
title: "Ok I Have Got HTTPS! What Next?"
comments: true
categories: 
- Security
tags: 
date: 2018-07-09
completedDate: 2018-07-07 09:22:59 +1000
keywords: 
description: 
primaryImage: https_next.png
---

Security is of more and more a concern these days and we [saw why it's important to move on to HTTPs](https://rahulpnath.com/blog/https-for-free-and-why-you-should-care/). Hope you have already moved on to HTTPS, If not right now is a perfect time - this is one of the things that you should not be putting off for later. Also, do check out the [HTTPS is Easy](https://httpsiseasy.com/) series by Troy Hunt on how simple it is to get onboard with HTTPS. Most of the things mentioned here, I started exploring after getting introduced to it by [Scott Helme](https://scotthelme.co.uk/) and [Troy Hunt](https://www.troyhunt.com/) at the [NDC Security conference](https://rahulpnath.com/blog/ndc-security-2018-overview-and-key-takeaways/).

<a href="https://securityheaders.com/?q=rahulpnath.com&followRedirects=on">
    <img
        src="{{site.images_root}}/https_next.png"
        alt="Security Report Summary from SecurityHeaders.com"
        class="center" />
</a>

Once you have moved on to HTTPS you might be thinking is that it? Or is there still more that I need to be doing? An excellent place to start is [SecurityHeaders.com](https://securityheaders.com/), which analyses security headers on your website and provides a rating score for the site. The site also gives a short description of the various headers, appropriate links to explore more about them. Some of the headers are easy to add and immediately provides added security to your website.

<div class="alert alert-warning">
Just because you have an A+ Rating (or a good rating) does not mean that your site is not vulnerable to any attacks. These are just some guidelines to help you along the way to tighten up your website security.
</div>

> _I have been trying to walk the talk here - implementing these headers one by one on this blog as and when I am writing this_

In this post will walk through some of the headers that I added to this blog. I am planning to write this as a multi-series article, with each article specific to a header/other feature, why I added it and how I went about adding and verifying it.

- [HTTP Strict-Transport-Security (HSTS/STS)](/blog/http-strict-transport-security-sts-or-hsts)
- [Content-Security-Policy (CSP)](/blog/http-content-security-policy-csp/)
- [Subresource Integrity](/blog/subresource-integrity-sri/)
- Development Tools

I will be updating the links to the relevant posts here as I publish them and will add to the list as and when I come across new topics. One of the tricky thing with security is that very often something new comes up. The best we can do is to proactively do things that we can do to support and protect ourselves.
