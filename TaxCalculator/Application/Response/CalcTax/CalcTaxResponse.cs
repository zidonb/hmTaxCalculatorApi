namespace TaxCalculator.Application.Response.CalcTax;

/// <summary>
/// Holds the results of a tax calculation, including deductions, taxes, credits, and the total amount.
/// </summary>
/// <param name="Deduction">The total amount of tax deductions applied.</param>
/// <param name="Tax">The total amount of tax calculated.</param>
/// <param name="Credit">The total amount of tax credits applied.</param>
/// <param name="Total">The total amount of tax after applying deductions and credits.</param>
/// <param name="ResultObject"></param>
/// <param name="GroupedObject"></param>
/// <param name="ResultGroupedObject"></param>
public record CalcTaxResponse(double Deduction, double Tax, double Credit, double Total, Dictionary<string, double?> ResultObject, Dictionary<string, List<Dictionary<string, double>>> GroupedObject, Dictionary<string, double> ResultGroupedObject);

