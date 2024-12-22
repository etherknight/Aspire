using System.Diagnostics;

namespace Project.Shared.Interfaces;

public class Option<TObject> {
    private readonly TObject _value;
    private readonly OptionError _error;

    private bool IsSuccess() => Equals(_error, OptionError.Default);
    
    // Private constructors. 
    #region Constructors
    private Option(TObject value)
    {
        _error = OptionError.Default;
        _value = value;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8601 // Possible null reference assignment.
    private Option(OptionError error)
    {
        _error = error;
        _value = default;
    }
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion
    
    /// Methods that create an Option
    /// these will all invoke the private constructors. 
    #region Initialisers
    [DebuggerStepThrough]
    public static implicit operator Option<TObject>(OptionError error) 
        => new(error);
    
    [DebuggerStepThrough]
    public static implicit operator Option<TObject>(TObject value) 
        => new(value);
    
    [DebuggerStepThrough]
    public static Option<TObject> Ok() 
        => new(OptionError.Default);
    
    [DebuggerStepThrough]
    public static Option<TObject> Ok(TObject value) 
        => new(value);
    
    [DebuggerStepThrough]
    public static Option<TObject> Fail<TCurrent>(Option<TCurrent> result)
        => new(result._error);
    
    #endregion
    
    // Chaining methods.
    #region Then() Methods
    [DebuggerStepThrough]
    public Option<TOut> Then<TOut>(Func<TObject, Option<TOut>> next)
        => IsSuccess() switch {
            true => next(_value),
            false => _error
        };
    
    [DebuggerStepThrough]
    public async Task<Option<TOut>> Then<TOut>(Func<TObject, Task<Option<TOut>>> next)
        => IsSuccess() switch {
            true => await next(_value),
            false => _error
        };
    #endregion
    
    #region Finally() resolution methods
    [DebuggerStepThrough]
    public TResult Finally<TResult>(Func<TObject, TResult> some, Func<OptionError, TResult> none)
        => IsSuccess() switch {
            true => some(_value),
            false => none(_error)
        };
    
    [DebuggerStepThrough]
    public async Task<TResult> Finally<TResult>(Func<TObject, Task<TResult>> some, Func<OptionError, Task<TResult>> none)
        => IsSuccess() switch {
            true => await some(_value),
            false => await none(_error)
        };
    
    [DebuggerStepThrough]
    public void Finally(Action<TObject> some, Action<OptionError> none)
    {
        if (IsSuccess())
        {
            some(_value);
        }
        else
        {
            none(_error);
        }
    }
    
    // public async Task<TResult> Finally<TResult>(Func<TObject, Task<TResult>> some, Func<OptionError, TResult> none)
    //     => IsSuccess() switch
    //     {
    //         true => await some(_value),
    //         false => none(_error)
    //     };
    //
    //
    // [DebuggerStepThrough]
    // public async Task<TResult> Finally<TResult>(Func<TObject, TResult> some, Func<OptionError, Task<TResult>> none)
    //     => IsSuccess() switch
    //     {
    //         true => some(_value),
    //         false => await none(_error)
    //     };
    //
    // [DebuggerStepThrough]
    // public async Task Finally(Func<TObject, Task> some, Func<OptionError, Task> none)
    // {
    //     if (IsSuccess())
    //     {
    //         await some(_value);
    //     }
    //     else
    //     {
    //         await none(_error);
    //     }
    // }
    #endregion
}

public static class OptionAsyncExtensions
{
    #region Finally (Func)
    [DebuggerStepThrough]
    public static async Task<TResult> Finally<TData, TResult>
    (
        this Task<Option<TData>> option,
        Func<TData, Task<TResult>> some,
        Func<OptionError, Task<TResult>> none
    ) {
        Option<TData> result = await option;
        return await result.Finally(some, none);
    }
    
    [DebuggerStepThrough]
    public static async Task<TResult> Finally<TData, TResult>
    (
        this Task<Option<TData>> option,
        Func<TData, TResult> some,
        Func<OptionError, TResult> none
    ) {
        Option<TData> result = await option;
        return result.Finally(some, none);
    }
    
    // [DebuggerStepThrough]
    // public static async Task<TResult> Finally<TData, TResult>(
    //     this Task<Option<TData>> option,
    //     Func<TData, Task<TResult>> some,
    //     Func<OptionError, TResult> none
    // ) {
    //     Option<TData> result = await option;
    //     return await result.Finally(some, none);
    // }
    //
    // [DebuggerStepThrough]
    // public static async Task<TResult> Finally<TData, TResult>(
    //     this Task<Option<TData>> option,
    //     Func<TData, TResult> some, 
    //     Func<OptionError, Task<TResult>> none
    // ) {
    //     Option<TData> result = await option;
    //     return await result.Finally(some, none);
    // }
    #endregion
    
    #region Finally (Action)
    [DebuggerStepThrough]
    public static async Task<Option<TData>> Finally<TData>(
        this Task<Option<TData>> option, 
        Action<TData> some,  
        Action<OptionError> none
    ) {
        Option<TData> result = await option;
        result.Finally(some, none);
        return result;
    }
    #endregion
}