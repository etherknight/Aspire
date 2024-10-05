using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.Core.DataLayer;

namespace Project.BusinessLogic.Core;

public sealed record DatabaseInitCommand : IRequest { }


internal class DatabaseInitCommandHandler : IRequestHandler<DatabaseInitCommand>
{
    private readonly IApplicationDbContext _database;
    private readonly ILogger<DatabaseInitCommandHandler> _logger;

    public DatabaseInitCommandHandler(IApplicationDbContext applicationDb, ILogger<DatabaseInitCommandHandler> logger)
    {
        _database = applicationDb;
        _logger = logger;
    }

    public async Task Handle(DatabaseInitCommand request, CancellationToken cancellationToken) {
        bool initialised = await _database.Init(cancellationToken);
        
        if (!initialised) { 
            _logger.LogError("Failed to connect to database");
        }        
    }
}