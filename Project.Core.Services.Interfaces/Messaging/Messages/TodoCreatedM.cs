namespace Project.Core.Services.Interfaces.Messaging.Messages;

[IpcMessage(MessagingDestinationE.Worker)]
public sealed record TodoCreatedM(int TodoId) : IIpcMessage;