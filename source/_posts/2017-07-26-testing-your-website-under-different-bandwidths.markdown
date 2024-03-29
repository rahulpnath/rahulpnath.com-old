---
layout: post
title: "Tip of the Week: Testing Your Website Under Different Bandwidths"
comments: true
categories: 
- TipOW
- Web
tags: 
date: 2017-07-26
completedDate: 2017-07-26 09:49:26 +1000
keywords: 
description: How does your site behave on a lower bandwidth?
primaryImage: chrome_network_delay.png
---

With the blazing fast internet speeds at work (and at home) it might be hard to imagine how sites would work for people with a lower bandwidth. When developing for the Web, it is good to keep in mind that people from various regions across the globe might access your site. Internet speeds are not that fast around the world. So it is essential to test how your website performs for people on a lower bandwidth. 

There are a lot of plugins and external tools that can simulate a lower bandwidth scenario and makes testing on slower bandwidth easier. If you are using Google Chrome, then you already have one such tool under your belt. The [Network tab](https://developers.google.com/web/tools/chrome-devtools/network-performance/reference#throttling) under Google Chrome Developer Tools has an option to set different bandwidth profiles. Setting this to a profile that you want, and launching the site forces the site load on the set bandwidth. 

<img src="{{site.images_root}}/chrome_network_delay.png" alt="Google Chrome Developer Tools - Network Delay" class="center" />

There are many default bandwidth profiles in there, and it also allows you to add custom profiles if required. There is also an option to simulate offline mode, to test how the application behaves without an internet connection (if that applies to you). However such testing is mostly restricted to manual testing, and for automating this, you might have to look for external tools. 

Hope this helps optimize your site for lower bandwidth!



