namespace TaxCalculator.Domain.Entities;

public class CalcTaxRequest {
    // hard coded values
    public int Year { get; set; }
    public bool individual { get; set; }
    public bool properAccountingBooks { get; set; }

    // MST_TAR_LEDA1
    public int individualAge { get; set; }

    // STAT_MIN_RASHUM
    public bool IsWoman { get; set; }

    // ??
    public double taxableIncomeNotFromPersonalExertion { get; set; }

    // CODE158 + CODE150
    public double personalExertionIncome { get; set; }

    // CODE20
    public bool israeliResident { get; set; }


    // CODE37
    public double donationAmount { get; set; }

    // CODE36
    public double lifeInsurence { get; set; }

    // CODE45
    public double pensionInsuranceEmployedDeposit { get; set; }

    // CODE248
    public double pensionInsuranceFromEmployerDeposit { get; set; }

    // CODE135
    public double pensionInsuranceSelfEmployedDeposit { get; set; }

    // ODDE140
    public double survivorPensionInsurance { get; set; }

    // CODE244
    public double insuredIncome { get; set; }

    // CODE206
    public double wrdInsuranceEmployedDeposit { get; set; }

    // CODE112
    public double wrdInsuranceSelfEmployedDeposit { get; set; }

    // CODE24
    public double regularServiceMonths { get; set; }

    // CODE224
    public string EndOfMilitaryServiceDate { get; set; }

    // CODE232/132
    public double amountPaidForCare { get; set; }

    // CODE23
    public int numberOfChildrenWithDisabilities { get; set; }

    // CODE260
    public string childrensData { get; set; } = "0000000000000";

    public double corporateTaxableIncome { get; set; }

    public double rentalIncome { get; set; }

    public double applicableRate{ get; set; }

}
