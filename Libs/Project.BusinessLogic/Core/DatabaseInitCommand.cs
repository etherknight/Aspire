using MediatR;
using Microsoft.Extensions.Logging;
using Project.Core.DataLayer;

namespace Project.BusinessLogic.Core;

public sealed record DatabaseInitCommand : IRequest { }


internal class DatabaseInitCommandHandler(
    IApplicationDbContext applicationDb,
    ILogger<DatabaseInitCommandHandler> logger)
    : IRequestHandler<DatabaseInitCommand>
{
    public async Task Handle(DatabaseInitCommand request, CancellationToken cancellationToken) {
        
         await applicationDb.Init(cancellationToken)
                       .Finally
                        (
                            _ => logger.LogInformation("Database initialized."),
                            error => logger.LogError(error.Exception,"Database initialization failed.")
                        );
    }
}