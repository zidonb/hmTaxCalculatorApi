using RulesEngine.Models;
using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Tests.BusinessRulesTests.clauses {

    [Collection("SharedCollection")]
    public class clause45ATest {

        private static readonly string _workflowName = "clause45A";
        private static readonly string _category = "credit";
        private static readonly int _year = 2022;
        private readonly WorkflowsFixture _fixture;

        public clause45ATest(WorkflowsFixture worflowsFixture) {

            _fixture = worflowsFixture;
        }


        [Theory]
        //[InlineData(0, 0, 10000, 0, 80000, 75000, 0, true, -3206)]
        //[InlineData(0, 0, 17000, 0, 100000, 0, 0, true, -175)]
        [InlineData(0, 7597, 0, 0, 262456, 0, 126612, true, -2616)]

        public async Task WorkflowClause45ATestAsync(
                  double lifeInsurence,
                  double pensionInsuranceEmployedDeposit,
                  double pensionInsuranceSelfEmployedDeposit,
                  double survivorPensionInsurance,
                  double personalExertionIncome,
                  double taxableIncomeNotFromPersonalExertion,
                  double insuredIncome,
                  bool israeliResident,
                  double expectedOutput
              ) {
            // Arrange
            var workFlowInputParams = new CalcTaxRequest
            {
                lifeInsurence = lifeInsurence,
                pensionInsuranceEmployedDeposit = pensionInsuranceEmployedDeposit,
                pensionInsuranceSelfEmployedDeposit = pensionInsuranceSelfEmployedDeposit,
                survivorPensionInsurance = survivorPensionInsurance,
                personalExertionIncome = personalExertionIncome,
                taxableIncomeNotFromPersonalExertion = taxableIncomeNotFromPersonalExertion,
                insuredIncome = insuredIncome,
                israeliResident = israeliResident
            };

            var rulesParam = new RuleParameter("Input1", workFlowInputParams);
            var inputs = new[] { rulesParam };

            var engine = _fixture._rulesEngine.GetRuleEngine(_year, _category);

            // Act
            var result = await engine.ExecuteAllRulesAsync(_workflowName, inputs);

            // Assert: Check if the result is as expected (e.g., success event, and correct output)
            var totalOutput = _fixture._workFlowsService.GetTotalOutput(result);
            Assert.Equal(expectedOutput, totalOutput, 1d); // Compare to the expected output with 1 decimal precision

        }
    }
}

//taxableIncomeAmount = שאינה מיגיעה אישית
