---
layout: post
title: "Tip of the Week: Track Current File in Visual Studio Solution Explorer"
comments: true
categories: 
- TipOW
- Productivity
tags: 
date: 2017-03-08
completedDate: 2017-03-08 14:28:38 +1100
keywords: 
description: Keep Visual Studio Solution Explorer in sync with the current working file.
primaryImage: visualstudio_trackActiveItem.png
---

While working on large codebases, I want my Solution Explorer to be synchronized with the current working file. With the solution explorer in sync, it makes navigating to other related files, adding new classes in the same location, renaming files, etc. faster. 

<img alt="Track Active Item in Solution Explorer, Visual Studio" src="{{ site.images_root}}/visualstudio_trackActiveItem.png" />

The setting to keep the items in sync is configurable in Visual Studio and is turned off by default. You can enable this by checking the '***Track Active Item in Solution Explorer***' under *Options -> Projects and Solutions -> General*. You can navigate there quickly using [Visual Studio Quick Launch (Ctrl + Q)](http://www.rahulpnath.com/blog/quick-launch-in-visual-studio/). Just type '*Track active*' and you will get the quick link to the setting. Keep it checked, and off you go, the solution explorer and the current file will be in sync.

PS: Visual Studio 2017 is now available.[Get it](https://www.visualstudio.com/downloads/) if you have not already!