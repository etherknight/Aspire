using System.Data.Common;
using Project.Core.DataLayer.Entities;
using Project.Shared.Interfaces;

namespace Project.Core.DataLayer;


public interface IApplicationDbContext {
    Task<Option<bool>> SaveChangesAsync(CancellationToken token);
    Task<Option<bool>> Init(CancellationToken token);

    public DbSet<Todo> Todos { get; }
}

internal class ApplicationDbContext : DbContext, IApplicationDbContext {
    
    public required DbSet<Todo> Todos { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {
    }

    public new async Task<Option<bool>> SaveChangesAsync(CancellationToken token) {
        Option<bool> saved = false;
        try {
            await base.SaveChangesAsync(token);
            saved = true;
        }
        catch (DbException ex) {
            saved = OptionError.FromException(ex);
        }
        return saved;
    }

    public async Task<Option<bool>> Init(CancellationToken token) {
        Option<bool> result = OptionError.NotComplete;
        try {
            await Database.MigrateAsync(cancellationToken: token);
            result = true;
        }
        catch (Exception ex) {
            result = OptionError.FromException(ex);
        }

        return result;
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder
    //            .UseSnakeCaseNamingConvention();
}
