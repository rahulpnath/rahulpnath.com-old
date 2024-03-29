---
layout: post
title: "Azure Key Vault As A Connected Service in Visual Studio 2017"
comments: true
categories: 
- Azure Key Vault
tags: 
date: 2018-06-15
completedDate: 2018-06-15 16:00:51 +1000
keywords: 
description: Getting started with Key Vault is now more seamless!
primaryImage: keyVault_connectedService.png
---

Visual Studio (VS) now supports adding [Azure Key Vault](https://rahulpnath.com/blog/category/azure-key-vault/) as a [Connected Service](https://docs.microsoft.com/en-us/azure/key-vault/vs-key-vault-add-connected-service), for Web Projects ( ASP.NET Core or any ASP.NET project). Enabling this from the Connected Service makes it easier for you to get started with Azure Key Vault. Below are the prerequisites to use the Connected Service feature

> [Prerequisites](https://docs.microsoft.com/en-us/azure/key-vault/vs-key-vault-add-connected-service#prerequisites)
>
> - An Azure subscription. If you do not have one, you can sign up for a free account.
> - Visual Studio 2017 version 15.7 with the Web Development workload installed. Download it now.
> - An ASP.NET 4.7.1 or ASP.NET Core 2.0 web project open.

<img class =" center" src="{{site.images_root}}/keyVault_connectedService.png" alt="Visual Studio, Azure Key Vault Connected Services" />

When selecting 'Secure Secrets with Azure Key Vault' option from the list of Connected Services provided it takes you to a new page within Visual Studio with your Azure Subscription associated with Visual Studio Account and gives you the ability to add a Key Vault to it. VS does generate some defaults for the Vault Name, Resource Group, Location and the Pricing Tier which you can edit as per your requirement. Once you confirm to the _Add_ the Key Vault, VS provisions the Key Vault with the selected configuration and modifies some things in your project.

- [Changes Made to your ASP.NET project](https://docs.microsoft.com/en-us/azure/key-vault/vs-key-vault-aspnet-what-happened)
- [Changes made to your ASP.NET Core project](https://docs.microsoft.com/en-us/azure/key-vault/vs-key-vault-aspnet-core-what-happened)

<img class =" center" src="{{site.images_root}}/keyVault_connectedService_createKeyVault.png" alt="Visual Studio, Azure Key Vault Connected Services" />

In short, VS adds

- a bunch of NuGet packages to access Azure Key Vault
- Adds in the Keyvault Url details
- In ASP.NET Web project VS modifies the configuration file to add in the AzureKeyVaultConfigBuilder as shown below.

```xml Web.config
<configuration>
<configSections>
<section
      name="configBuilders"
      type="System.Configuration.ConfigurationBuildersSection, System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
      restartOnExternalChanges="false"
      requirePermission="false" />
</configSections>
<configBuilders>
<builders>
<add
      name="AzureKeyVault"
      vaultName="webapplication-47-dev-kv"
      type="Microsoft.Configuration.ConfigurationBuilders.AzureKeyVaultConfigBuilder, Microsoft.Configuration.ConfigurationBuilders.Azure, Version=1.0.0.0, Culture=neutral"
      vaultUri="https://WebApplication-47-dev-kv.vault.azure.net" />
</builders>
</configBuilders>
```

To start using Azure Key Vault from your application we first need to add some Secrets to the Key Vault created by Visual Studio. You can add a secret to the portal using multiple ways, the most straightforward being [using the Azure Portal](https://rahulpnath.com/blog/managing-key-vault-through-azure-portal/). Once you add the Secret to the Key Vault, update the configuration file with the Secret names. Below is how you would do for an ASP.NET Web Project. (_MySecret_ and _VersionedSecret_ keys)

<div class="alert alert-warning">
Make sure to add <b>configBuilders="AzureKeyVault"</b> to the appSettings tag. This tells the Configuraion Manager to use the configured AzureKeyVaultConfigBuilder
</div>

```xml
<appSettings configBuilders="AzureKeyVault">
      <add key="webpages:Version" value="3.0.0.0" />
      <add key="webpages:Enabled" value="false" />
      <add key="ClientValidationEnabled" value="true" />
      <add key="UnobtrusiveJavaScriptEnabled" value="true" />
      <add key="MySecret" value="dummy1"/>
      <add key="VersionedSecret" value="dummy2"/>
</appSettings>
```

The values _dummy\*_ are just dummy values and will be overridden at runtime from the Secret Values created in the Key Vault. If the Secret with the corresponding name does not exist in Key Vault, then the _dummy_ values will be used.

### Authentication

When VS creates the Vault, it adds in the user logged into VS to the Access Policies list. When running the application, the AzureKeyVaultConfigBuilder uses the same details to authenticate with the Key Vault.

> If you are not logged in as the same user or not logged in at all the provider will not be able to authenticate with the Key Vault and will fallback to use the dummy values in the configuration file. Alternatively you could specify [connection option avaiable for AzureServiceTokenProvider](https://docs.microsoft.com/en-us/azure/key-vault/service-to-service-authentication#connection-string-support)

<img class =" center" src="{{site.images_root}}/keyVault_connectedService_AccessPolicies.png" alt="Visual Studio, Azure Key Vault Connected Services" />

### Secrets and Versioning

The [AzureKeyVaultConfigBuilder](https://github.com/aspnet/MicrosoftConfigurationBuilders/tree/master/src/Azure) requests to get all the Secrets in the Key Vault at application startup using the [Secrets endoint](https://docs.microsoft.com/en-us/rest/api/keyvault/getsecrets/getsecrets). This call returns all the Secrets in the Key Vault. For whatever keys in the AppSettings that has a match with a Secret in the vault, a request is made to get the [Secret details](https://docs.microsoft.com/en-us/rest/api/keyvault/getsecret/getsecret), which returns the actual Secret value for the keys. Below are the traces of the calls going out captured using [Fiddler](https://rahulpnath.com/blog/fiddler-free-web-debugging-proxy/).

<img class =" center" src="{{site.images_root}}/keyVault_connectedService_requests.png" alt="AzureKeyVaultConfigBuilder Fiddler Traces " />

It looks like at the moment the AzureKeyVaultConfigBuilder get only the latest version of the Secrets. As you can tell from one of my Secret names (VersionedSecret), I have created two versions for the Secret, and the config builder picks the latest version. I don't see a way right now whereby I can specify a specific secret version.

The Visual Studio Connected Services makes it easy to get started with Azure Key Vault and move your secrets to a more secure store, than having it around in your [configuration files](https://rahulpnath.com/blog/keeping-sensitive-configuration-data-out-of-source-control/).
