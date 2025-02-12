using RulesEngine.Models;
using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Tests.BusinessRulesTests.clauses {

    [Collection("SharedCollection")]
    public class clause36Test {

        private static readonly string _workflowName = "clause36";
        private static readonly string _category = "credit";
        private static readonly int _year = 2022;
        private readonly WorkflowsFixture _fixture;

        public clause36Test(WorkflowsFixture worflowsFixture) {

            _fixture = worflowsFixture;
        }

        [Theory]
        [InlineData(true, -669)]  // israeliResident true, should apply 25% to creditPointValue
        [InlineData(false, 0)]    // israeliResident false, should return 0 (failure case)
        public async Task WorkflowClause36IsraeliResidentTestAsync(bool israeliResident, double expected) {
            // Arrange
            var workFlowInputParams = new CalcTaxRequest
            {
                israeliResident = israeliResident
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
