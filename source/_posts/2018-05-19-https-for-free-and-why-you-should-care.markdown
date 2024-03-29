---
layout: post
title: "HTTPS For Free and Why You Should Care"
comments: true
categories: 
- Blogging
- Security
tags: 
date: 2018-05-19
completedDate: 2018-05-19 19:50:30 +1000
keywords: HTTPS, Free, Cloudflare, Azure
description: Upgrade to HTTPS for free
primaryImage: blog_https.png
---

If you are here and reading this probably you have a website and is serving it over HTTP. If you are unsure of whether your site needs HTTPS or not, don't think twice - **YES, YOUR SITE NEEDS HTTPS**.

If you are not convinced check out [https://doesmysiteneedhttps.com/](https://doesmysiteneedhttps.com/). One of the main reasons that I have seen (including me) why people have shied away from having HTTPS on sites was cost. And this post explains how to get HTTPS for free. But make sure you are getting it for the correct reasons and you know exactly what you are getting

<div class="center">
    <blockquote class="twitter-tweet" data-lang="en"><p lang="en" dir="ltr">HTTPS &amp; SSL doesn&#39;t mean &quot;trust this.&quot; It means &quot;this is private.&quot; You may be having a private conversation with Satan.</p>&mdash; Scott Hanselman (@shanselman) <a href="https://twitter.com/shanselman/status/187572289724887041?ref_src=twsrc%5Etfw">April 4, 2012</a></blockquote>
    <script async src="https://platform.twitter.com/widgets.js" charset="utf-8"></script>
</div>

Depending on how you are hosting you could possibly take two routes to enable HTTPS on your site. Let's look at them in detail.

### Option 1 - Get your Certificate and Add to Your Host

If your hosting service already allows you to upload a custom domain certificate, but you were just holding back because of the extra cost of getting a certificate, then head over to [Let's Encrypt](https://letsencrypt.org/) to get your free certificate. Again depending on your hosting provider and the level of access that you have on your web server, Let's Encrypt has [muliple ways](https://letsencrypt.org/getting-started/) on how you can get a certificate.

> [**What does it cost to use Let’s Encrypt? Is it really free?**](https://letsencrypt.org/docs/faq/#general)  
> _We do not charge a fee for our certificates. Let’s Encrypt is a nonprofit, our mission is to create a more secure and privacy-respecting Web by promoting the widespread adoption of HTTPS. Our services are free and easy to use so that every website can deploy HTTPS._

> _We require support from generous sponsors, grantmakers, and individuals in order to provide our services for free across the globe. If you’re interested in supporting us please consider donating or becoming a sponsor._

> _In some cases, integrators (e.g. hosting providers) will charge a nominal fee that reflects the administrative and management costs they incur to provide Let’s Encrypt certificates._

### Option 2 - CloudFlare

If you are like me on a shared/cheaper hosting service it is more likely that your hosting plan does not support adding SSL certificates. You will be forced to upgrade to a higher plan to upload a certificate, which in turn will cost you more. In this case, you can use [Cloudflare](https://www.cloudflare.com/), to enable HTTPS for free.

Cloudflare provides lots of features for websites, but in our case, we are more interested in what the Free plan gives us. It gives us a Shared SSL Certificate and also added benefits of Global CDN.

<img src="{{site.images_root}}/cloudflare_free_plan.png" class="center" />

Cloudflare acts as a [reverse proxy](https://en.wikipedia.org/wiki/Reverse_proxy) between you and the server hosting this web page, which simply means that all requests now go through Cloudflare which in turn reaches out to the web server, if it cannot find a locally cached copy. So this also means that there are now reduced number of calls to the web server as Cloudflare would serve it from its cache if already available.

Shared SSL is what is more interesting for us as part of this blog post. What shared SSL gives us is free HTTPS for our website. We get a Domain Validated (DV) certificate, with a small catch. It is not issued to our domain but to a shared Cloudflare domain server (sni154817.cloudflaressl.com in my case). If you want a custom SSL certificate then you need to be on a paid plan.

<img src="{{site.images_root}}/blog_ssl_certificate.png" class="center" />

Cloudflare supports [multiple SSL settings](https://support.cloudflare.com/hc/en-us/articles/200170416) - Off, Flexible SSL, Full SSL, Full SSL(Strict). Depending on how your host is setup you can choose one of the options. Since I am using [Azure Web Apps](https://azure.microsoft.com/en-au/services/app-service/web/) to host, it supports https over _\*.azurewebsites.net_ subdomain. But since the certficate is not for my custom domain name (rahulpnath.com), I have set the SSL setting to Full SSL. Cloudfare in this case will connect over HTTPS but not validate the certificate. If your host does not support HTTPs connection (for free) you can use Flexible SSL.
<a href="https://www.cloudflare.com/ssl/" >
<img src="{{site.images_root}}/cloudflare_ssl_modes.png" class="center" />
</a>

You can also choose to enable Cloudflare with Full SSL(Strict) if you have followed Option 1 and have a custom SSL certificate for the domain. This will give you the added benefits that Cloudfare provides.

### Enabling HSTS Preload

Now that you have HTTPS setup on your domain with either of the options above, we can see that the website is now accessible over HTTPS. However, when you make the very first request to the website, the request goes over HTTP which then redirects over to HTTPS, after which the communication happens over a secure channel. However, there is a risk where the very first request can be intercepted and cause undesired behaviour.

> _[Trust on first use](https://en.wikipedia.org/wiki/Trust_on_first_use) (TOFU), or trust upon first use (TUFU), is a security model used by client software which needs to establish a trust relationship with an unknown or not-yet-trusted endpoint._

By setting the [STS(Strict-Transport-Security)](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Strict-Transport-Security) header along with the preload directive, we can then add our domain to the [HSTS Preload list](https://hstspreload.org/). By adding your domain into this list it is literally getting hardcoded into source code of browsers (like for e.g [Chrome here](https://chromium.googlesource.com/chromium/src/net/+/master/http/transport_security_state_static.json)). So anytime a request is made to a site it is checked against this hardcoded list available in memory and if present the request goes as HTTPS from the very first. You can set all subdomains for your domain as well as HSTS preloaded. Make sure you have all subdomains are served over HTTPS so that you do not lock yourself out on those sites. You can find more details on HSTS [here](https://scotthelme.co.uk/hsts-the-missing-link-in-tls/).

Now that the cost factor is out of making your site support HTTPS, is there anything else that is holding you back? If speed is a concern and it worries that encryption/decryption at both ends of communication is going to slow you down take a look at this post on [HTTPS' massive speed advantage](https://www.troyhunt.com/i-wanna-go-fast-https-massive-speed-advantage/).
If you are still not convinced let me give it one last shot to get you on board. Going forward most modern browsers are going to default to the web as a secure place. So instead of the present positive visual security indicators, it would start showing warnings on pages served over HTTP. That means soon your sites would start showing <span style="color:red;">Not Secure</span> if you are not moving over to HTTPS.

<a href="https://blog.chromium.org/2018/05/evolving-chromes-security-indicators.html" >
    <img src="{{site.images_root}}/chrome_https.png" class="center" />
</a>

I don't see any reason why we should still be serving our sites over HTTP. As you can see I have moved over to the HTTPS and have added this domain to the preload list as well. Let's make the web secure by default!
