using RulesEngine.Models;
using System.Text;
using TaxCalculator.Application.Interfaces.Services;

public class RuleEngineLogger : IRuleEngineLogger {
    private readonly ILogger<RuleEngineLogger> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuleEngineLogger"/> class.
    /// </summary>
    /// <param name="logger">The logger to be used for logging rule engine results.</param>
    public RuleEngineLogger(ILogger<RuleEngineLogger> logger) {
        _logger = logger;
    }

    /// <summary>
    /// Logs a collection of rule result trees.
    /// </summary>
    /// <param name="ruleResultTrees">The collection of <see cref="RuleResultTree"/> to log.</param>
    public void LogRuleResultTrees(IEnumerable<RuleResultTree> ruleResultTrees) {
        if (ruleResultTrees == null || !ruleResultTrees.Any()) {
            _logger.LogWarning("No RuleResultTrees to log.");
            return;
        }

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Rule Result Trees:");

        foreach (var ruleResultTree in ruleResultTrees) {
            AppendRuleResultTree(ruleResultTree, stringBuilder, 0);
        }

        _logger.LogInformation(stringBuilder.ToString());
    }

    /// <summary>
    /// Recursively appends information from a <see cref="RuleResultTree"/> to the provided <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="ruleResultTree">The <see cref="RuleResultTree"/> to append.</param>
    /// <param name="stringBuilder">The <see cref="StringBuilder"/> to append information to.</param>
    /// <param name="indentLevel">The current level of indentation for nested trees.</param>
    private void AppendRuleResultTree(RuleResultTree ruleResultTree, StringBuilder stringBuilder, int indentLevel) {
        var indent = new string(' ', indentLevel * 4);
        AppendRuleBasicInfo(ruleResultTree, stringBuilder, indent);
        AppendRuleInputs(ruleResultTree, stringBuilder, indent);
        AppendRuleActions(ruleResultTree, stringBuilder, indent);
        AppendActionResult(ruleResultTree.ActionResult, stringBuilder, indent);
        AppendExceptionMessage(ruleResultTree.ExceptionMessage, stringBuilder, indent);

        if (ruleResultTree.ChildResults != null && ruleResultTree.ChildResults.Any()) {
            foreach (var child in ruleResultTree.ChildResults) {
                AppendRuleResultTree(child, stringBuilder, indentLevel + 1);
            }
        }
    }

    /// <summary>
    /// Appends basic information about a <see cref="RuleResultTree"/> to the <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="ruleResultTree">The <see cref="RuleResultTree"/> containing the basic information.</param>
    /// <param name="stringBuilder">The <see cref="StringBuilder"/> to append information to.</param>
    /// <param name="indent">The indentation string to format the output.</param>
    private void AppendRuleBasicInfo(RuleResultTree ruleResultTree, StringBuilder stringBuilder, string indent) {
        stringBuilder.AppendLine($"{indent}Rule: {ruleResultTree.Rule.RuleName}");
        stringBuilder.AppendLine($"{indent}  IsSuccess: {ruleResultTree.IsSuccess}");
        stringBuilder.AppendLine($"{indent}  Enabled: {ruleResultTree.Rule.Enabled}");
        stringBuilder.AppendLine($"{indent}  Expression: {ruleResultTree.Rule.Expression}");
        stringBuilder.AppendLine($"{indent}  Operator: {ruleResultTree.Rule.Operator}");
        stringBuilder.AppendLine($"{indent}  Properties: {JsonConvert.SerializeObject(ruleResultTree.Rule.Properties)}");
    }

    /// <summary>
    /// Appends the inputs of a <see cref="RuleResultTree"/> to the <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="ruleResultTree">The <see cref="RuleResultTree"/> containing the inputs.</param>
    /// <param name="stringBuilder">The <see cref="StringBuilder"/> to append information to.</param>
    /// <param name="indent">The indentation string to format the output.</param>
    private void AppendRuleInputs(RuleResultTree ruleResultTree, StringBuilder stringBuilder, string indent) {
        stringBuilder.AppendLine($"{indent}  Inputs: {JsonConvert.SerializeObject(ruleResultTree.Inputs)}");
    }

    /// <summary>
    /// Appends the actions associated with a <see cref="RuleResultTree"/> to the <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="ruleResultTree">The <see cref="RuleResultTree"/> containing the actions.</param>
    /// <param name="stringBuilder">The <see cref="StringBuilder"/> to append information to.</param>
    /// <param name="indent">The indentation string to format the output.</param>
    private void AppendRuleActions(RuleResultTree ruleResultTree, StringBuilder stringBuilder, string indent) {
        if (ruleResultTree.IsSuccess) {
            stringBuilder.AppendLine($"{indent}  SuccessEvent: {ruleResultTree.Rule.SuccessEvent}");
            if (ruleResultTree.Rule.Actions?.OnSuccess != null) {
                stringBuilder.AppendLine($"{indent}  OnSuccess Action: {JsonConvert.SerializeObject(ruleResultTree.Rule.Actions.OnSuccess)}");
            }
        }
        else {
            stringBuilder.AppendLine($"{indent}  ErrorMessage: {ruleResultTree.Rule.ErrorMessage}");
            if (ruleResultTree.Rule.Actions?.OnFailure != null) {
                stringBuilder.AppendLine($"{indent}  OnFailure Action: {JsonConvert.SerializeObject(ruleResultTree.Rule.Actions.OnFailure)}");
            }
        }
    }

    /// <summary>
    /// Appends the result of an action from a <see cref="RuleResultTree"/> to the <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="actionResult">The action result to append.</param>
    /// <param name="stringBuilder">The <see cref="StringBuilder"/> to append information to.</param>
    /// <param name="indent">The indentation string to format the output.</param>
    private void AppendActionResult(RulesEngine.Models.ActionResult actionResult, StringBuilder stringBuilder, string indent) {
        if (actionResult != null) {
            stringBuilder.AppendLine($"{indent}  ActionResult Output: {actionResult.Output}");
            if (actionResult.Exception != null) {
                stringBuilder.AppendLine($"{indent}  ActionResult Exception: {actionResult.Exception.Message}");
            }
        }
    }

    /// <summary>
    /// Appends any exception message from a <see cref="RuleResultTree"/> to the <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="exceptionMessage">The exception message to append.</param>
    /// <param name="stringBuilder">The <see cref="StringBuilder"/> to append information to.</param>
    /// <param name="indent">The indentation string to format the output.</param>
    private void AppendExceptionMessage(string exceptionMessage, StringBuilder stringBuilder, string indent) {
        if (!string.IsNullOrEmpty(exceptionMessage)) {
            stringBuilder.AppendLine($"{indent}  ExceptionMessage: {exceptionMessage}");
        }
    }
}
