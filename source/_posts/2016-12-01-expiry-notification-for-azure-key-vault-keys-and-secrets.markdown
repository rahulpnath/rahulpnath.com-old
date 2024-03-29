---
layout: post
title: "Expiry Notification for Azure Key Vault Keys and Secrets"
comments: true
categories:
- Azure Key Vault 
tags: 
date: 2016-12-01
completedDate: 2016-11-13 05:31:25 +1100
keywords: 
description: Explores into setting up a daily task to trigger keys/secrets in Key Vault that are nearing expiry.
primaryImage: vaultexpiry_key.png
---

I came across this [question](https://social.msdn.microsoft.com/Forums/azure/en-US/90d4b814-f025-42a0-acac-b8c8bf9d8cf8/alert-or-event-on-secret-expiry?forum=AzureKeyVault) in Azure Key Vault forums looking for options to get notified when Key or Secrets in vault nears expiry. It's useful to know when Object's (Keys/Secrets) near expiry, to take necessary action. I decided to explore on my proposed solution of having a scheduled custom PowerShell script to notify when a key is about to expire. In this post, we will see how to get all objects nearing expiry and scheduling this using Azure Runbook to run daily.

### Getting Expiring Objects

Both Keys and Secrets can be set with an Expiry date. The expiry date can be set when creating the Object or can be set on an existing Object. This can be set from the UI or using PowerShell scripts (setting the [-Expires attribute](https://msdn.microsoft.com/en-us/library/dn868045.aspx)).

<img class="center" alt="Azure Key Vault - Set Key Expiry" src="{{ site.images_root}}/vaultexpiry_key.png"/>

Key Vault (at the time of writing) throws an [exception](https://social.msdn.microsoft.com/Forums/azure/en-US/c0d8953a-c117-4ca4-ad3d-e5d2b1868f9e/get-operation-not-permitted-for-some-of-the-secret-in-my-vault?forum=AzureKeyVault) when an expired key is accessed over the API. Also, it does not provide any notification whenever a key/secret is about to expire. The last thing you want is your application go down because of an expired object in the vault. With the Get and List access on the vault, we can retrieve all the keys and secrets in the vault and loop through the elements to see objects that are nearing expiry.

The PowerShell script takes the Vault Name, number of days before with alert should be raised and flags to indicate whether all versions of keys/secrets should be checked for expiry. The full script is available [here](https://github.com/rahulpnath/Blog/blob/master/KeyVaultExpiryAlerter/Expiry%20Alert.ps1).

``` powershell
$VaultName = ''
$IncludeAllKeyVersions = $true
$IncludeAllSecretVersions = $true
$AlertBeforeDays = 3
```

All keys and secrets are converted into a common object model, which contains just the Identifier, Name, Version and the Expiry Date if it has one.

``` powershell
Function New-KeyVaultObject
{
    param
    (
        [string]$Id,
        [string]$Name,
        [string]$Version,
        [System.Nullable[DateTime]]$Expires
    )

    $server = New-Object -TypeName PSObject
    $server | Add-Member -MemberType NoteProperty -Name Id -Value $Id
    $server | Add-Member -MemberType NoteProperty -Name Name -Value $Name
    $server | Add-Member -MemberType NoteProperty -Name Version -Value $Version
    $server | Add-Member -MemberType NoteProperty -Name Expires -Value $Expires
    
    return $server
}
```

Depending on the flag set for retrieving all key/secret version, it fetches objects from the vault and returns in the common object model above.

``` powershell
function Get-AzureKeyVaultObjectKeys
{
  param
  (
   [string]$VaultName,
   [bool]$IncludeAllVersions
  )

  $vaultObjects = [System.Collections.ArrayList]@()
  $allKeys = Get-AzureKeyVaultKey -VaultName $VaultName
  foreach ($key in $allKeys) {
    if($IncludeAllVersions){
     $allSecretVersion = Get-AzureKeyVaultKey -VaultName $VaultName -IncludeVersions -Name $key.Name
     foreach($key in $allSecretVersion){
         $vaultObject = New-KeyVaultObject -Id $key.Id -Name $key.Name -Version $key.Version -Expires $key.Expires
         $vaultObjects.Add($vaultObject)
     }
     
    } else {
      $vaultObject = New-KeyVaultObject -Id $key.Id -Name $key.Name -Version $key.Version -Expires $key.Expires
      $vaultObjects.Add($vaultObject)
    }
  }
  
  return $vaultObjects
}

function Get-AzureKeyVaultObjectSecrets
{
  param
  (
   [string]$VaultName,
   [bool]$IncludeAllVersions
  )

  $vaultObjects = [System.Collections.ArrayList]@()
  $allSecrets = Get-AzureKeyVaultSecret -VaultName $VaultName
  foreach ($secret in $allSecrets) {
    if($IncludeAllVersions){
     $allSecretVersion = Get-AzureKeyVaultSecret -VaultName $VaultName -IncludeVersions -Name $secret.Name
     foreach($secret in $allSecretVersion){
         $vaultObject = New-KeyVaultObject -Id $secret.Id -Name $secret.Name -Version $secret.Version -Expires $secret.Expires
         $vaultObjects.Add($vaultObject)
     }
     
    } else {
      $vaultObject = New-KeyVaultObject -Id $secret.Id -Name $secret.Name -Version $secret.Version -Expires $secret.Expires
      $vaultObjects.Add($vaultObject)
    }
  }
  
  return $vaultObjects
}
```

Now that we have all the keys and secrets we want to check for expiry all we need to know is if there are any keys that are expiring in the upcoming days.

``` powershell
$allKeyVaultObjects = [System.Collections.ArrayList]@()
$allKeyVaultObjects.AddRange((Get-AzureKeyVaultObjectKeys -VaultName $VaultName -IncludeAllVersions $IncludeAllKeyVersions))
$allKeyVaultObjects.AddRange((Get-AzureKeyVaultObjectSecrets -VaultName $VaultName -IncludeAllVersions $IncludeAllSecretVersions))

# Get expired Objects
$today = (Get-Date).Date
$expiredKeyVaultObjects = [System.Collections.ArrayList]@()
foreach($vaultObject in $allKeyVaultObjects){
if($vaultObject.Expires -and $vaultObject.Expires.AddDays(-$AlertBeforeDays).Date -lt $today)
{
  # add to expiry list
  $expiredKeyVaultObjects.Add($vaultObject) | Out-Null
  Write-Output "Expiring" $vaultObject.Id
  
}

# Pass to Alerter $expiredKeyVaultObjects
}
```

### Scheduling Expiry Notification using Azure Runbook

You can either run this manually every time you want to get a list of objects that are expired or nearing expiry. Alternatively, you can set up a scheduled task to run the script at a set frequency. Since you are already on Azure, you can try [Azure Automation](https://azure.microsoft.com/en-us/services/automation/) and schedule the task for you. A Runbook in Azure Automation account can be granted access to the key vault. The Automation account should have 'Run as account' setup and the service principal created for it can be used to assign Access Policies to access the vault. Check out the [Accessing Azure Key Vault From Azure Runbook](/blog/accessing-azure-key-vault-from-azure-runbook) post for a step by step instruction on how to setup runbook to access key vault. You can then [schedule the runbook](https://azure.microsoft.com/en-us/documentation/articles/automation-scheduling-a-runbook/) to execute at fixed time intervals. Feel free to modify the script to send email notifications or push notification or any other that matches your need.

Generally, it is a good practice to rotate your keys and secrets once in a while. Use the expiry attribute to set the expiry and force yourself to keep your sensitive configurations fresh. It's likely that such a notification feature will be built into the key vault. But till then Hope this helps to keep track of keys or secrets that are nearing expiry within the vault and take the necessary action to renew them. The full script is available [here](https://github.com/rahulpnath/Blog/blob/master/KeyVaultExpiryAlerter/Expiry%20Alert.ps1).