---
layout: post
title: "Tip of the Week: Authy - Sync Two Factor Authentication Across Devices"
comments: true
categories: 
- TipOW
- Security
tags: 
date: 2018-05-21
completedDate: 2018-05-21 13:45:19 +1000
keywords: 
description: 2FA across multiple devices with cloud backups.
primaryImage: authy.jpg
---

[Two Factor Authentication](https://en.wikipedia.org/wiki/Multi-factor_authentication) (2FA) is becoming more and more common these days and is a good way to protect your accounts from getting into the wrong hands. SMS and App based 2FA are more common with the day to day services that we use, like Gmail, Outlook, Facebook etc. Enabling 2FA the user is prompted for a number that gets sent to them via phone or generated using an application, in addition to the username and password, when logging in. Enabling 2FA protects your account a level further. Even if an attacker has your credentials from a data breach, they would still need access to your phone to log in to your account. Using an app to generate the codes is more preferable than using SMS as it does not require internet connectivity or mobile service.

Until lately I have been using [Google Authenticator](https://support.google.com/accounts/answer/1066447?hl=en&ref_topic=2954345) to generate codes for all the accounts that I have 2FA enabled. The app does work well on a single mobile device but becomes a pain when you want to switch phones or lose the phone. You could potentially be locked out of your accounts if you lose the phone and don't have the backup codes available.

<img src="{{site.images_root}}/authy_preview.png" alt="Authy" class="center" />

[Authy](https://authy.com/features/) is one of the best-rated 2FA application which targets exactly the issues with Google Authenticator. It is easy to setup, can be secured via TouchId/Password, supports encrypted backups and syncs across multiple applications and devices. Once setup any code that you add to your app gets synced through Authy servers and is all encrypted and secured. Authy has applications for the [mobile, desktop and also has a plugin for Chrome browser](https://authy.com/download/). You can also manage devices from the account and revoke a device if it gets lost or is not used anymore. [Authy vs Google Authentication post](https://authy.com/blog/authy-vs-google-authenticator/) covers in detail all the differences between the two and the advantages of using Authy.

Check out [Authy](https://authy.com/features/) and do setup 2FA if you are not already!
