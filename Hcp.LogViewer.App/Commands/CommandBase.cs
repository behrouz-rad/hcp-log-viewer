// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using ReactiveUI;

namespace Hcp.LogViewer.App.Commands;

/// <summary>
/// Base class for all commands in the application
/// </summary>
internal abstract class CommandBase
{
    /// <summary>
    /// Gets the name of the command
    /// </summary>
    public abstract string Name { get; }
}

/// <summary>
/// Base class for commands that don't require parameters
/// </summary>
/// <typeparam name="TResult">The type of the result returned by the command</typeparam>
internal abstract class CommandBase<TResult> : CommandBase
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
internal abstract class CommandBase<TParam, TResult> : CommandBase
{
    /// <summary>
    /// Gets the ReactiveCommand instance
    /// </summary>
    public abstract ReactiveCommand<TParam, TResult> Command { get; }
}
