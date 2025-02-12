using RulesEngine.Models;

namespace TaxCalculator.Application.Interfaces.Services;

public interface IRuleEngineLogger {
    void LogRuleResultTrees(IEnumerable<RuleResultTree> ruleResultTrees);
}