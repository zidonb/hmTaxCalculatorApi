namespace TaxCalculator.Infrastructure.DbContexts;

public class SecondDbContext : BaseContext<SecondDbContext> {
    public DbSet<Sample> Samples { get; set; }

    public SecondDbContext(DbContextOptions<SecondDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken()) {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
