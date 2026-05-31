namespace Project.Core.DataLayer.Entities.Interfaces;

/// <summary>
/// Interface for all common entities
/// </summary>
public interface IEntity {
    /// <summary>
    /// A v7 GUID with sortable date times
    /// </summary>
    public Guid Id { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset ModifiedAt { get; set; }
}

/// <summary>
/// Base class for all entities, provides core fields common across all
/// </summary>
public class Entity : IEntity {
    /// <inheritdoc />
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    /// <inheritdoc/>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <inheritdoc/>
    public DateTimeOffset ModifiedAt { get; set; }
}