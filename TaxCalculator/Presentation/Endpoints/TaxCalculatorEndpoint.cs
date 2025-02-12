using TaxCalculator.Infrastructure.Services;

namespace TaxCalculator.Presentation.Endpoints;
public class TaxCalculatorEndpoint  {
    #region Private members

    private const string TAX_CALCULATOR = Constants.TAX_CALCULATOR_GROUP;

    #endregion

    public void RegisterEndpoints(IEndpointRouteBuilder app) {
        var group = app.MapGroup(TAX_CALCULATOR);

        group.MapPost("/api/calcTax", CalcTax)
            .AddEndpointFilter<ValidationFilter<CalcTaxRequest>>()
            .WithName(nameof(CalcTax));

        group.MapGet("/api/reloadWorkflows", ReloadWorkflows)
            .WithName(nameof(ReloadWorkflows));
    }

    public async Task<IResult> CalcTax(CalcTaxRequest workFlowInputParams, IWorkFlowsService taxCalculator) {
        var res = await taxCalculator.CalculateTaxAsync(workFlowInputParams);
        return Results.Ok(res);
    }

    public async Task<IResult> ReloadWorkflows(RuleEnginePerYearFactory factory,
            IConfiguration configuration) {
        try {
            // Get the directory path from configuration
            string workflowDirectoryPath = WorkFlowsLoader.GetworkflowDirectoryPath(configuration);

            // Load workflows and capture errors/warnings
            var result = await factory.LoadWorkflowsAsync(workflowDirectoryPath);

            if (result.HasErrors) {
                // If there were errors, return a bad request with the error details
                return Results.BadRequest(new { Message = "Failed to reload  workflows", Errors = result.Errors });
            }

            // Load workflows and capture errors/warnings
             await factory.AddMaskingFiles(workflowDirectoryPath);

            if (result.HasErrors) {
                // If there were errors, return a bad request with the error details
                return Results.BadRequest(new { Message = "Failed to reload  workflows", Errors = result.Errors });
            }

            // Return successful result with no issues
            return Results.Ok(new { Message = "All workflows reloaded successfully." });
        }
        catch (Exception ex) {
            // Return bad request in case of any other error
            return Results.BadRequest($"Failed to reload workflows: {ex.Message}");
        }
    }

}



