---
layout: post
title: "Exploring AzureKeyVaultConfigBuilder"
comments: true
categories: 
- Azure Key Vault
tags: 
date: 2018-06-22
completedDate: 2018-06-22 19:10:41 +1000
keywords: 
description: Various usage scenarios of Azure Key Vault as a Visual Studio Connected Service
primaryImage: AzureKeyVaultConfigBuilder_config.png
---

Over the last weekend, I was playing around with [Visual Studio Connected Services support for Azure Key Vault](https://rahulpnath.com/blog/azure-key-vault-as-a-connected-service-in-visual-studio-2017/). The new feature allows seamless integration of ASP.NET Web applications with Azure Key Vault, making it as simple as using the ConfigurationManager to retrieve the Secrets from the Key Vault - just like you would retrieve it from the config file.

<img src="{{site.images_root}}/AzureKeyVaultConfigBuilder_config.png" class="center" >

In this post, we will look detailed into the [AzureKeyVaultConfigBuilder](https://github.com/aspnet/MicrosoftConfigurationBuilders/tree/master/src/Azure) class that allows the seamless integration provided by Connected Services. As we saw in the previous post when you add [Key Vault as a Connected Service](https://rahulpnath.com/blog/azure-key-vault-as-a-connected-service-in-visual-studio-2017/), it modifies the applications configuration file to add in the AzureKeyVaultConfigBuilder references.

<div class="alert alert-warning">
Make sure to update the <b><i>Microsoft.Configuration.ConfigurationBuilders.Azure</i></b> and <b><i>Microsoft.Configuration.ConfigurationBuilders.Base</i></b> Nuget packages to the latest version.
</div>

### Loading Connection String and App Settings

The AzureKeyVaultConfigBuilder can be specified on both [appsettings](https://msdn.microsoft.com/en-us/library/ms228154.aspx) and [connectionString](<https://msdn.microsoft.com/en-us/library/bf7sd233(v=vs.100).aspx>) element using the _configBuilders_ attribute.

```xml Configuration File
 <appSettings configBuilders="AzureKeyVault">
 ...
 </appSettings>
 <connectionStrings configBuilders="AzureKeyVault">
 ...
 </connectionStrings>
```

### Accessing Multiple Key Vaults

The configBuilders element supports comma-separated list of builders. Using this feature, we can specify multiple Vaults as a source for our secrets. Note how we pass in _'keyVault1,keyVault2'_ to configBuilders option below.

```xml Configuration File
<configBuilders>
    <builders>
      <add
        name="keyVault1"
        vaultName="keyVault1"
        type="Microsoft.Configuration.ConfigurationBuilders.AzureKeyVaultConfigBuilder, Microsoft.Configuration.ConfigurationBuilders.Azure, Version=1.0.0.0, Culture=neutral" />

      <add
        name="keyVault2"
        vaultName="keyVault2"
        type="Microsoft.Configuration.ConfigurationBuilders.AzureKeyVaultConfigBuilder, Microsoft.Configuration.ConfigurationBuilders.Azure, Version=1.0.0.0, Culture=neutral" />
    </builders>
  </configBuilders>
  <appSettings configBuilders="keyVault1,keyVault2">
  ...
  </appSettings>
```

If the same key has a value in multiple sources, then the value from the last builder in the list takes precedence. (But I assume you would not need that feature!)

### Modes

All config builders have the options of setting a [mode](https://github.com/aspnet/MicrosoftConfigurationBuilders#mode), which allows three options.

> - _Strict_ - This is the default. In this mode, the config builder will only operate on well-known key/value-centric configuration sections. It will enumerate each key in the section, and if a matching key is found in the external source, it will replace the value in the resulting config section with the value from the external source.

> - _Greedy_ - This mode is closely related to Strict mode, but instead of being limited to keys that already exist in the original configuration, the config builders will dump all key/value pairs from the external source into the resulting config section.

> - _Expand_ - This last mode operates on the raw XML before it gets parsed into a config section object. It can be thought of as a simple expansion of tokens in a string. Any part of the raw XML string that matches the pattern ${token} is a candidate for token expansion. If no corresponding value is found in the external source, then the token is left alone.

In short when set to _Strict_ it matches the names in configuration file to Secrets in the Vault's configured. If it does not find corresponding Secret it ignores that key. When set to _Greedy_, irrespective of what keys are there in the configuration file, it makes all the secrets available in the Vaults specified via Configuration. **This to me sounds like magic and would not prefer to do in an application that I build.**

### Greedy Mode Filtering and Formatting Secrets

When using Greedy mode, we can filter on the list of keys that are made available by using the [prefix](https://github.com/aspnet/MicrosoftConfigurationBuilders#prefix) option. Only Secret Names starting with the prefix is made available in the configuration. The other secrets are ignored. This feature can be used in conjunction with [stripPrefix](https://github.com/aspnet/MicrosoftConfigurationBuilders#stripprefix) option. When stripPrefix is set to true (defaults to false), the Secret is made available in the configuration after stripping off the prefix.

For e.g. if we have a Secret with the name _connectionString-MyConnection_, having the below configuration will add the connection string with name _MyConnection_.

```xml Configuration File
<add
  name="keyVault1"
  vaultName="keyVault1"
  prefix="connectionString-"
  stripPrefix="true"
  type="Microsoft.Configuration.ConfigurationBuilders.AzureKeyVaultConfigBuilder, Microsoft.Configuration.ConfigurationBuilders.Azure, Version=1.0.0.0, Culture=neutral" />
```

```csharp
var connectionString = ConfigurationManager.ConnectionStrings["MyConnection"];
```

_Use prefix and stripPrefix in conjunction with the Greedy mode. For keys mentioned in the config it will try to match it with the prefix appended to the key name_

### Preloading Secrets

By default, the Key Vault Config Builder is set to preload the available Secrets in key vault. By doing this the config builder knows the list of configuration values that the key vault can resolve. For preloading the Secrets, the config builder uses the _List_ call on Secrets. If you don't have list access on Secrets you can turn this feature off using the _preloadSecretNames_ configuration option. At the time of writing the config builder version (1.0.1) throws an exception when preloading Secrets in turned on and List policy is not available on the Vault. I have raised a [PR to fix](https://github.com/aspnet/MicrosoftConfigurationBuilders/pull/24) this issue, which if accepted would no longer throw the exception and would invalidate this configuration option.

```xml Configuration File
<builders>
    <add
    name="keyVault1"
    preloadSecretNames="false"
    vaultName="keyVault1"
```

### Authentication Modes

The connectionString attribute allows you to specify the authentication mechanism with Key Vault. By default when using the Connected Service to create the key vault it adds the Visual Studio user to the access policies of the Key Vault. When connecting it uses the same. But this does not help you in a large team scenario. Most likely the Vault will be created under your organization subscription and you might want to share the same vault between all developers in the team. You could add the users individually and give them the appropriate access policies, but this might soon become cumbersome for a large team. Instead of using the [Client Id/Secret or Certificate](https://rahulpnath.com/blog/authenticating-a-client-application-with-azure-key-vault/) authentication along with [Managed Service Identity](https://rahulpnath.com/blog/authenticating-with-azure-key-vault-using-managed-service-identity/) configuration for localhost works the best. The configuration provider will then use the _AzureServicesAuthConnectionString_ value from environment variable to connect to the key vault.

```text Local System
Set AzureServicesAuthConnectionString Environment variable
RunAs=App;AppId=AppId;TenantId=TenantId;AppKey=Secret.
Or
RunAs=App;AppId=AppId;TenantId=TenantId;CertificateThumbprint=Thumbprint;CertificateStoreLocation=CurrentUser
```

As you can see the AzureKeyVaultConfigBuilder does provide good integration with Key Vault and makes using the Key Vault seamless. It does have a few issues, especially around [handling different Secret versions](https://github.com/aspnet/MicrosoftConfigurationBuilders/issues/20), which might be fixed in future releases.

_PS: At the time of writing there were a few issues that I had found while playing around. You can follow up on the individual issues on [Github](https://github.com/aspnet/MicrosoftConfigurationBuilders/issues?utf8=%E2%9C%93&q=is%3Aissue+author%3Arahulpnath+). Fingers crossed hope at least one of my PR's makes its way through to master!_
