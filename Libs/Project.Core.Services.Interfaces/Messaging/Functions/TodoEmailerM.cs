namespace Project.Core.Services.Interfaces.Messaging.Functions;

[IpcMessage(MessagingDestinationE.Function)]
public class TodoEmailerM : IIpcMessage {
    public required string Message { get; init; }
}