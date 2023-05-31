using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AtraBase.Models.Result;

/// <summary>
/// Based off of Rust's Result enum.
/// </summary>
/// <typeparam name="TSuccess">Type of the success condition.</typeparam>
/// <typeparam name="TError">Type of the error condition.</typeparam>
public struct Result<TSuccess, TError>
    where TError : Exception
{
    private readonly TSuccess? succcess;
    private readonly TError? error;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TSuccess, TError}"/> struct.
    /// </summary>
    /// <param name="succcess">The success value.</param>
    public Result(TSuccess succcess)
        => this.succcess = succcess;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TSuccess, TError}"/> struct.
    /// </summary>
    /// <param name="error">The error value.</param>
    public Result(TError error)
        => this.error = error;

    /// <summary>
    /// Gets the success value, if applicable. Null otherwise.
    /// </summary>
    public TSuccess? Success => this.succcess;

    /// <summary>
    /// Gets the error value, if applicable. Null otherwise.
    /// </summary>
    public TError? Error => this.error;

    /// <summary>
    /// Gets the success value if no error exists. Throws the error otherwise.
    /// </summary>
    /// <returns>success value.</returns>
    /// <exception cref="TError">If an error exists, it will be thrown.</exception>
    public TSuccess Unwrap()
    {
        if (this.error is not null)
        {
            Throw(this.error);
        }
        return this.succcess!;
    }

    /// <summary>
    /// Gets the success value if no error exists, or uses the default value otherwise.
    /// </summary>
    /// <param name="default">Default value.</param>
    /// <returns>success or default.</returns>
    public TSuccess Unwrap_Or(TSuccess @default) =>
        this.error is null ? this.succcess! : @default;

    /// <summary>
    /// Gets the success value if no error exists, or uses the default for the type.
    /// </summary>
    /// <returns>success or default.</returns>
    public TSuccess? Unwrap_Or_Default()
        => this.error is null ? this.succcess! : default;

    /// <summary>
    /// Gets the success value if no error exists, or runs the delegate to get a value.
    /// </summary>
    /// <param name="del">Delegate that provides a default value.</param>
    /// <returns>success or result of delegate.</returns>
    public TSuccess Unwrap_Or_Else(Func<TSuccess> del) =>
        this.error is null ? this.succcess! : del();

    /// <summary>
    /// If true, this represents a success value.
    /// </summary>
    /// <returns>True if error is not set.</returns>
    public bool IsSuccess() => this.error is null;

    /// <summary>
    /// If true, this represents an error value.
    /// </summary>
    /// <returns>True if error is set.</returns>
    public bool IsError() => this.error is not null;

    /// <summary>
    /// Throws an exception.
    /// </summary>
    /// <param name="exception">Exception to throw.</param>
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Throw(Exception exception)
    {
        throw exception;
    }
}
