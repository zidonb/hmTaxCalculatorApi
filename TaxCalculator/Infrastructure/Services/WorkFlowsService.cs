using RulesEngine.Interfaces;
using RulesEngine.Models;
using TaxCalculator.Application.Interfaces.Services;
using TaxCalculator.Application.Response.CalcTax;

namespace TaxCalculator.Infrastructure.Services {
    public partial class WorkFlowsService : BaseService<WorkFlowsService>, IWorkFlowsService {
        private readonly RuleEnginePerYearFactory _rulesEngineFactory;
        private readonly IRuleEngineLogger _ruleEngineLogger;
        public Dictionary<string, double> _resultObject;
        public Dictionary<string, double> _resultGroupedObject;
        public Dictionary<string, List<Dictionary<string, double>>> _groupedObject;
        public Dictionary<string, List<string>> _workflowsObject;


        public WorkFlowsService(
            ILogger<WorkFlowsService> logger,
            IMapper mapper,
            RuleEnginePerYearFactory rulesEngineFactory,
            IRuleEngineLogger ruleEngineLogger) : base(logger, mapper) {
            _rulesEngineFactory = rulesEngineFactory;
            _ruleEngineLogger = ruleEngineLogger;
            _resultObject = new Dictionary<string, double>();
            _resultGroupedObject = new Dictionary<string, double>();
            _groupedObject = new Dictionary<string, List<Dictionary<string, double>>>();
            _workflowsObject = new Dictionary<string, List<string>>();
        }

        public async Task<CalcTaxResponse> CalculateTaxAsync<T>(T workFlowInputParams) {
            int workFlowInputParamsYear = 0;
            bool IsIndividual = false; 
            switch (workFlowInputParams) {
                case CalcTaxRequest calcTaxRequest:
                    workFlowInputParamsYear = calcTaxRequest.Year;
                    IsIndividual = calcTaxRequest.individual;
                    break;
            }
            // Initialize dictionaries
            _resultObject = new Dictionary<string, double>();
            _groupedObject = new Dictionary<string, List<Dictionary<string, double>>>();
            _workflowsObject.Clear();
            _resultGroupedObject.Clear();

            // Prepare rule engine inputs
            var rulesParam1 = new RuleParameter("Input1", workFlowInputParams);
            var inputs = new[] { rulesParam1 };

            // Run workflows for deduction, tax, and credit categories
            var deduction = await RunWorkflows(inputs, workFlowInputParamsYear, "deduction");
            var tax = await RunWorkflows(inputs, workFlowInputParamsYear, "tax");
            var credit = await RunWorkflows(inputs, workFlowInputParamsYear, "credit");

            // Group results into _groupedObject for dependency handling
            GroupResults(_groupedObject, _resultObject, workFlowInputParamsYear);

            // Calculate the summarized totals for each type
            var typeSums = await CalculateSummaryForEachType(workFlowInputParamsYear);

            // Log calculated sums for each type
            foreach (var typeSum in typeSums) {
                _logger.LogInformation("Total for {Type}: {Sum}", typeSum.Key, typeSum.Value);
            }
            
            var resultObject = ApplyMasking(_resultObject, IsIndividual);
            
            // Return the final TaxRecord with totals
            return new CalcTaxResponse(
                typeSums.GetValueOrDefault("deduction", 0), // Deduction total
                typeSums.GetValueOrDefault("tax", 0),       // Tax total
                typeSums.GetValueOrDefault("credit", 0),    // Credit total
                typeSums.Values.Sum(),                       // Overall total
                resultObject,
                _groupedObject,
                _resultGroupedObject
            );
        }

        private Dictionary<string, double?> ApplyMasking(Dictionary<string, double> resultObject, bool IsIndividual) {

            var result = new Dictionary<string, double?>();
            var mask = _rulesEngineFactory._maskingCompanies;

            foreach (var kvp in resultObject) {
                bool shouldMask = false;
                if (mask.TryGetValue(kvp.Key, out bool maskValue)) {
                    shouldMask = IsIndividual ? !maskValue : maskValue; 
                }

                if (shouldMask && kvp.Value == 0) {
                    result[kvp.Key] = null;
                }
                else {
                    result[kvp.Key] = kvp.Value;
                }
            }

            return result;
        }


        public async Task<double> RunWorkflows(RuleParameter[] inputs, int year, string category) {
            var engine = _rulesEngineFactory.GetRuleEngine(year, category);
            if (engine == null) {
                _logger.LogWarning("No engine available for year {Year}, category {Category}.", year, category);
                return 0;
            }

            var workflows = _rulesEngineFactory.GetWorkflowNames(year, category);
            if (!workflows.Any()) {
                _logger.LogWarning("No workflows found for year {Year}, category {Category}.", year, category);
                return 0;
            }

            double total = 0;
            _workflowsObject.Add(category, workflows);
            foreach (var workflow in workflows) {
                try {
                    _logger.LogInformation("Executing workflow {Workflow} for year {Year}, category {Category}.", workflow, year, category);

                    total += await RunWorkflow(workflow, engine, inputs);
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Error executing workflow {Workflow} for year {Year}, category {Category}.", workflow, year, category);
                }
            }

            return total;
        }

        public double GetTotalOutput(List<RuleResultTree> results) {
            double CalculateTotal(RuleResultTree resultTree) {
                // Base case: Sum the current node's output if it's not null
                double currentOutput = 0;

                if (resultTree.ActionResult.Output != null && (!(resultTree.ActionResult.Output is bool boolOutput && boolOutput == true))) {
                    currentOutput = Convert.ToDouble(resultTree.ActionResult.Output);
                }


                // Recursively calculate the total for all child results
                double childOutput = resultTree.ChildResults?.Sum(CalculateTotal) ?? 0;

                return currentOutput + childOutput;
            }

            // Start calculation for all root-level results
            return results.Sum(CalculateTotal);
        }

        public void GroupResults(Dictionary<string, List<Dictionary<string, double>>> groupedObject, Dictionary<string, double> resultObject, int year) {
            // Dictionary to hold dependencies for a specific year
            Dictionary<string, List<string>?>? relevantWorkflowDependencies;

            // Check if the dependencies for the given year exist
            if (_rulesEngineFactory._workflowsDependencies.TryGetValue(year, out relevantWorkflowDependencies)) {
                resultObject.ToList().ForEach(r => {
                    string workflowName = r.Key;

                    // If the workflow has dependencies
                    if (relevantWorkflowDependencies.TryGetValue(workflowName, out List<string>? relevantWorkflowDependenciesValues) && relevantWorkflowDependenciesValues?.Count > 0) {
                        // Process dependent clauses
                        var dependencies = relevantWorkflowDependenciesValues.Distinct().OrderBy(clause => clause).ToList();
                        string groupedKey = workflowName + "_" + string.Join("_", dependencies); // Create a concatenated key for dependent clauses
                        var clausesNum = groupedKey.Split("_"); // ["121","126"]
                        if (!groupedObject.Keys.Any(k => clausesNum.Any(num => k.Contains(num)))) {
                            // Prepare a list of key-value pairs for the dependent clauses
                            var groupedList = new List<Dictionary<string, double>>();
                            groupedList.Add(new Dictionary<string, double> { { workflowName, resultObject[workflowName] } });
                            foreach (var dependency in dependencies) {
                                var clauseName = string.Concat("clause", dependency);
                                if (resultObject.ContainsKey(clauseName)) {
                                    groupedList.Add(new Dictionary<string, double> { { clauseName, resultObject[clauseName] } });
                                }
                            }

                            // Add the group to the groupedObject
                            if (groupedList.Count > 0) {
                                groupedObject[groupedKey] = groupedList;
                            }
                        }

                    }
                    else {
                        // This is a clause with no dependencies
                        groupedObject[workflowName] = new List<Dictionary<string, double>>
                {
                    new Dictionary<string, double> { { workflowName, r.Value } }
                };
                    }
                });
            }
        }

        public async Task<double> RunWorkflow(string workflow, IRulesEngine engine, RuleParameter[] inputs) {
            var result = await engine.ExecuteAllRulesAsync(workflow, inputs);
            _ruleEngineLogger.LogRuleResultTrees(result);

            _resultObject.Add(workflow, GetFinalResult(result));


            return GetTotalOutput(result);


        }

        public bool? CheckEntryCondition(List<RuleResultTree> result) {

            var entryCondition = result.FirstOrDefault(r => r.Rule.RuleName == "WorkflowEntryCondition");

            if ((entryCondition != null && entryCondition.IsSuccess) || entryCondition == null) {
                return true;
            }

            return null;
        }

        public double GetFinalResult(List<RuleResultTree> result) { 
        
            return CheckEntryCondition(result).GetValueOrDefault() ? GetTotalOutput(result) : 0;
        }

        public async Task<Dictionary<string, double>> CalculateSummaryForEachType(int year) {
            var typeSums = new Dictionary<string, double>(); // Store the total sum for each type
            var processedGroupedKeys = new HashSet<string>(); // Track grouped keys to avoid double-processing

            _logger.LogInformation("Starting calculation for year {Year}", year);

            foreach (var type in _workflowsObject.Keys) {
                double typeSum = 0;

                // Track workflows already included in grouped processing
                var processedWorkflows = new HashSet<string>();

                _logger.LogInformation("Processing type: {Type}", type);

                foreach (var workflow in _workflowsObject[type]) {
                    // Check if the workflow is part of a grouped object
                    var groupedWorkflow = _groupedObject.FirstOrDefault(g => g.Value.Any(group => group.ContainsKey(workflow)));
                    var workflowCount = groupedWorkflow.Value.Count(); // Count the number of workflows in the group

                    if (!groupedWorkflow.Equals(default(KeyValuePair<string, List<Dictionary<string, double>>>)) &&
                        !processedGroupedKeys.Contains(groupedWorkflow.Key) && workflowCount > 1) {
                        // Process grouped workflows only once
                        _logger.LogInformation("Processing grouped workflow: {GroupedWorkflow}", groupedWorkflow.Key);

                        processedGroupedKeys.Add(groupedWorkflow.Key);

                        // Collect all key-value pairs from the grouped workflows
                        var ruleInputs = groupedWorkflow.Value
                            .SelectMany(group => group)
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                        // Send the grouped workflow to the summary rule engine
                        var summaryRuleEngine = _rulesEngineFactory.GetRuleEngine(year, "summary");
                        if (summaryRuleEngine != null) {
                            var ruleParameters = new RuleParameter("Input1", ruleInputs);
                            var summaryResult = await summaryRuleEngine.ExecuteAllRulesAsync(groupedWorkflow.Key, new[] { ruleParameters });

                            // Log rule engine result trees
                            _ruleEngineLogger.LogRuleResultTrees(summaryResult);

                            // Add the processed value from the summary engine
                            var output = GetTotalOutput(summaryResult);
                            _logger.LogInformation("Output for grouped workflow {GroupedWorkflow}: {Output}", groupedWorkflow.Key, output);
                            _resultGroupedObject.Add(groupedWorkflow.Key, output);
                            typeSum += output;
                        }

                        // Mark all workflows in this group as processed
                        foreach (var group in groupedWorkflow.Value) {
                            foreach (var kvp in group) {
                                processedWorkflows.Add(kvp.Key);
                            }
                        }
                    }
                    else if (!processedWorkflows.Contains(workflow) && _resultObject.ContainsKey(workflow)) {
                        // Process standalone workflows
                        _logger.LogInformation("Processing standalone workflow: {Workflow}", workflow);
                        typeSum += _resultObject[workflow];
                        processedWorkflows.Add(workflow);
                    }
                }

                // Store the calculated sum for the type
                _logger.LogInformation("Total sum for type {Type}: {TypeSum}", type, typeSum);
                typeSums[type] = typeSum;
            }

            _logger.LogInformation("Calculation completed for year {Year}", year);
            return typeSums;
        }


    }
}
