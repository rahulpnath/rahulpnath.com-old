---
layout: post
title: "Managing Key Vault Through Azure Portal"
comments: true
categories: 
- Azure Key Vault
tags: 
date: 2016-11-01
completedDate: 2016-10-21 06:08:18 +1100
keywords: 
description: Walkthrough Key Vault features available in Azure Portal.
primaryImage: manage_key_vault_portal.png
---

You can now manage Key Vault through the Azure portal. Prior to this the Key Vault's were managed either using [Powershell](http://www.rahulpnath.com/blog/how-the-deprecation-of-switch-azuremode-affects-azure-key-vault/), [ARM Templates](http://www.rahulpnath.com/blog/managing-azure-key-vault-using-azure-resource-manager-arm-templates/) or [REST API](http://www.rahulpnath.com/blog/managing-azure-key-vault-over-the-rest-api/). Managing key Vault is now easy and user-friendly for the nontechnical people. In this post, I will walk through the new features in Azure Portal to manage key vault.

> *If you find any difference between what you see in the portal versus those in the screenshots below, it's likely that the portal is updated*

In the new Azure portal search for 'key vault' to access this new feature. Or you can go to 'More Services' and scroll down to 'Security + Identity' section in the menu. Selecting Key Vault takes you to all the available Key Vaults under your subscription. You can further filter the Vault's list by the vault name using the filter box or based on your subscriptions.

<img  alt="Key Vault in Azure Portal" src="{{ site.images_root}}/keyvaultportal_menu_option.png"/>

#### **Creating Key Vault**
To create a new Key Vault select the 'Create' option. By entering the Vault Name, Subscription, Resource Group and Location you can create a new Key Vault. 

<img  alt="Create Key Vault in Azure Portal" src="{{ site.images_root}}/keyvaultportal_create_key_vault.png"/>

The pricing tier defaults to Standard and it does not support HSM backed keys. This means you will be only able to create Software keys. You can select the  Premium pricing tier if you need HSM backed keys. By default the login with which you create the subscription with gets granted access to the vault and is added to the access policies. You can grant additional applications or users access and control the permissions. Below I am adding in an AD application.

<img  alt="Key Vault Add Access Policy" src="{{ site.images_root}}/keyvaultportal_access_policy.png"/>

You can specify the Key Permissions and the Secret Permissions that the ServicePrincipal.

<img  alt="Access Policy set Secret Permissions" src="{{ site.images_root}}/keyvaultportal_secretpermissions.png"/>

#### **Keys and Secrets**
Once the Vault is created you can add in Keys and Secrets into the vault. This can be done by selecting the Vault that was just created from the Vaults list. From here you can manage different aspects of the Vault and also manage Keys and Secrets.

<img  alt="Access Policy set Secret Permissions" src="{{ site.images_root}}/keyvaultportal_keyvault.png"/>


The Azure Portal experience of Key Vault is good. It covers most of the functionalities needed when using a Key Vault. How do you find the new UI experience?
