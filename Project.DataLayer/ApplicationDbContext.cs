using Project.Core.DataLayer.Entities;

namespace Project.Core.DataLayer;


public interface IApplicationDbContext
{
    Task<bool> Init(CancellationToken token);

    public DbSet<Todo> Todos { get; }
}

internal class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public required DbSet<Todo> Todos { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public async Task<bool> Init(CancellationToken token)
    {
        try
        {
            // bool created = await Database.EnsureCreatedAsync(token);
            // if (created)
            // {
            await Database.MigrateAsync();
            // }
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder
    //            .UseSnakeCaseNamingConvention();
}
