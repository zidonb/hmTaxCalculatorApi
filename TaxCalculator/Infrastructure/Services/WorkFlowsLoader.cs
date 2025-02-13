
namespace TaxCalculator.Infrastructure.Services;

/// <summary>
/// המחלקה הזאת נטענת בעליית המערכת
/// המטרה שלה היא לטעון את כל החוקים מהתיקיות
/// </summary>
public class WorkFlowsLoader : IHostedService {
    private readonly RuleEnginePerYearFactory _ruleEnginePerYearFactory;
    private readonly IConfiguration _configurationApp;

    private static ILogger<WorkFlowsLoader> _logger;

    public WorkFlowsLoader(RuleEnginePerYearFactory ruleEnginePerYearFactory, IConfiguration configuration, ILogger<WorkFlowsLoader> logger) {
        _ruleEnginePerYearFactory = ruleEnginePerYearFactory;
        _configurationApp = configuration;
        _logger = logger;
    }
    public async Task StartAsync(CancellationToken cancellationToken) {
        // Get the directory path from configuration
        string workflowDirectoryPath = GetworkflowDirectoryPath(_configurationApp);
        await _ruleEnginePerYearFactory.LoadWorkflowsAsync(workflowDirectoryPath);
        await _ruleEnginePerYearFactory.AddMaskingFiles(workflowDirectoryPath);
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public static string GetworkflowDirectoryPath(IConfiguration configuration) {
        // בודקים אם יש ערך בקובץ קונפיג, אם יש אז לוקחים אותו, אם אין אז מנסים לחפש במקומי
        string workflowDirectoryPath = configuration.GetValue<string>("WorkflowDirectoryPath") ?? string.Empty;
       
        if (string.IsNullOrEmpty(workflowDirectoryPath) || !Directory.Exists(workflowDirectoryPath)) {
            _logger.LogInformation("Workflow directory path not found in configuration. Using default path.");
            workflowDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.WORKFLOW_DIRECTORY_NAME);
        }
         _logger.LogInformation("Workflow directory path: {0}", workflowDirectoryPath);

        return workflowDirectoryPath;

    }

}
