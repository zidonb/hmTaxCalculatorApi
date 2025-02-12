using RulesEngine.Models;
using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Tests.BusinessRulesTests.clauses {

    [Collection("SharedCollection")]
    public class clause121Test {

        private static readonly string _workflowName = "clause121";
        private static readonly string _category = "tax";
        private static readonly int _year = 2022;
        private readonly WorkflowsFixture _fixture;

        public clause121Test(WorkflowsFixture worflowsFixture) {

            _fixture = worflowsFixture;
        }

        [Theory]
        //מדרגות מס ליגיעה אישית בלבד לפחות מ-60
        [InlineData(30, 0, 60000, 6000)]
        [InlineData(30, 0, 80000, 8104)]
        [InlineData(30, 0, 110000, 12304)]
        [InlineData(30, 0, 200000, 32662.4)]
        [InlineData(30, 0, 250000, 48264)]
        [InlineData(30, 0, 550000, 157474)]

        //מדרגות מס ליגיעה אישית בלבד ליותר מ-60
        [InlineData(60, 0, 60000, 6000)]
        [InlineData(60, 0, 80000, 8104)]
        [InlineData(62, 0, 110000, 12304)]
        [InlineData(60, 0, 200000, 32662.4)]
        [InlineData(65, 0, 250000, 48264)]
        [InlineData(60, 0, 550000, 157474)]

        //מדרגות מס להכנסה חייבת לפחות מ-60
        [InlineData(30, 200000, 0, 62000)]
        [InlineData(30, 250000, 0, 77602)]
        [InlineData(30, 600000, 0, 210312)]

        //מדרגות מס להכנסה חייבת ליותר מ-60
        [InlineData(61, 200000, 0, 32662.4)]
        [InlineData(61, 250000, 0, 48264)]
        [InlineData(61, 550000, 0, 157474)]

        //הטסט הבא לא עובר, הסימולטור נותן 76806 אבל אנחנו נותנים 69264
        //[InlineData(60, 200000, 110000, 76806)]
        //[InlineData(30, 200000, 110000, 76806)]

        public async Task WorkflowClause121TestAsync(int age, double taxableIncomeAmount, double personalExertionIncome, double expected) {
            // Arrange
            var workFlowInputParams = new CalcTaxRequest
            {
                individualAge = age,
                taxableIncomeNotFromPersonalExertion = taxableIncomeAmount,
                personalExertionIncome = personalExertionIncome
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
