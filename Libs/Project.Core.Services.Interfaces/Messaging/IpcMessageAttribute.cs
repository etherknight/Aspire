namespace Project.Core.Services.Interfaces.Messaging;

/// <summary>
/// Locator interface for reflection.
/// </summary>
public interface IIpcMessage { }


[AttributeUsage(AttributeTargets.Class)]
public class IpcMessageAttribute(MessagingDestinationE destination) : Attribute
{
    public MessagingDestinationE Destination { get; } = destination;
}

