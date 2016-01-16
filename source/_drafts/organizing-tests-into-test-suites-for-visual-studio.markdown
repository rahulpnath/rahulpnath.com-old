---
layout: post
title: "Organizing Tests into Test Suites for Visual Studio"
comments: true
categories:
- Testing
- Productivity 
tags: 
thisIsStillADraft:
keywords: 
description: 
---

While working with large code base, that has a lot of tests (unit, integration, acceptance etc), running all of them every time we make a small  change (if you are doing TDD or just using build for feedback) takes a lot of time. Organizing tests into different test suites makes it easier to run them as required by the current context is handy in such cases. 

There are multiple ways that we can do this within Visual Studio and below are some of the options available. I tend to use a mix of all these in my current project. This gives the flexibility to run only the new tests that I am writing while actively developing a new code or set of related tests for the updates that I am making. Once done with the changes, I can run the full suite of unit tests, followed by the integration tests. This reduces the interruption duration while coding and has a direct impact on the overall productivity too.

<img class="center" alt="Geek productivity" src="{{ site.images_root}}/geek_productivity.jpg" />

#### Test Traits ####

Traits are a good way to group tests together and to run them as different suites. It encompasses TestCategory, TestProperty, Priority and Owner. Using [TestCategory](https://msdn.microsoft.com/en-au/library/microsoft.visualstudio.testtools.unittesting.testcategoryattribute.aspx) attribute we can specify  the group of the test. The Visual Studio Test Explorer uses this value to group the tests and allows executing tests in specific groups.

You can specify the TestCategory attribute and specify a group name or inherit from the TestCategory and have types indicating groups.
** Image showing the grouping ** 
Xunit traits and overriding them

Limitation with the above approach is that it depends on developers to put these attributes on the test cases or class level and not leveraging any existing conventions that might be already in place. Having integration tests, unit tests, acceptance tests in different VS projects is a very common practice with conventions like project names ending with '.UnitTests, .IntegrationTests, .AcceptanceTests' etc. 


**Build Tasks and Task Runner Explorer**
The Task Runner Explorer (TRE)
convention based tests
can organize js and c# ts tests
build scripts using grunt/gulp

**test settings file**
quickly select a subset of tests 
no project level groupings
short lived groupings
