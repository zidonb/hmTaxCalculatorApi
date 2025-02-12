using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TaxCalculator.Infrastructure.Services;

namespace TaxCalculator.Tests.BusinessRulesTests;
public class WorkflowsFixture {



    public readonly string CONFIG_FOLDER = "Config";
    public readonly IMapper _mapper;
    public readonly Mock<ILogger<WorkFlowsService>> _mockLogger;
    public readonly Mock<ILogger<RuleEnginePerYearFactory>> _mockLoggerRuleEnginePerYearFactory;
    public readonly WorkFlowsService _workFlowsService;
    public readonly RuleEnginePerYearFactory _rulesEngine;
    public readonly RuleEngineLogger _ruleEngineLogger;

    public WorkflowsFixture() {

        _mockLogger = new Mock<ILogger<WorkFlowsService>>();
        _mapper = new Mock<IMapper>().Object;
        _mockLoggerRuleEnginePerYearFactory = new Mock<ILogger<RuleEnginePerYearFactory>>();
        _ruleEngineLogger = new RuleEngineLogger(new Mock<ILogger<RuleEngineLogger>>().Object);
        // Initialize RuleEnginePerYearFactory and load workflows
        _rulesEngine = new RuleEnginePerYearFactory(_mockLoggerRuleEnginePerYearFactory.Object);
        var baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIG_FOLDER);
        InitializeRulesEngineAsync(baseDirectory).Wait(); // This could be refactored as an async methodare loaded
        Console.WriteLine($"baseDirectory: {baseDirectory}");
        var loadedWorkflows = _rulesEngine.GetWorkflowNames(2022, "tax");
        Console.WriteLine($"Loaded workflows: {string.Join(", ", loadedWorkflows)}");
        _workFlowsService = new WorkFlowsService(_mockLogger.Object, _mapper, _rulesEngine, _ruleEngineLogger);
    }

    public async Task InitializeRulesEngineAsync(string baseDirectory) {
        await _rulesEngine.LoadWorkflowsAsync(baseDirectory); // Async initialization

        await _rulesEngine.AddMaskingFiles(baseDirectory);
    }
}


