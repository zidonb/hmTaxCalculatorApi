using System.Text;

namespace TaxCalculator.Infrastructure.Services {
    public class WorkflowLoadResult {
        public bool HasErrors => Errors.Count != 0;
        public List<string> Errors { get; } = new List<string>();
        public StringBuilder Logs { get; } = new StringBuilder();
    }
}
