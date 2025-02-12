namespace TaxCalculator.Infrastructure.Repositories;

public abstract class GenericBaseRepository<TContext, T> : IGenericBaseRepository<TContext, T> where TContext : BaseContext<TContext> where T : class {
    #region Protected members

    protected readonly IDbConnection _connection;

    #endregion

    #region Ctor

    public GenericBaseRepository(TContext context) => _connection = context.Connection;

    #endregion

    #region Public methods

    #endregion
}