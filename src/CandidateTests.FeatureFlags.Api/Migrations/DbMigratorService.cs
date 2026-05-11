using System.Reflection;
using DbUp;
using Microsoft.Extensions.Logging;

namespace CandidateTests.FeatureFlags.Api.Migrations;

public class DbMigratorService : IDbMigratorService
{
    private readonly string _connectionString;
    private readonly ILogger<DbMigratorService> _logger;

    public DbMigratorService(string connectionString, ILogger<DbMigratorService> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public void MigrateDb()
    {
        var upgradeEngine = DeployChanges.To
            .SQLiteDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithVariablesDisabled()
            .WithTransactionPerScript()
            .LogToAutodetectedLog()
            .Build();

        if (!upgradeEngine.IsUpgradeRequired())
        {
            return;
        }

        var pending = upgradeEngine.GetScriptsToExecute();
        _logger.LogInformation("DbUp -> upgrade required. Scripts to run: {Count}", pending.Count);

        var result = upgradeEngine.PerformUpgrade();
        if (!result.Successful)
        {
            _logger.LogError(result.Error, "DbUp -> migration failed on script {Script}", result.ErrorScript.Name);
            throw result.Error;
        }

        _logger.LogInformation("DbUp -> migration complete");
    }
}
