// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using ReactiveUI;

namespace Rad.LogViewer.App.Commands;

/// <summary>
/// Base class for all commands in the application
/// </summary>
internal interface ICommandBase
{
    /// <summary>
    /// Gets the name of the command
    /// </summary>
    public string Name { get; }
}

/// <summary>
/// Base class for commands that don't require parameters
/// </summary>
/// <typeparam name="TResult">The type of the result returned by the command</typeparam>
internal interface ICommandBase<TResult> : ICommandBase
{
    /// <summary>
    /// Gets the ReactiveCommand instance
    /// </summary>
    public abstract ReactiveCommand<Unit, TResult> Command { get; }
}

/// <summary>
/// Base class for commands that require parameters
/// </summary>
/// <typeparam name="TParam">The type of the parameter required by the command</typeparam>
/// <typeparam name="TResult">The type of the result returned by the command</typeparam>
internal interface ICommandBase<TParam, TResult> : ICommandBase
{
    /// <summary>
    /// Gets the ReactiveCommand instance
    /// </summary>
    public abstract ReactiveCommand<TParam, TResult> Command { get; }
}
