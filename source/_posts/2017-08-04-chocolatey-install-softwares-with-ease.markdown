---
layout: post
title: "Tip of the Week: Chocolatey - Install Softwares With Ease"
comments: true
categories: 
- TipOW
- Productivity
tags: 
date: 2017-08-04
completedDate: 2017-08-03 05:01:26 +1000
keywords: 
description: The sane way to manage software on Windows.
primaryImage: chocolatey.png
---
<img src="{{site.images_root}}/chocolatey.png" alt="Chocolatey - Package Manager for Windows" class="center" >

[Chocolatey](https://chocolatey.org/) is a package manager for Windows. Chocolatey is a central store of applications, tools, and other packages. It allows you to install them on your computer from the command line. Let’s look at an example to make things more clear. Let’s say I want to install the Google Chrome Browser on my system. Below is how I would go about installing it.

> **Installing Google Chrome (Without Chocolatey)**

> 1. Bing for Google Chrome (Assuming that you are on Windows and open up Internet Explorer and search for Google Chrome)
> 2. Click the relevant link from the Search Results
> 3. Hit the Download Chrome button
> 4. Wait for the installer to download and open that to install.

> **Installing Google Chrome (With Chocolatey)**

> 1. Run '*choco install googlechrome*' from command line

Installing with Chocolatey is quick and easy. You no longer need to go around searching the web for the software that you want to install. If you are unsure of the package name (e.g. googlechrome), you can search the entire packages library. This is possible from the command line using '*choco search*.' Or you can also search on their [website](https://chocolatey.org/packages).

Run the below script from the command line to [install Chocolatey Package Manager](https://chocolatey.org/install). Once installed you can start managing applications through the command line using Chocolatey.

``` bash
@"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -ExecutionPolicy Bypass -Command "iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"
```

Other than installing and searching packages, Chocolatey also supports a lot of other commands to manage applications including un-installation. The entire commands supported are available [here](https://chocolatey.org/docs/commandslist). Instead, you can get the details in your command line using '*choco -h*.'

Hope that helps you save some time when you next want to install something.