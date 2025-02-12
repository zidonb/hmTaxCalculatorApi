using RulesEngine.Interfaces;
using RulesEngine.Models;

namespace TaxCalculator.Infrastructure.Services {
    public class RuleEnginePerYearFactory {

        private readonly ILogger<RuleEnginePerYearFactory> _logger;
        private readonly Dictionary<int, Dictionary<string, IRulesEngine>> _engines =
            [];
        private readonly Dictionary<int, Dictionary<string, List<string>>> _workflowNames =
            [];

        // הערך הראשון מייצג את השנה, השני מייצג את רשימת התלויות
        public readonly Dictionary<int, Dictionary<string, List<string>?>> _workflowsDependencies =
        [];

        public Dictionary<string, bool> _maskingCompanies = new Dictionary<string, bool>();

        public RuleEnginePerYearFactory(ILogger<RuleEnginePerYearFactory> logger) {
            _logger = logger;
        }

        public IRulesEngine? GetRuleEngine(int year, string category) {
            if (_engines.TryGetValue(year, out var categories) &&
                categories.TryGetValue(category, out var engine)) {
                return engine;
            }
            return null;
        }

        public async Task<WorkflowLoadResult> LoadWorkflowsAsync(string baseDirectory) {
            // Create a result object to track errors and logs
            var result = new WorkflowLoadResult();

            var defaultPath = Path.Combine(baseDirectory, "default");

            // Load workflows for each year
            var yearDirectories = Directory.EnumerateDirectories(baseDirectory)
                .Where(d => int.TryParse(Path.GetFileName(d), out _));

            foreach (var yearDir in yearDirectories) {
                int year = int.Parse(Path.GetFileName(yearDir));

                foreach (var category in new[] { "deduction", "tax", "credit", "summary" }) {
                    var categoryPath = Path.Combine(yearDir, category);

                    if (!Directory.Exists(categoryPath)) {
                        _logger.LogInformation($"Category directory not found. Creating: {categoryPath}");
                        result.Logs.AppendLine($"Category directory not found for {category}. Creating: {categoryPath}");
                        Directory.CreateDirectory(categoryPath);
                    }

                    try {
                        // Load the category with defaults
                        await LoadCategoryWithDefaults(baseDirectory, category, year, categoryPath, result);
                    }
                    catch (Exception ex) {
                        // If an error occurs during loading, log the error
                        _logger.LogError($"Error loading category '{category}' for year {year}: {ex.Message}");
                        result.Errors.Add($"Error loading category '{category}' for year {year}: {ex.Message}");
                    }
                }
            }

            return result;
        }

        public List<string> GetWorkflowNames(int year, string category) {
            if (_workflowNames.TryGetValue(year, out var categories) &&
                categories.TryGetValue(category, out var workflows)) {
                return workflows;
            }

            return [];
        }

        #region Private Functions

        private async Task LoadCategoryWithDefaults(string baseDirectory, string category, int year, string yearCategoryPath, WorkflowLoadResult result) {
            // Get or create the rule engine for the year and category
            var engine = _engines.GetOrAdd(year, _ => new Dictionary<string, IRulesEngine>())
                .GetOrAdd(category, _ => new RulesEngine.RulesEngine(new ReSettings { CustomTypes = new Type[] { typeof(CsvUtilities) } }));

            var dependencies = _workflowsDependencies.GetOrAdd(year, _ => new Dictionary<string, List<string>?>());

            // Track workflow names
            if (!_workflowNames.ContainsKey(year))
                _workflowNames[year] = new Dictionary<string, List<string>>();
            if (!_workflowNames[year].ContainsKey(category))
                _workflowNames[year][category] = new List<string>();

            // Load default workflows for this rule engine and for this year
            var defaultPath = Path.Combine(baseDirectory, Constants.DEFAULT_DIRECTORY_NAME, category);
            if (Directory.Exists(defaultPath)) {
                await AddWorkflowsFromDirectoryAsync(defaultPath, engine, year, category, result, dependencies);
            }

            // Load year-specific workflows (if any)
            var yearSpecificPath = Path.Combine(yearCategoryPath);
            if (Directory.Exists(yearSpecificPath)) {
                await AddWorkflowsFromDirectoryAsync(yearSpecificPath, engine, year, category, result, dependencies);
            }
        }

        private static List<string>? GetDependenciesByWorkflow(Workflow workflow) {
            return workflow?.GlobalParams?.Where(w => w.Name.StartsWith(Constants.DEPENDENCY_GLOBAL_PARAM_NAME)).ToList().Select(w => w.Expression).ToList();
        }

        public async Task AddMaskingFiles(string baseDirectory) {

            if (Directory.Exists(baseDirectory)) {
                try {
                    var maskingFile = Path.Combine(baseDirectory, Constants.MASKING_DIRECTORY, "companiesMasking.json");


                    try {

                        var maskingData = await File.ReadAllTextAsync(maskingFile);
                        _maskingCompanies = JsonConvert.DeserializeObject<Dictionary<string, bool>>(maskingData) ?? [];
                    }

                    catch (Exception) {

                    }

                }
                catch (Exception) {

                }

            }
        }
        private async Task AddWorkflowsFromDirectoryAsync(string path, IRulesEngine engine, int year, string category, WorkflowLoadResult result, Dictionary<string, List<string>?> dependencies) {
            // Ensure we only add workflows if the directory exists
            if (Directory.Exists(path)) {
                try {
                    var workflowFiles = Directory.EnumerateFiles(path, "*.json");

                    foreach (var file in workflowFiles) {
                        try {
                            // add the workflow to the engine 
                            var workflowData = await File.ReadAllTextAsync(file);
                            var workflow = JsonConvert.DeserializeObject<Workflow>(workflowData);
                            if (workflow != null) {
                                if (category != Constants.SUMMARY_WORKFLOW_NAME) {
                                    dependencies[workflow.WorkflowName] = GetDependenciesByWorkflow(workflow);
                                }
                                // Add workflow to the engine 
                                engine.AddOrUpdateWorkflow(workflow);

                                // Track the name of the workflow
                                _workflowNames[year][category].Add(workflow.WorkflowName);

                                _logger.LogInformation("Workflow {WorkflowName} added/overridden for year {Year}, category {Category}.", workflow.WorkflowName, year, category);
                            }

                        }
                        catch (Exception ex) {
                            _logger.LogError($"Error loading workflow from {file}: {ex.Message}");
                            // Log any specific issues with individual workflows
                            result.Errors.Add($"Error loading workflow from {file}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex) {
                    // Log any issues loading workflows from the directory
                    _logger.LogError($"Error reading workflows from {path}: {ex.Message}");
                    result.Errors.Add($"Error reading workflows from {path}: {ex.Message}");
                }
            }
        }
        #endregion
    }
}
