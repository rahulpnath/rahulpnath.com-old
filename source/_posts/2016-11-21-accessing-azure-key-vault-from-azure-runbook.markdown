---
layout: post
title: "Accessing Azure Key Vault From Azure Runbook"
comments: true
categories: 
- Azure Key Vault 
tags: 
date: 2016-11-21
completedDate: 2016-11-12 05:10:27 +1100
keywords: 
description: A quick walk-through on using Key Vault from Azure Automation Runbook.
primaryImage: runbook_create.png
---

Azure Automation is a new service in Azure that allows you to automate your Azure management tasks and to orchestrate actions across external systems from right within Azure. If you are new to Azure Automation, get started [here](https://azure.microsoft.com/en-us/blog/azure-automation-runbook-management/). Runbooks live within the Azure Automation account and can execute PowerShell scripts. In this post, I will walk you through on how to use Key Vault from an Azure Automation Runbook. 

To create a Runbook go to 'Add a Runbook' under  Automation Account, Runbooks as shown in the image below. Once created you can author your PowerShell script there.  

<img class="center" alt="Azure Automation Create a Runbook" src="{{ site.images_root}}/runbook_create.png"/>

In this example I will get all the Keys from an existing key vault, using the [Get-AzureKeyVaultKey](https://msdn.microsoft.com/en-us/library/dn868053.aspx) cmdlet. This returns all the key for the given key vault.

``` powershell
Get-AzureKeyVaultKey -VaultName YoutubeVault
```

If we are to run this script now, it will fail for many reasons - we do not have the key vault cmdlets imported in the runbook nor have we given access to the automation account to access the keys in the vault. Running it gives me the below error.

<span style='color:red'>*Get-AzureKeyVaultKey : The term 'Get-AzureKeyVaultKey' is not recognized as the name of a cmdlet, function script, file, or operable program. Check the spelling of the name, or if a path was included, verify that the path is correct and try again *</span>. 

Under 'Assets' from the Azure Automation account Resources section select 'Modules' (as shown in the image below) to add in Modules to the runbook. To execute key vault cmdlets in the runbook, we need to add *AzureRM.profile* and AzureRM.key vault. Search for this under 'Browse Gallery' and import. 

<img class="center" alt="Azure Runbook Add KeyVault Module" src="{{ site.images_root}}/runbook_add_Module.png"/>

To give Runbook access to the keys in the vault, it needs to be specified in the Access Policies of the key vault. The 'Run As Accounts' feature will create a new service principal user in Azure Active Directory and assign the Contributor role to this user at the subscription. The 'Application ID' from creating the run as account is used to assign Access Policies for the key vault. In this example, I give the 'list' and 'get' PermissionToKeys.

<img class="center" alt="Azure Automation Runbook, set run as account" src="{{ site.images_root}}/runbook_run_as_accounts.png"/>

You can use the [sample code below](https://azure.microsoft.com/en-us/documentation/articles/automation-sec-configure-azure-runas-account/#sample-code-to-authenticate-with-resource-manager-resources), taken from the AzureAutomationTutorialScript example runbook, to authenticate using the Run As account to manage Resource Manager resources with your runbooks. The *AzureRunAsConnection* is a connection asset automatically created when we created 'run as accounts' above. This can be found under Assets -> Connections. After the authentication code, I run the same code above to get all the keys from the vault.

``` powershell
$connectionName = "AzureRunAsConnection"
try
{
    # Get the connection "AzureRunAsConnection "
    $servicePrincipalConnection=Get-AutomationConnection -Name $connectionName         

    "Logging in to Azure..."
    Add-AzureRmAccount `
        -ServicePrincipal `
        -TenantId $servicePrincipalConnection.TenantId `
        -ApplicationId $servicePrincipalConnection.ApplicationId `
        -CertificateThumbprint $servicePrincipalConnection.CertificateThumbprint 
}
catch {
    if (!$servicePrincipalConnection)
    {
        $ErrorMessage = "Connection $connectionName not found."
        throw $ErrorMessage
    } else{
        Write-Error -Message $_.Exception
        throw $_.Exception
    }
}

Get-AzureKeyVaultKey -VaultName YoutubeVault
```

On execution, the Runbook can connect to the key vault and retrieve all the keys. Based on the permissions set on the vault you can perform different actions on the key vault. This helps in automating a lot of tasks that otherwise needs to be done manually. Hope this helps you connect Runbooks with Key Vault.