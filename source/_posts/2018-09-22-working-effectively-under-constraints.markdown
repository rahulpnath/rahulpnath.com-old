---
layout: post
title: "Working Effectively Under Constraints"
comments: true
categories:
- Thoughts 
tags: 
date: 2018-09-22
completedDate: 2018-09-22 07:19:07 +1000
keywords: 
description: 
primaryImage: working_constraints.png
---

At times you might be working in environments where there are a lot of restrictions on the tools that you can use, the process that you need to follow, etc. Under these circumstances, it is essential that we stick to some core and fundamental principles and practices that we as an industry have adopted. We need to make sure that we have that in place no matter what the restrictions imposed are. Below are a few of the restrictions that I along with my team had to face at one of the clients and what we did to keep ourselves on top of it and still deliver at a higher speed. 

<img src="{{site.images_root}}/working_constraints.png" alt="Working under Constraints." title="Working under Constraints. Image Source - https://www.saba.com/blog/how-to-master-employee-engagement-constraints">     


> *The issues discussed might or might not immediately relate to you, the important thing is your attitude towards such issues and finding ways around your constraints, keeping yourself productive on the long run.*

### No Build Deploy Pipeline

When I joined the project, it amazed that we were still building/packaging the application from a local developer system and manually deploying this to the various environments (Dev, Test, UAT, and PROD). 

> *Whenever a release was to be made one of the developers was to pause his current work, switch to the appropriate branch for the release, make sure he had the latest code base, build with the correct configuration to generate a package.*

This might sound an outdated practice (as it did to me) but here I am at a client, in the year 2018 and it's still happening. What surprised me, even more, was that the team did have access to an [Octopus](https://octopus.com/) server (backed by a [Jenkins](https://jenkins.io/) build server) but since the **deployment server did not have access to UAT/PROD servers** they chose not to use it. You bet this was the first thing I was keen on fixing, as generating a release package from my local system would be the last thing that I want to do.

After a quick chat with the team, we decided on the below.   

- **Set up build/deploy pipeline up to Test environment.** This would allow seamless integration while we are developing features and get it out for testing. Since we had access till the Test environment, this was hardly an hours work to get it all working.  

- Since we did not have access to UAT/PROD and the process required us to hand over a deployment package to the concerned team, we **set up a 'Packaging Project' in Octopus.** This project basically unzips the selected build package into our Dev environment server, applies the configuration transforms and zips up the folder into a deployment package. With this, we are now able to create a deployment package for any given build and for any environment. We are also having discussions to enable access to UAT/PROD servers for the deployment servers so that we can deploy automatically, all the way to production.

No longer was the process dependent on a developer or a developer machine and was completely automated. For those reading this and in a similar situation but who does not have access to a build/deploy system like Jenkins/Octopus I would basically set up a simple script to pull down the source given a commit hash/branch/tfs label and perform a build and package independent of the working directory of the developer. This script could run on a shared server (if you have access to one) or worst on a developer's machine/VM. The fundamental thing that we are trying to achieve is to decouple it from the current working folder on a developer machine and the manual steps involved in generating a package. As long as you have an automated way to create a package irrespective of what tools/systems you use we should be safe and sound.

### Out of Sync SQL and Code Artifacts

The application is heavily dependent on Stored Procedures for pulling/pushing data out of the SQL Server Database. Yes, you heard it right, Stored Procedures and ones with business logic in them, which is what makes it actually worse. Looking at how the Stored Procedures were maintained I could see that the team started off with good intentions using [DbUp](https://dbup.readthedocs.io/en/latest/) but soon moved away from it. When I joined the process was to share SQL artifacts as attachments in the [Jira](https://www.atlassian.com/software/jira) story/bug. The Db Administrator (DBA) would then pull that out and manage them separately in a source control repository that was not the same as the application code base. 

There was not much information on why this was the case, but the primary reason that they moved away from DbUp was that there was no visibility of the SQL scripts when running updates as it was an executable file that was the output of the DbUp project. Also, there were poor development/deployment practices that led to the ad-hoc execution of scripts in environments without actually updating the source control. This soon put the DBA out of control, and the only way to gain control back was to maintain it separately.

Again we decided to have a quick chat with the team along with the DBA on how to improve the current process, as it was getting harder to track application package versions and the associated scripts to go with that package.

- DbUp by default embeds the SQL artifacts into the executable which removes all visibility into the actual scripts. However, this behaviour is configurable using [ScriptProviders](https://dbup.readthedocs.io/en/latest/more-info/script-providers/). By using the **FileSystemScriptProvider, we can specify the folder from which to load the SQL scripts**. Configuring the msbuild to copy out all the folder files to the output and including them into the final package was an easy change. This provided the DBA with the actual SQL artifacts, and he could review them quickly. We also started a [code review](https://rahulpnath.com/blog/code-review/) process and began including the DBA for any changes related to SQL artifacts. This gave even more visibility to the DBA and helped catch issues right at the time of development.  

- With automated build/deploy till Test environment in place, we **no longer had to make ad-hoc changes** to the databases, and everything was pushed through the source control as it was faster and more comfortable.

With this few tweaks we were now in a much better state, and there was a one to one trackability between source code and SQL artifacts. It all lived as part of one package, traceable all the way to the source code commit tag auto-generated by the build system. 

### Not Invented Here Syndrome

With the kind of restrictions you have seen till now, you can guess the approach towards third party services and off-the-shelf products. Most of the things are still done in-house (including trying to replicate a service bus). The problem with this approach is that there is a limit as to which you can go doing that after which you probably lose out on your team or the code takes over you where you just cannot maintain it. When starting out on a new project and when the code base is still small building your own mechanisms might seem to work well. But once past that point, you no longer want to continue down the path but invest in industry proven tools. These include logging servers, service-bus/queues (if you need one), email services (especially if you want to track and do statistics on top of the emails send out).

Biggest challenge introducing this is mostly not cost related (as there are a lot of really affordable services for every business)it's mostly the fear of unknown and lack of interest to venture out into unfamiliar territory. The reasons might vary for you, but try to understand the core reason that is hindering the change. 

> *One technique that worked to get over the fear of the unknown was to introduce this slowly into the system, one at a time, giving enough time for people to get used to the change.*

[Seq](https://getseq.net/) was one of the first things that we had proposed and long waiting in the wish list. The team was using [Serilog](https://serilog.net/) to log, and all the logs were stored in SQL table making it really hard to query and monitor the logs. The infrastructure team did not want to install Seq as it was all new to them and were not sure about the additional task for managing a Seq instance. So we suggested to them to just have it on the development server and to get familiar with the application first. After a couple of days, the business was seeing a benefit with increased visibility into the logs and infrastructure team was also happy with the increased visibility into the logs. Even before a week, they were happy to install one for the test environment as well. At the time of writing we are looking at getting a Seq instance on the UAT server and soon to have a production instance as well. Getting the interested stakeholders to have a feel of the application and slowly introducing the change is a great way to get buy-in. 

Now we are trying to push for a service bus!

### Build Server without NuGet access

The build server we were using was in-house hosted, and the box that it ran did not have internet access. This meant that you cannot have any external package dependencies that can be pulled at build time. We chose to include the package references along with the source code, which is anyways what [I tend to prefer more](https://rahulpnath.com/blog/checking-in-package-dependencies-into-source-control/). All our third-party libraries were pushed along with the source code repository, so the build machine had all the required dependencies and did not have to have internet connectivity to make a build. 

Those are just a subset of the issues that we had to run into and bet you there were so many smaller ones. At times problems are not technical in nature, but more about communication and how effectively you are able to get all the people involved to get along with each other. 

> *Any journey to advancement is about valuing the people around you, understanding them and taking them along with the change. It's a journey that the team needs to make together and not a solo one.*

Different people have different experiences, pain points, concerns and targets to check off. So as a team you need to understand what works for everyone and come to a collective agreement. Just getting all of the concerned parties into a room and having a healthy discussion (mainly by not being prescriptive but descriptive of the issues that you are facing) solves most of the problems.

Do you work in a similar environment? What challenges do you face at work? Sound off in the comments!
