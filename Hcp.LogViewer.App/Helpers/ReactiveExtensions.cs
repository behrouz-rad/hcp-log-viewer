// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using System.Reactive.Linq;

namespace Hcp.LogViewer.App.Helpers;

/// <summary>
/// Extension methods for reactive programming.
/// </summary>
internal static class ReactiveExtensions
{
    /// <summary>
    /// Converts an IObservable&lt;Unit&gt; to a Task.
    /// </summary>
    /// <param name="observable">The observable to convert.</param>
    /// <returns>A task that completes when the observable completes.</returns>
    public static Task ToTask(this IObservable<Unit> observable)
    {
        if (observable == null)
        {
            return Task.CompletedTask;
        }

        var tcs = new TaskCompletionSource<Unit>();

        var subscription = observable.Subscribe(
            _ => { },
            ex => tcs.TrySetException(ex),
            () => tcs.TrySetResult(Unit.Default));

        return tcs.Task;
    }
}