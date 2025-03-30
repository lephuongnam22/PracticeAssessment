// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PracticeAssessment.Migration;

var hostBuilder = Host.CreateDefaultBuilder(args);

var host = hostBuilder.Build();

var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");
var timeOut = Environment.GetEnvironmentVariable("TIMEOUT");

DatabaseVersionUp.EnsureDatabase(connectionString!);

var logger = host.Services.GetRequiredService<ILogger<DatabaseVersionUp>>();
var databaseVersionUp = new DatabaseVersionUp(logger);
var versionScriptPath = Path.GetFullPath("SQLs");

if (!Path.Exists(versionScriptPath))
{
    throw new ArgumentException("Script path doesn't exist");
}

int.TryParse(timeOut, out var secondTimeout);

databaseVersionUp.UpdateVersion(connectionString!, "practice", versionScriptPath, secondTimeout);