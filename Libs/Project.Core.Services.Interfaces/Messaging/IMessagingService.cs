namespace Project.Core.Services.Interfaces.Messaging;

public interface IMessagingService {
    Task Send<TMsg>(TMsg message);
}