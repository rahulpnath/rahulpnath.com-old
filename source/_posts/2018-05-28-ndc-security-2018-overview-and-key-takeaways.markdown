---
layout: post
title: "NDC Security 2018 - Overview and Key Takeaways"
comments: true
categories:
- Security
- Programming 
tags: 
date: 2018-05-28
completedDate: 2018-05-26 07:17:09 +1000
keywords: 
description: Some key takeaways from the security conference held in Gold Coast.
primaryImage: ndc_security.jpg
---

While in Sydney I was lucky enough to have attended the [first](https://rahulpnath.com/blog/ndc-sydney/) and [second](https://rahulpnath.com/blog/ndc-sydney-2017/) NDC Conferences. After moving up to Brisbane, did not think I could attend one of these soon. However, then comes a nice shorter version of NDC specific to Security - [NDC Security](https://ndcsecurity.com.au/). As the name suggests, this conference is particular to security-related topics with a 2-day workshop and 1-day conference, as was held in Gold Coast, Queensland.

### The Workshop

[Troy Hunt](https://www.troyhunt.com/) and [Scott Helme](https://scotthelme.co.uk/) ran [two workshops](https://ndcsecurity.com.au/workshops/) and I attended [Hack Yourself First](https://ndcsecurity.com.au/workshop/hack-yourself-first-how-to-go-on-the-cyber-offence/) by Troy. The workshop covers a wide range of topics and is perfect for anyone who is into web development. The best thing is that you only need to have a browser and [Fiddler](https://www.telerik.com/download/fiddler)/[Charles Proxy](https://www.charlesproxy.com/download/) (depending on whether you are on Windows or Mac land). One of the interesting thing about the workshop is that it puts you first into the hackers perspective and forces you to exploit existing vulnerabilities in the [sample site](http://hackyourselffirst.troyhunt.com/) designed specifically for this. Once you can do this, we then look at ways of protecting ourselves against such exploits and other mechanisms involved.

<img src="{{site.images_root}}/ndc_security_hyf.jpg" class="center" alt="Hack yourself first, Troy Hunt">

The workshop highlights how easy it is to find and exploit vulnerabilities in applications. Some tools detect vulnerabilities and exploit them for you if you input a few details to them. You necessarily need not know the vulnerabilities itself or how exactly to exploit them. Such tools make it easy for people to use them on any website that is out there on the web. Combined with the power of search engines it makes it quite easy to make your site vulnerabilities to be easily discoverable.

### The Conference

There were [six talks](https://ndcsecurity.com.au/agenda/) in total and below are the ones that I found interesting.

* [Scott Helme Talk: CSP XXP STS PKP CAA ETC OMG WTF BBQ…](https://ndcsecurity.com.au/talk/csp-xxp-sts-pkp-caa-etc-omg-wtf-bbq/)
* [Talk: Dependable Dependencies](https://ndcsecurity.com.au/talk/dependable-dependencies/)
* [Everything is Cyber-broken](https://ndcsecurity.com.au/talk/everything-is-cyber-broken/)

<img src="{{site.images_root}}/ndc_security_conference.jpg" class="center" alt="NDC Securtiy, 2018 - Conference">

The whole web is on a journey towards making it more secure. So it is an excellent time to [move on to HTTPS](https://rahulpnath.com/blog/https-for-free-and-why-you-should-care/) if you are not already. Even after enabling HTTPS, it is a good idea to make sure you have got all the appropriate [security headers](https://securityheaders.com/) set. Making sure that the libraries that you depend on are patched and updated is equally essential.
There are incidents of massive data breaches because of vulnerabilities in third-party libraries and not keeping them updated.

> _Functionality need not be the only reason to upgrade third-party libraries. There might be security vulnerabilities that are getting patched which is an equally good reason to update dependent packages_

The harder thing is to keep track of the vulnerabilities that are getting reported and always checking back with your application's dependencies. There is a wide range of tools that help make this easy and seamlessly integrate within the development workflow. It can be included as early as when a developer intends to include a library into the source code, or in the build pipeline or even for sites that are up and running. The earlier such issues get detected in the software development lifecycle, the less costly and impact it has on time and cost.

#### **Tools**

* [Sonarwhal](https://sonarwhal.com/scanner/)
* [Snyk](https://snyk.io/)
* [Retire.js](https://retirejs.github.io/retire.js/)
* [Netsparker](https://www.netsparker.com/)
* [OWASP Zed Attack Proxy Project](https://www.owasp.org/index.php/OWASP_Zed_Attack_Proxy_Project)
* [Ultimate List of Security Links](https://www.troyhunt.com/troys-ultimate-list-of-security-links/)

The conference ended with a good discussion between Troy and Scott on how everything is Cyber broken. It touches upon the value of Extended Validation (EV) Certificate and how CA's are trying to push for them while browsers are more and more going away from them. It also touches on various proponents of HTTP and the wrong messages that are getting spread to a broader audience and also about [certificate revocations](https://scotthelme.co.uk/revocation-is-broken/) and a lot more. It was a fun discussion and a great end to the three-day event.

### Location and Food

NDC Security was held at [QT Gold Coast, Queensland](https://www.qthotelsandresorts.com/gold-coast/) and well organized. Coffee and drinks were available all throughout the day with a barista on the last day (which was cool). Food was served at start, breaks, and lunch and was good. The conference rooms were great and spacious and had reasonable good internet. Did not face much connectivity issues and everything ran smoothly.

<img src="{{site.images_root}}/ndc_security_food_location.jpg" class="center" alt="NDC Securtiy, 2018 - Food and Location">

One of the things I first did after coming from the conference was to [move this blog over to HTTPS](https://rahulpnath.com/blog/https-for-free-and-why-you-should-care/). I had been procrastinating long on this, but there were enough reasons to make a move now. Also, there are a bunch of things that catch my eye at client places and other web sites that I visit often. Attending the conference and workshop has been a great value add and recommend to anyone if you have a chance to attend that. For the others, most of the content is available in [Pluralsight](https://www.pluralsight.com/).

_PS: Special thanks to [Readify](https://rahulpnath.com/blog/finding-a-job-abroad/) for sending me to this conference and also providing a 'paid vacation (accommodation)' in Gold Coast. It was a nice three-day break for my wife and son also._
