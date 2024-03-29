---
layout: post
title: "Exclude Certain Scripts From Transaction When Using DbUp"
comments: true
categories: 
- Programming
tags: 
date: 2018-12-17
completedDate: 2018-12-15 05:34:26 +1000
keywords: 
description: Certain commands cannot run under a transaction. See how you can exclude them while still keeping your rest of the scripts under transaction.
primaryImage: dbup_batches.png
---

Recently I had written about [Setting Up DbUP in Azure Pipelines](https://rahulpnath.com/blog/setting-up-dbup-in-azure-pipelines/) at one of my clients. We had all our scripts run under [Transaction Per Script](https://dbup.readthedocs.io/en/latest/more-info/transactions/) mode and was all working fine until we had to deploy some SQL scripts that cannot be run under a transaction. So now I have a bunch of SQL script files that can be run under a transaction and some (like the ones below - [Full-Text Search](https://azure.microsoft.com/en-au/blog/full-text-search-is-now-available-for-preview-in-azure-sql-database/)) that cannot be run under a transaction. By default, if you run this using DbUp under a transaction you get the error message<span class="text-danger">
CREATE FULLTEXT CATALOG statement cannot be used inside a user transaction </span> and this is an [existing issue](https://github.com/DbUp/DbUp/issues/207).


``` sql Full Text Search Script
CREATE FULLTEXT CATALOG MyCatalog
GO

CREATE FULLTEXT INDEX 
ON  [dbo].[Products] ([Description])
KEY INDEX [PK_Products] ON MyCatalog
WITH CHANGE_TRACKING AUTO
GO
```

One option would be to turn off transaction all together using *builder.WithoutTransaction()* (default transaction setting) and everything would work as usual. But in case you want each of your scripts to be run under a transaction you can choose either of the options below.

### Using Pre-Processors to Modify Script Before Execution

[Script Pre-Processors](https://dbup.readthedocs.io/en/latest/more-info/preprocessors/) are an extensibility hook into DbUp and allows you to modify a script before it gets executed. So we can wrap each SQL script with a transaction before it gets executed. In this case, you have to configure your builder to run WithoutTransaction and modify each script file before execution and explicitly wrap with a transaction if required. Writing a custom pre-processor is quickly done by implementing the IScriptPreprocessor interface, and you get the contents of the script file to modify. In this case, all I do is check whether the text contains 'CREATE FULLTEXT' and wrap with a transaction if it does not. You could use file-name conventions or any other rules of your choice to perform the check and conditionally wrap with a transaction.

``` csharp Conditionally Apply Transaction
public class ConditionallyApplyTransactionPreprocessor : IScriptPreprocessor
{
    public string Process(string contents)
    {
        if (!contents.Contains("CREATE FULLTEXT", StringComparison.InvariantCultureIgnoreCase))
        {
            var modified =
                $@"
BEGIN TRANSACTION   
BEGIN TRY
           {contents}
    COMMIT;
END TRY
BEGIN CATCH
    ROLLBACK;
    THROW;
END CATCH";

            return modified;
        }
        else
            return contents;
    }
}
```

### Using Multiple UpgradeEngine to Deploy Scripts

If you are not particularly fine with tweaking the pre-processing step and want to use the default implementations of DbUp and still achieve keep transactions for you scripts where possible, you can use multiple upgraders to perform the job for you. Iterate over all your script files and then partition them into batches of files that need to be run under a transaction and those that can't be run under a transaction. As shown in the image below you will end up with multiple batches with alternating sets of transaction/non-transaction set of scripts. When performing the upgrade over a batch, set the *WithTransactionPerScript* on the builder conditionally. If any of the batches fail, you can terminate the database upgrade.

<img src="{{site.images_root}}/dbup_batches.png" alt="Script file batches" class="center" />


``` csharp Execute all batches (Might not be production ready)
{
    Func<string,bool> canRunUnderTransaction = (fileName) => !fileName.Contains("FullText");
    Func<List<string>, string, bool> belongsToCurrentBatch = (batch, file) =>
		batch != null &&
        canRunUnderTransaction(batch.First()) == canRunUnderTransaction(file);
    
    var batches = allScriptFiles.Aggregate
        (new List<List<string>>(), (current, next) =>
            {
                if (belongsToCurrentBatch(current.LastOrDefault(),next))
                    current.Last().Add(next);
                else
                    current.Add(new List<string>() { next });

                return current;
            });

    foreach (var batch in batches)
    {
        var includeTransaction = !batch.Any(canRunUnderTransaction);

        var result = PerformUpgrade(batch.ToSqlScriptArray(), includeTransaction);

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
            return -1;
        }
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Success!");
    Console.ResetColor();
    return 0;
}

private static DatabaseUpgradeResult PerformUpgrade(
    SqlScript[] scripts,
    bool includeTransaction)
{
    var builder = DeployChanges.To
        .SqlDatabase(connectionString)
        .WithScripts(scripts)
        .LogToConsole();

    if (includeTransaction)
        builder = builder.WithTransactionPerScript();

      var upgrader = builder.Build();

    var result = upgrader.PerformUpgrade();

    return result;
}
```

Keeping all your scripts in a single place and automating it through the build-release pipeline is [something you need to strive for](https://rahulpnath.com/blog/working-effectively-under-constraints/). Hope this helps you to continue using DbUp even if you want to execute scripts that are a mix of transactional and non-transactional.