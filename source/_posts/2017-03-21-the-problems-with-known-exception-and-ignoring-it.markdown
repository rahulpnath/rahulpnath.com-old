---
layout: post
title: "The Problems with Known Exceptions and Ignoring It"
comments: true
categories: 
- Programming
- Thoughts
tags: 
date: 2017-03-21
completedDate: 2017-03-10 12:53:01 +1100
keywords: 
description: Having known errors and exceptions in the applications we develop and maintain can have a larger impact than we think.
primaryImage: known_errors.png
---

"*Oh yes! That is an expected error. It is because…*".

How many times have you given that explanation yourself or heard the other developer tell that? Known errors or exceptions are common in applications and us developers find ways to live with such errors. At times when the number of such errors grows it becomes a problem directly or indirectly to the business. These known errors could either be exceptions in application logs, failed messages (commands/events) in a [message based architecture](https://en.wikipedia.org/wiki/Event-driven_architecture), alert popups in Windows client applications, etc.

<img alt="Known Errors" src="{{site.images_root}}/known_errors.png" />  

We should try and keep known errors and exception count close to zero. Below are some of the problems that can happen by ignoring it over a period.

#### **Business Value** ####

Since the errors are known to us, we train ourselves or even the users to ignore them. It is easy to justify that fixing them does not have any business value as there is no direct impact. This assumption need not be true. If a piece of code has no value then why is it there in the first place? Possibly it is not having any visible effects at present but might be having an impact at a later point in time. It could also be that it is not affecting the data consistency of your system, but is a problem for an external system. There can be business flows that are written at a later point of time not being aware of this known error. Some developer time gets lost when glancing over such errors or messages in the log which directly equates to money for the business.

#### **Important Errors Missed** ####

If there are a lot of such known errors, it is easy for new or important ones to get missed or ignored. Depending on the frequency of the known error, it can end up flooding the logs. The logs start to get overwhelming to monitor or trace for other issues with lots of such known errors. The natural tendency for people when they find something overwhelming is to ignore it. I worked on a system which had over 250 failed messages coming to the error queue daily. It was overwhelming to monitor them and was soon getting ignored. Important errors were getting missed and often ended up as support requests for the application. Such errors otherwise could have been proactively handled, giving the end user more confidence.

#### **Lower Perceived Stability** ####

The overall perceived stability of the system comes down as more and more such errors happen. It is applicable both for the users and developers. When errors no longer get monitored or tracked, critical errors gets ignored. Users have to resort to other means like support requests for the errors they face. For users who are new to the system, it might take a while to get used to the known errors. These errors decrease the trust they have in the system and soon starts suspecting everything as an issue or a problem. 

Seeing more and more of such errors does not leave a positive impact on the developers. It's possible that developers loose interest to work on an unstable system and start looking for a change. It is also a challenge when new members join the team. It takes time for them to get used to errors and exceptions and to learn to ignore them. 

#### **Stereotyping Exceptions** ####

Errors of a particular type can get stereotyped together, and get ignored mistaking it for one that is already known. It is easy for different '*object null reference exception*' error messages to be treated as a particular error whereas it could be failing for various reasons. At one of my clients, we had a specific message type failing with the null reference error. We had identified the reason for one such message and found that it is not causing '*any direct business impact*' and can be ignored. The message was failing as one of the properties on the message was alphanumeric while the code expected numeric. The simple fix in the code would be to validate it, but since this was not causing any business impact it was ignored, and messages of that type kept piling up. Until later where we found that there were other message formats of the same message type failing which was for a different reason. And those messages were causing a loss of revenue to the business. But since we were stereotyping the error messages of the particular type to the one that we found invalid and not having a business impact all of such messages were ignored. The stereotyping resulted in the important message getting ignored.

#### **Maintaining a Known Bugs Database** ####

When having a large number of such errors, it is important to document a list of such errors.It forces us to a new document and also comes with the responsibility of maintaining it. For any new developers or users joining the system, they need to go through the documentation to verify if it is a known error or not. Internalizing these errors might take some time, and critical errors can get missed during this time. Any such document needs to be kept current and up to date as and when new errors are found or more details found for older ones. This is not the best of places where a developers time is spent.

#### **Count Keeps Increasing** ####

If the count of such errors is not monitored and not valued for the probability of the number of error messages increasing is higher. New errors getting introduced will not be noticed, and even when noticed it becomes acceptable. *We already have a lot of them, so it is fine.* It sets a wrong goal for the team and can soon become unmanageable.

#### **New Business Flow Assuming Exception**s####

Since the exceptions are so used to, it is highly possible that we set that as an expectation. New business flows come up expecting a certain kind of exception to be thrown or assuming a particular type of message will not get processed. Since we are so used to the fact that it happens, we take it for granted and start coding against it. It might be the last thing that happens on a project, but believe me, it happens!. Such code becomes harder to maintain and might not work once the actual exception gets fixed. 

Ignoring exceptions and getting around to live with it can be more costly over a longer period. The further we delay action on such errors the higher the cost involved. Even though there is no immediate or direct business value seen from fixing such errors, we saw that on a longer run it could have a great impact. So try not to live with such errors but instead prioritize them with the work your team is doing and get it fixed. A fix might not always be an extra null check or a conditional to avoid the error. It might seem the easier approach to reducing the errors but will soon become a different problem. Understand the business and explore into what is causing the error. Do you have any known exceptions in the application you are working? What are you doing about it?