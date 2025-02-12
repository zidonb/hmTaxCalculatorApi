namespace TaxCalculator.Infrastructure.DbContexts;

public abstract class BaseContext<TDbContext> : DbContext, IBaseDbContext where TDbContext : DbContext {
    public IDbConnection Connection => Database.GetDbConnection();

    public BaseContext(DbContextOptions<TDbContext> options) : base(options) { }

}
