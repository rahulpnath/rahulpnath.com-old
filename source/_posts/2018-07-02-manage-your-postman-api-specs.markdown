---
layout: post
title: "Managing Your Postman API Specs"
comments: true
categories: 
- Testing
- Programming
tags: 
date: 2018-07-02
completedDate: 2018-06-27 11:23:09 +1000
keywords: 
description: Organizing and managing your API specs either through Postman Cloud or your Source Control.
primaryImage: postman_published.png
---

In the previous post, we explored how to use [Postman for testing API endpoints](/blog/automated-api-testing-using-postman-collection-runner/). Postman is an excellent tool to manage API specs as well, so that you can try API requests individually to see how things are working. It also acts as documentation for all your API endpoints and serves as a good starting point for someone new to the team. When it comes to managing the API specs for your application, there are a few options that we have and let's explore what they are.

### Organizing API Specs

Postman supports the concept of [Collections](https://www.getpostman.com/docs/v6/postman/collections/creating_collections), which are nothing but a Folder to group of saved API requests/Specs. Collections support nesting which means you can add Folders within a collection to further group them. As you can see below the _MyApplication_ and _Postman Echo_ are collections, and there are subfolders inside them which in turn contains API requests. The multi-level hierarchy helps you to organize your requests the way you want to.

<img src="{{site.images_root}}/postman_collections.png" alt="Postman Collections" class ="center">

### Sharing API Specs

Any Collection that you create in Postman is automatically synced to Postman Cloud if you are logged in with an account. It allows you to share collections through a link. With [paid version of Postman](https://www.getpostman.com/pricing) you get to create [team workspaces](https://www.getpostman.com/workspaces), which means a team can collaborate on the shared versions. It allows easy sharing of specs across your team and manages them in a centralized place.

However, if you are not logged in or don't have a paid version of Postman, you can maintain the specs along with your Source Code. Postman allows you to [export Collections and share specs as a JSON file](https://www.getpostman.com/docs/v6/postman/collections/sharing_collections#sharing-as-a-file). You can then check this file into your source code repository. Other team members can Import the exported file to get the latest specs. The only disadvantage with this is that you need to make sure to export/import every time you/other team members make a change to the JSON file. However, I have seen this approach work well in teams and one way we made sure that the JSON file was up to date is to have to update the API spec as a Work Item and which required to be [peer reviewed](https://rahulpnath.com/blog/code-review/)(through Pull Requests)

### Managing Environments

Typically any application/API would be deployed to multiple environments (like localhost, Development, Testing, Production, etc.) and you would want to switch between these environments to test your API endpoints seamlessly. Postman makes this easy by using the [Environment Feature](https://www.getpostman.com/docs/v6/postman/environments_and_globals/manage_environments).

<img src="{{site.images_root}}/postman_environment.png" alt="Postman Environment" class="center" />

Again as with Collections, Environments are also synced to Postman Cloud when you are logged in. It makes all your environments available to all your team seamlessly. However, if you are not logged in you can again export the environments as a JSON file and then share that out of band (in a secure manner as this might have sensitive information like tokens, keys, etc.) with your team.

### Publishing API Specs

Postman allows you to [publish API specs](https://www.getpostman.com/docs/v6/postman/api_documentation/publishing_public_docs) (even to a custom URL), which can act like your API Documentation. You can publish it per environments and also easily execute them. Publishing is available only if you log in to an account as it requires the API Specs and environment details in the first place.

<img src="{{site.images_root}}/postman_published.png" alt="Postman Published" class="center" />

### Security Considerations

When using the sync feature of Postman (logged in to the application with Postman account), it is [recommended](https://www.getpostman.com/docs/v6/postman_for_publishers/run_button/security) that you do not have any sensitive information (like passwords/tokens) as part of the API request spec/Collection. These should be extracted out as Environment variables and stored as part of the appropriate environment.

If you are logged in, all the data that you add to it is automatically synced, which means it will be living in Postman's cloud server. This might not be a desirable option for every company but looks like there is no option to turn sync off at the Collection level. The only way to not sync collections is to not log into an account in Postman.

<div class="alert alert-warning">
    <i>
    If you are logged into Postman then any collection that you create is automatically synced to Postman server. Only way to <a href="https://support.getpostman.com/hc/en-us/articles/203492852-How-do-I-disable-Sync-">prevent sync</a> is not to log in
    </i>
</div>

We have seen the options by which you can share API collections and environments amongst your team even if you are logged in. However, one thing to be aware of is if any of your team members are logged into Postman and imports a collection shared via Repository/out of band methods, it will be synced to Postman server. So at the organization/team level, you would need ways to prevent this from happening if it is essential for you. Best is to have your API's designed in such a way that you do not have to expose such sensitive information, which anyways is a better practice.

Hope this allows to manage your API specs better!
