using DbUp.Helpers;
using DbUp.ScriptProviders;
using DbUp;
using Microsoft.Extensions.Logging;
using DbUp.Engine;

namespace PracticeAssessment.Migration;

public class DatabaseVersionUp(ILogger<DatabaseVersionUp> logger)
{
    public static void EnsureDatabase(string connectionString)
    {
        DbUp.EnsureDatabase.For.SqlDatabase(connectionString);
    }

    public void UpdateVersion(string connectionString, string schema, string scriptPath, int timeOut)
    {

        logger.LogInformation("Start executing pre-deployment scripts...");

        //var preDeploymentScriptPath = Path.Combine(scriptPath, "PreDeployment");

        //RunPrePostDeploymentScripts(connectionString, preDeploymentScriptPath);

        logger.LogInformation("Start executing migration scripts...");

        var migrationScriptPath = Path.Combine(scriptPath, "Migrations");

        var isSuccess = RunDeploymentScripts(connectionString, schema, migrationScriptPath, timeOut);
        if (!isSuccess)
        {
            return;
        }

        logger.LogInformation("Start executing post-deployment scripts...");

        // var postDeploymentScriptPath = Path.Combine(scriptPath, "PostDeployment");

        //RunPrePostDeploymentScripts(connectionString, postDeploymentScriptPath);
    }

    private static void RunPrePostDeploymentScripts(string connectionString, string folder)
    {
        if (!Directory.Exists(folder))
        {
            ShowWarning($"Folder does not exist: {folder}");
        }

        var preDeploymentScriptsExecutor =
            DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsFromFileSystem(folder, new FileSystemScriptOptions
                {
                    IncludeSubDirectories = true
                })
                .LogToConsole()
                .JournalTo(new NullJournal())
                .WithTransactionPerScript()
                .WithExecutionTimeout(TimeSpan.FromMinutes(30))
                .Build();
        var preDeploymentUpgradeResult = preDeploymentScriptsExecutor.PerformUpgrade();

        if (!preDeploymentUpgradeResult.Successful)
        {
            ShowError(preDeploymentUpgradeResult.Error.ToString());
        }
        else
        {
            ShowSuccess();
        }
    }

    private static bool RunDeploymentScripts(string connectionString, string schema, string scriptPath, int timeOut)
    {
        if (Directory.Exists(scriptPath))
        {
            return ExecuteUpgrade(connectionString, schema, scriptPath, timeOut);
        }

        ShowWarning($"Script folder does not exist: {scriptPath}");
        return true;
    }

    private static bool ExecuteUpgrade(string connectionString, string schema, string scriptPath, int timeOut, bool keepJournal = true)
    {

        DatabaseUpgradeResult result;

        if (!keepJournal)
        {
            var upgrade =
                    DeployChanges.To
                        .SqlDatabase(connectionString)
                        .WithScriptsFromFileSystem(scriptPath, new FileSystemScriptOptions { IncludeSubDirectories = true })
                        .LogToConsole()
                        .JournalTo(new NullJournal())
                        .WithTransactionPerScript()
                        .WithExecutionTimeout(TimeSpan.FromMinutes(timeOut))
                        .Build();
            result = upgrade.PerformUpgrade();
        }
        else
        {
            var upgrades =
                    DeployChanges.To
                        .SqlDatabase(connectionString)
                        .WithScriptsFromFileSystem(scriptPath, new FileSystemScriptOptions { IncludeSubDirectories = true })
                        .LogToConsole()
                        .JournalToSqlTable("dbo", "MigrationsJournal")
                        .WithTransactionPerScript()
                        .WithExecutionTimeout(TimeSpan.FromMinutes(timeOut))
                        .Build();
            result = upgrades.PerformUpgrade();
        }

        if (result.Successful)
        {
            ShowSuccess();
            return true;
        }

        ShowError(result.Error.ToString());
        return false;
    }

    private static void ShowSuccess()
    {
        ShowMessage("Success!", ConsoleColor.Green);
    }

    private static void ShowWarning(string message)
    {
        ShowMessage(message, ConsoleColor.DarkYellow);
    }

    private static void ShowError(string error)
    {
        ShowMessage(error, ConsoleColor.Red);
    }

    private static void ShowMessage(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
