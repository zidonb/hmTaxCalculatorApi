using RulesEngine.Models;
using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Tests.BusinessRulesTests.clauses {
    [Collection("SharedCollection")]

    public class clause44_45Test {

        private static readonly string _workflowName = "clause44_45";
        private static readonly string _category = "summary";
        private static readonly int _year = 2022;
        private readonly WorkflowsFixture _fixture;

        public clause44_45Test(WorkflowsFixture worflowsFixture) {

            _fixture = worflowsFixture;
        }

        [Theory]
        [InlineData(true, 1, 50000, 100000, -5352)]
        public async Task WorkflowClause44_45TestAsync(bool israeliResident, int numberOfChildrenWithDisabilities, double amountPaidForCare, double personalExertionIncome, double expected) {
            // Arrange
            var workFlowInputParams = new CalcTaxRequest
            {
                israeliResident = israeliResident,
                numberOfChildrenWithDisabilities = numberOfChildrenWithDisabilities,
                amountPaidForCare = amountPaidForCare,
                personalExertionIncome = personalExertionIncome
            };

            var rulesParam = new RuleParameter("Input1", workFlowInputParams);
            var inputs = new[] { rulesParam };

            var credit = await _fixture._workFlowsService.RunWorkflows(inputs, _year, "credit");


            _fixture._workFlowsService.GroupResults(_fixture._workFlowsService._groupedObject, _fixture._workFlowsService._resultObject, _year);

            // Assert
            Assert.True(_fixture._workFlowsService._groupedObject.Count > 0);
            Assert.True(_fixture._workFlowsService._groupedObject.ToList().TrueForAll(go => go.Value.Count > 0));
            Assert.True(_fixture._workFlowsService._groupedObject.First(o => o.Key == _workflowName).Value != null);
            Assert.True(_fixture._workFlowsService._groupedObject.First(o => o.Key == _workflowName).Value.Count == 2);
        }
    }
}

//taxableIncomeAmount = שאינה מיגיעה אישית
