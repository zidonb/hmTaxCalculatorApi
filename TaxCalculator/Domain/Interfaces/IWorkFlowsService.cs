using TaxCalculator.Application.Response.CalcTax;

namespace TaxCalculator.Domain.Interfaces;

public interface IWorkFlowsService {

    public Task<CalcTaxResponse> CalculateTaxAsync<T>(T workFlowInputParams);

}
