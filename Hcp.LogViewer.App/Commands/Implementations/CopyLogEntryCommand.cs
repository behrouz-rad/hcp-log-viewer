// © 2025 Behrouz Rad. All rights reserved.

using System.Globalization;
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
internal class CopyLogEntryCommand : ICommandBase<LogEntryViewModel, Unit>
{
    public string Name => "CopyLogEntry";

    public ReactiveCommand<LogEntryViewModel, Unit> Command { get; }

    public CopyLogEntryCommand()
    {
        Command = ReactiveCommand.CreateFromTask<LogEntryViewModel>(ExecuteAsync);
    }

    private static string FormatLogEntry(LogEntryViewModel logEntry)
    {
        var builder = new System.Text.StringBuilder();

        builder.AppendLine(CultureInfo.InvariantCulture, $"Time: {logEntry.Time:yyyy-MM-dd HH:mm:ss.fff zzz}");

        builder.AppendLine(CultureInfo.InvariantCulture, $"Level: {logEntry.Level}");

        builder.AppendLine(CultureInfo.InvariantCulture, $"Message: {logEntry.Message}");

        if (!string.IsNullOrWhiteSpace(logEntry.TraceId))
        {
            builder.AppendLine(CultureInfo.InvariantCulture, $"TraceId: {logEntry.TraceId}");
        }

        if (!string.IsNullOrWhiteSpace(logEntry.SpanId))
        {
            builder.AppendLine(CultureInfo.InvariantCulture, $"SpanId: {logEntry.SpanId}");
        }

        if (logEntry.Attributes?.Any() == true)
        {
            builder.AppendLine("Attributes:");
            foreach (var attr in logEntry.Attributes)
            {
                builder.AppendLine(CultureInfo.InvariantCulture, $"  {attr.Key}: {attr.Value}");
            }
        }

        return builder.ToString();
    }

    private static async Task ExecuteAsync(LogEntryViewModel logEntry)
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
