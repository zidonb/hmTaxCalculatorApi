using RulesEngine.Models;
using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Tests.BusinessRulesTests.clauses {
    [Collection("SharedCollection")]

    public class clause66Test {

        private static readonly string _workflowName = "clause66_C_4_and_5";
        private static readonly string _category = "credit";
        private static readonly int _year = 2022;
        private readonly WorkflowsFixture _fixture;

        public clause66Test(WorkflowsFixture worflowsFixture) {

            _fixture = worflowsFixture;
        }

        [Theory]
        [InlineData(true, "0000000000000", 0)]  // Woman, without children
        [InlineData(true, "0000000000001", -4014)]  // Woman, 1 children aged 0-1
        [InlineData(true, "0000000000101", -10704)]  // Woman, 1 children aged 0-1, 1 children aged 1-5
        [InlineData(true, "0000000010101", -16056)]  // Woman, 1 children aged 0-1, 1 children aged 1-5, 1 children aged 6-12
        [InlineData(true, "0000001010101", -18732)]  // Woman, 1 children aged 0-1, 1 children aged 1-5, 1 children aged 6-12, 1 children aged 13-17
        [InlineData(true, "0000101010101", -20070)]  // Woman, 1 children aged 0-1, 1 children aged 1-5, 1 children aged 6-12, 1 children aged 13-17, 1 children aged 18
        [InlineData(false, "0000000000000", 0)]  // // Man (not a single parent), without children
        [InlineData(false, "0000000000001", -4014)]  // Man (not a single parent), 1 children aged 0-1
        [InlineData(false, "0000000000101", -10704)]  // Man (not a single parent), 1 children aged 0-1, 1 children aged 1-5
        [InlineData(false, "0000000010101", -13380)]  // Man (not a single parent), 1 children aged 0-1, 1 children aged 1-5, 1 children aged 6-12
        [InlineData(false, "0000001010101", -13380)]  // Man (not a single parent), 1 children aged 0-1, 1 children aged 1-5, 1 children aged 6-12, 1 children aged 13-17
        [InlineData(false, "0000101010101", -13380)]  // Man (not a single parent), 1 children aged 0-1, 1 children aged 1-5, 1 children aged 6-12, 1 children aged 13-17, 1 children aged 18
        public async Task WorkflowClauseAI66ChildernTestAsync(bool isWoman, string childrensData, double expected) {
            // Arrange
            var workFlowInputParams = new CalcTaxRequest
            {
                IsWoman = isWoman,
                childrensData = childrensData

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

