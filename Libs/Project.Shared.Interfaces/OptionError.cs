using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Project.Shared.Interfaces;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum OptionErrorCodeE
{
    /// <summary>
    /// No error, default state for an OptionError.
    /// </summary>
    NONE,
    
    /// <summary>
    /// Unspecified exception.
    /// </summary>
    EXCEPTION,
    
    /// <summary>
    /// Used when there are multiple errors being returned.
    /// Check the <see cref="OptionError.Errors"/> property for each error. 
    /// </summary>
    /// <remarks>
    ///     Primarily useful when returning validation errors, and you want to
    /// aggregate the results into one error response. 
    /// </remarks>
    MANY,
    
    /// <summary>
    /// Return if there called code has not been implemented yet.  
    /// </summary>
    NOT_IMPLEMENTED,
    
    /// <summary>
    /// Initial error state for a method, the code has
    /// not been elevated to a success or specific failure.
    /// </summary>
    NOT_RUN,
    
    /// <summary>
    /// A passed parameter was invalid, check message for details
    /// </summary>
    INVALID_PARAMETER,
    
    /// <summary>
    /// A condition was not met to allow the code to condition.
    /// </summary>
    INVALID_CONDITION
}

/// <summary>
/// Class for sending error information with responses.
/// </summary>
[method: DebuggerStepThrough]
public sealed class OptionError(OptionErrorCodeE code, string message)
{
    public static OptionError Default => new(OptionErrorCodeE.NONE, string.Empty);
    
    public static OptionError NotImplemented => new(OptionErrorCodeE.NOT_IMPLEMENTED, "Not implemented");
    
    public static OptionError NotComplete => new(OptionErrorCodeE.NOT_RUN, "Unset error, method incomplete");

    public static OptionError GuardError(string condition, string? message) => new(condition, message);
    public static OptionError FromException(Exception ex, OptionErrorCodeE code = OptionErrorCodeE.EXCEPTION) => new(code, ex.Message);
    
    /// <summary>
    /// The error code.
    /// </summary>
    public OptionErrorCodeE Code { get; init;  } = code;

    /// <summary>
    /// A non-localised error message, localised messages should use the error <see cref="CodeE"/>
    /// </summary>
    public string Message { get; init; } = message;

    /// <summary>
    /// Returns multiple errors wrapped in a single error.
    /// </summary>
    public IReadOnlyCollection<OptionError> Errors { get; init; } = [];
    
    /// <summary>
    /// Optional Exception if thrown by the calling code.
    /// </summary>
    public Exception? Exception { get; init; }

    [DebuggerStepThrough]
    public OptionError(IEnumerable<OptionError> errors) 
        : this (OptionErrorCodeE.MANY, string.Empty)
    {
        Errors = errors.ToList().AsReadOnly();
    }
    
    [DebuggerStepThrough]
    public OptionError(OptionErrorCodeE code, string message, Exception exception) 
        : this (code, string.Empty)
    {
        Exception = exception;
    }
    
    public OptionError(string condition, string? message)
        : this (OptionErrorCodeE.INVALID_CONDITION, $"{condition}: ${message}")
    {}
    
    #region INTERFACE: IEquatable
    public override bool Equals(object? value)
    {
        return value is OptionError error &&
              Code == error.Code &&
              Message == error.Message &&
              Errors.Count == error.Errors.Count;
    }

    public override int GetHashCode()
        => HashCode.Combine(Code, Message);
    #endregion
}