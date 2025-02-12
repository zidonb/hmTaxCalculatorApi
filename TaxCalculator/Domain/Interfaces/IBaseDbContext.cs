namespace TaxCalculator.Domain.Interfaces;

public interface IBaseDbContext {
    IDbConnection Connection { get; }
}
