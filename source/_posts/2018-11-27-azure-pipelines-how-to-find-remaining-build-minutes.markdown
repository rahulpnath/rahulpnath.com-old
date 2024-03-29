---
layout: post
title: "Tip of the Week: Azure Pipelines - How to Find Remaining Free Build Minutes?"
comments: true
categories: 
- TipOW
tags: 
date: 2018-11-27
completedDate: 2018-11-27 20:40:32 +1000
keywords: 
description: 
primaryImage: azure_devops_remaining_build_minutes.png
---

With [Azure Pipelines](https://azure.microsoft.com/en-us/services/devops/pipelines/) you can continuosly build, test and deploy to any cloud platform. Azure Pipelines has multiple options to start based on your project. Even if you are developing a private application, Pipelines offers you 1 Free parallel job  with upto 1800 minutes per month  and also 1 Free self hosted with unlimited months (as it's anyway running on your infrastructure).

On the Microsoft-hosted CI/CD with 1800 minutes you might need to find the used/remaining time any time during the month. You can find the remaining minutes from the [Azure Devops portal](https://dev.azure.com/) and select the relevant organization.

**Organization settings -> Retention and parallel jobs -> Parallel Jobs**

<img src="{{site.images_root}}/azure_devops_remaining_build_minutes.png" alt="Azure Devops Pipelines - Remaining Build Minutes" 
    class ="center" />

Hope that helps you find the remaining free build minutes for your organization!
