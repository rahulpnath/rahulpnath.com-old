---
layout: post
title: "Being Explicit About Time when Handling Multiple Timezones"
comments: true
categories: 
tags: 
thisIsStillADraft:
keywords: 
description: 
---
Dealing with date/time in application that affect different time zones becomes quite tricky. The general recommendation is that all dates to be saved in UTC time and convert them as required. This might work well if all the data used is persisted by the application itself and all the developers ensure that at the application boundaries the date is converted to UTC.  
At one of my clients, we are facing issues with time and I have been thinking about an approach to solve this. The client dells office spaces across the globe and the application is an internal facing application for their employees to sell/manage office spaces. It integrates with various other back end systems and provides a single point of access for everything, aggregating data across those different  systems and itself. Some of the backend systems are in different locations and  save/respond with times local to them. This increases the challenge when sending/retrieving data from them. The application has defined set of locations, identified by ,three letter location codes (SYD, TRV, SEA), and these locations are fall in one of the timezones. 

> This article is me just putting my thoughts together on what I think might be a good solution for this. 

 The more I think of it, I feel that the issue with dealing with time is about not being explicit about it. Across the domain where time is used/consumed we use either datetime or datetimeoffset. The problem with using this is neither of the fits into the application domain nicely. 