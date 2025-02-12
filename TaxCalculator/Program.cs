using TaxCalculator.Infrastructure.Services;
using TaxCalculator.Presentation.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddConfigureSettings();
builder.Services.AddExternalServices();
builder.Services.AddAppServices();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Your existing API group
var taxCalculatorGroup = app.MapGroup("/api");

// POST: /api/calcTax
taxCalculatorGroup.MapPost("/calcTax", async (CalcTaxRequest workFlowInputParams, IWorkFlowsService taxCalculator) => await TaxCalculatorEndpoint.CalcTax(workFlowInputParams, taxCalculator))
    .WithName("CalcTax");

// GET: /api/reloadWorkflows
taxCalculatorGroup.MapGet("/reloadWorkflows", async (RuleEnginePerYearFactory factory,
            IConfiguration configuration) => await TaxCalculatorEndpoint.ReloadWorkflows(factory, configuration)) 
    .WithName("ReloadWorkflows");

    
app.Run();

