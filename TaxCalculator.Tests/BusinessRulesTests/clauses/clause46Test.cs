using RulesEngine.Models;
using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Tests.BusinessRulesTests.clauses {

    [Collection("SharedCollection")]
    public class clause46Test {

        private static readonly string _workflowName = "clause46";
        private static readonly string _category = "credit";
        private static readonly int _year = 2022;
        private readonly WorkflowsFixture _fixture;

        public clause46Test(WorkflowsFixture worflowsFixture) {

            _fixture = worflowsFixture;
        }

        [Theory]
        [InlineData(100000, 200000, 100, 0)]
        [InlineData(100000, 0, 36000, -10500)]

        public async Task WorkflowClause46TestAsync(double taxableIncomeNotFromPersonalExertion, double personalExertionIncome, double donationAmount, double expected) {
            // Arrange
            var workFlowInputParams = new CalcTaxRequest
            {
                individual = true,
                taxableIncomeNotFromPersonalExertion = taxableIncomeNotFromPersonalExertion,
                personalExertionIncome = personalExertionIncome,
                donationAmount = donationAmount
            };

            var rulesParam = new RuleParameter("Input1", workFlowInputParams);
            var inputs = new[] { rulesParam };
            var engine = _fixture._rulesEngine.GetRuleEngine(_year, _category);

            // Act
            var result = await engine.ExecuteAllRulesAsync(_workflowName, inputs);

            // Assert
            Assert.Equal(expected, _fixture._workFlowsService.GetTotalOutput(result), 1d);
        }
    }
}

//taxableIncomeAmount = שאינה מיגיעה אישית
