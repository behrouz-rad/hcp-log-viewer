// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Hcp.LogViewer.App.ViewModels;
using ReactiveUI;

namespace Hcp.LogViewer.App.Commands.Implementations;

/// <summary>
/// Command for copying a log entry to the clipboard
/// </summary>
internal class CopyLogEntryCommand : CommandBase<LogEntryViewModel, Unit>
{
    public override string Name => "CopyLogEntry";

    public override ReactiveCommand<LogEntryViewModel, Unit> Command { get; }

    public CopyLogEntryCommand()
    {
        Command = ReactiveCommand.CreateFromTask<LogEntryViewModel>(ExecuteAsync);
    }

    private static string FormatLogEntry(LogEntryViewModel logEntry)
    {
        var builder = new System.Text.StringBuilder();

        builder.AppendLine($"Time: {logEntry.Time:yyyy-MM-dd HH:mm:ss.fff zzz}");

        builder.AppendLine($"Level: {logEntry.Level}");

        builder.AppendLine($"Message: {logEntry.Message}");

        if (!string.IsNullOrWhiteSpace(logEntry.TraceId))
        {
            builder.AppendLine($"TraceId: {logEntry.TraceId}");
        }

        if (!string.IsNullOrWhiteSpace(logEntry.SpanId))
        {
            builder.AppendLine($"SpanId: {logEntry.SpanId}");
        }

        if (logEntry.Attributes is not null && logEntry.Attributes.Any())
        {
            builder.AppendLine("Attributes:");
            foreach (var attr in logEntry.Attributes)
            {
                builder.AppendLine($"  {attr.Key}: {attr.Value}");
            }
        }

        return builder.ToString();
    }

    private async Task ExecuteAsync(LogEntryViewModel logEntry)
    {
        if (logEntry is null)
        {
            return;
        }

        var desktop = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        var formattedEntry = FormatLogEntry(logEntry);

        var topLevel = TopLevel.GetTopLevel(desktop!.MainWindow);
        if (topLevel != null)
        {
            await topLevel.Clipboard!.SetTextAsync(formattedEntry);
        }
    }
}
