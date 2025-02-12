using RulesEngine.Models;
using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Tests.BusinessRulesTests.clauses {

    [Collection("SharedCollection")]
    public class clause34Test {

        private static readonly string _workflowName = "clause34";
        private static readonly string _category = "credit";
        private static readonly int _year = 2022;
        private readonly WorkflowsFixture _fixture;

        public clause34Test(WorkflowsFixture worflowsFixture) {

            _fixture = worflowsFixture;
        }


        [Theory]
        //תושב ישראל מקבל שני נקודות זיכוי
        [InlineData(true, true, -5352)]
        [InlineData(true, false, 0)]
        [InlineData(false, true, 0)]
        [InlineData(false, false, 0)]

        public async Task WorkflowClause34TestAsync(bool isIndividual, bool isIsraeliResident, double expected) {
            // Arrange
            var workFlowInputParams = new CalcTaxRequest
            {
                individual = isIndividual,
                israeliResident = isIsraeliResident
            };

            var rulesParam = new RuleParameter("Input1", workFlowInputParams);
            var inputs = new[] { rulesParam };

            var engine = _fixture._rulesEngine.GetRuleEngine(_year, _category);

            // Act
            var result = await engine.ExecuteAllRulesAsync(_workflowName, inputs);


            // Assert
            Assert.Equal(expected, _fixture._workFlowsService.GetFinalResult(result), 1d);
        }
    }
}

//taxableIncomeAmount = שאינה מיגיעה אישית
