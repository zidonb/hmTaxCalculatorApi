using RulesEngine.Models;
using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Tests.BusinessRulesTests.clauses {

    [Collection("SharedCollection")]
    public class clause44Test {

        private static readonly string _workflowName = "clause44";
        private static readonly string _category = "credit";
        private static readonly int _year = 2022;
        private readonly WorkflowsFixture _fixture;

        public clause44Test(WorkflowsFixture worflowsFixture) {

            _fixture = worflowsFixture;
        }

        [Theory]
        [InlineData(true, 50000, 100000, -13125)]
        [InlineData(true, 10000, 100000, 0)]
        [InlineData(false, 50000, 100000, 0)]
        public async Task WorkflowClause44specialinstitutionTestAsync(bool israeliResident, double amountPaidForCare, double personalExertionIncome, double expected) {
            // Arrange
            var workFlowInputParams = new CalcTaxRequest
            {
                israeliResident = israeliResident,
                amountPaidForCare = amountPaidForCare,
                personalExertionIncome = personalExertionIncome
            };

            var rulesParam = new RuleParameter("Input1", workFlowInputParams);
            var inputs = new[] { rulesParam };
            var engine = _fixture._rulesEngine.GetRuleEngine(_year, _category);

            // Act
            var result = await engine.ExecuteAllRulesAsync(_workflowName, inputs);

            // Assert
            Assert.Equal(expected, _fixture._workFlowsService.GetTotalOutput(result));
        }
    }
}

//taxableIncomeAmount = שאינה מיגיעה אישית
