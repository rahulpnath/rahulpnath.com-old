---
layout: post
title: "Tip of the Week: Visual Studio Task List - Keep Track of Your TODO Comments"
comments: true
categories: 
- TipOW
- Productivity
tags: 
date: 2017-05-03
completedDate: 2017-05-03 13:14:34 +1000
keywords: 
description: Track your unfinished work in one place.
primaryImage: taskList.png
---

It often happens when coding that I skip over some part and want to come back to it at a later point in time. I leave some comments in the code so that I do not miss it. It can be a bit tricky to keep track of these comments themselves. Before pushing up the changes to [master branch](https://git-scm.com/book/en/v2/Git-Branching-Branches-in-a-Nutshell) or creating a [Pull Request](https://help.github.com/articles/about-pull-requests/), I make sure that all such comments are addressed. 

Visual Studio comes with a [Task List](https://msdn.microsoft.com/en-us/library/txtwdysk(v=vs.120\).aspx) that is handy to track such unfinished work in code. It helps track your pending work items in one place and easily navigate to it. To have a comment appear in the task list, it has to start with a defined token (TODO, HACK, UNDONE, etc.) followed by the comment.

``` csharp
public bool IsInRange(DateTime theDateTime)
{
    //TODO: Implement this function
    throw new NotImplementedException();
}
```

<img src="{{site.images_root}}/taskList.png" class="center" alt="Visual Studio Task List" />

> *A comment in your code preceded by a comment marker and a predefined token will appear in the Task List window. For example, the above comment has three distinct parts:*    

> - *The comment marker (//)*     
> - *The token (TODO)*    
> - *The comment (the rest of the text)*    

Visual Studio by default has TODO, HACK and UNDONE as tokens. You can modify this under Options -> Task List. New custom tokens can be added as required and used instead of the default ones. 

<img src="{{site.images_root}}/taskList_customize.png" class="center" alt="Visual Studio, Customize Task List" />

When in a multi-member team you can either use custom tokens per member or append the comment with your name or feature name. The Task List provides Search feature with which you can filter the tasks created by you or for the feature you are working.

I try to remove all TODO comments before merging to the master branch. For tasks that need to be tracked even after a merge, I create separate work items to the project backlog (VSTS, GitHub Tasks, Jira whatever the team is using). I might still leave the TODO comment with the relevant ticket details as well for tracking.

The next time you leave some unfinished work for later make sure you have it tracked. Hope it helps!