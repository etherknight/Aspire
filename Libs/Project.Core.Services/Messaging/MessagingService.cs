using Microsoft.Extensions.Logging;
using Project.Core.Services.Interfaces.Messaging;
using Rebus.Bus;

namespace Project.Core.Services.Messaging;

public class MessagingService(IBus messageBus, ILogger<MessagingService> logger) : IMessagingService {
    private readonly IBus _messageBus = messageBus;
    private readonly ILogger<MessagingService> _logger = logger;

    public async Task Send<TMsg>(TMsg message) {
        try {
            await _messageBus.Send(message);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Sending message of type {MessageType} failed with error {ErrorMessage}", 
                typeof(TMsg), ex.Message);
        }
    }
}