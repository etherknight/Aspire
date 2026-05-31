using System.Data.Common;
using Project.Core.DataLayer.Entities;
using Project.Shared.Interfaces;

namespace Project.Core.DataLayer;


public interface IApplicationDbContext {
    Task<Option<bool>> SaveChangesAsync(CancellationToken token);
    Task<Option<bool>> Init(CancellationToken token);

    public DbSet<Todo> Todos { get; }
    public DbSet<Contact> Contacts { get; }
    public DbSet<CustomFieldDefinition> CustomFieldDefinitions { get; }
    public DbSet<CustomFieldValue> CustomFieldValues { get; }
}

internal class ApplicationDbContext : DbContext, IApplicationDbContext {

    public required DbSet<Todo> Todos { get; set; }
    public required DbSet<Contact> Contacts { get; set; }
    public required DbSet<CustomFieldDefinition> CustomFieldDefinitions { get; set; }
    public required DbSet<CustomFieldValue> CustomFieldValues { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Contact ↔ CustomFieldValue many-to-many via an explicit join table.
        // For each future entity that needs custom fields, add a similar HasMany/WithMany
        // block here — EF will create a separate join table (e.g. lead_custom_field_value)
        // with its own FK constraints, so CustomFieldValue stays free of nullable FK columns.
        modelBuilder.Entity<Contact>()
            .HasMany(c => c.CustomFieldValues)
            .WithMany()
            .UsingEntity(j => j.ToTable("contact_custom_field_value"));
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
}
