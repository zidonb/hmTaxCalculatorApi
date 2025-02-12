using RulesEngine.Models;
using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Tests.BusinessRulesTests.clauses {

    [Collection("SharedCollection")]
    public class clause39ATest {

        private static readonly string _workflowName = "clause39A";
        private static readonly string _category = "credit";
        private static readonly int _year = 2022;
        private readonly WorkflowsFixture _fixture;

        public clause39ATest(WorkflowsFixture worflowsFixture) {

            _fixture = worflowsFixture;
        }

        [Theory]
        [InlineData(true, 20, "112022", -223)]
        [InlineData(true, 25, "112022", -446)]
        [InlineData(false, 20, "112022", -223)]
        [InlineData(false, 25, "112022", -446)]
        [InlineData(false, 25, "112019", -4906)]
        [InlineData(true, 20, "112020", -2676)]
        [InlineData(true, 30, "022021", -5352)]
        [InlineData(true, 25, "012018", 0)]
        [InlineData(true, 25, "012019", -446)]
        public async Task WorkflowClause39ASoldierTestAsync(bool isWoman, double regularServiceMonths, string endOfMilitaryServiceDate, double expected) {
            // Arrange
            var workFlowInputParams = new CalcTaxRequest
            {
                IsWoman = isWoman,
                regularServiceMonths = regularServiceMonths,
                EndOfMilitaryServiceDate = endOfMilitaryServiceDate

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
