
namespace TaxCalculator.Infrastructure.Services;

/// <summary>
/// המחלקה הזאת נטענת בעליית המערכת
/// המטרה שלה היא לטעון את כל החוקים מהתיקיות
/// </summary>
public class WorkFlowsLoader : IHostedService {
    private readonly RuleEnginePerYearFactory _ruleEnginePerYearFactory;
    private readonly IConfiguration _configurationApp;

    public WorkFlowsLoader(RuleEnginePerYearFactory ruleEnginePerYearFactory, IConfiguration configuration) {
        _ruleEnginePerYearFactory = ruleEnginePerYearFactory;
        _configurationApp = configuration;
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
            workflowDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.WORKFLOW_DIRECTORY_NAME);
        }

        return workflowDirectoryPath;

    }

}
