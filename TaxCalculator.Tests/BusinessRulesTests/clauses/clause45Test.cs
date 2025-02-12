using RulesEngine.Models;
using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Tests.BusinessRulesTests.clauses {

    [Collection("SharedCollection")]
    public class clause45Test {

        private static readonly string _workflowName = "clause45";
        private static readonly string _category = "credit";
        private static readonly int _year = 2022;
        private readonly WorkflowsFixture _fixture;

        public clause45Test(WorkflowsFixture worflowsFixture) {

            _fixture = worflowsFixture;
        }

        [Theory]
        [InlineData(true, 1, -5352)]
        [InlineData(true, 2, -10704)]
        public async Task WorkflowClause45disabilityTestAsync(bool israeliResident, int numberOfChildrenWithDisabilities, double expected) {
            // Arrange
            var workFlowInputParams = new CalcTaxRequest
            {
                israeliResident = israeliResident,
                numberOfChildrenWithDisabilities = numberOfChildrenWithDisabilities,
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

