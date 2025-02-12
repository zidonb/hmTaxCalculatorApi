namespace TaxCalculator.Application.Validations.Request {
    public class CalcTaxRequestValidator : AbstractValidator<CalcTaxRequest> {

        public CalcTaxRequestValidator() {

            RuleFor(request => request.Year)
           .NotNull()
           .GreaterThan(2020)
           .WithMessage("'{PropertyName}' can't be null");


        }
    }
}
