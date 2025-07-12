# Rad Log Viewer

A cross-platform desktop application for viewing and analyzing structured JSON log files.

## Features

- **JSON Log Parsing**: Efficiently parse and display structured JSON log files
- **Cross-Platform**: Built with Avalonia UI, runs on Windows, Linux, and macOS
- **Advanced Filtering**: Filter logs by:
  - Text content (full-text search)
  - Log level (INFO, WARN, ERROR, etc.)
  - Date/time
  - Custom attributes
- **Real-time Search**: Instantly filter large log files as you type
- **Export to CSV**: Export filtered log data to CSV format for further analysis
- **Distributed Tracing Support**: View and filter by trace and span IDs
- **Structured Data Visualization**: Easily browse complex nested attributes

## Getting Started

### Download Pre-built Releases

Download the latest release for your platform:
- **Windows**: Download `rad-log-viewer-vX.X.X-win-x64.zip` from [Releases](https://github.com/behrouz-rad/hcp-log-viewer/releases)
- **Linux**: Download `rad-log-viewer-vX.X.X-linux-x64.tar.gz` from [Releases](https://github.com/behrouz-rad/hcp-log-viewer/releases)

Extract and run the executable directly - no installation required.

### Try with Sample Data

A sample log file is included in the repository at `sample_log.txt` to help you get started and test the application features.

### Building from Source

**Prerequisites**: .NET 9.0 SDK or later

```bash
git clone https://github.com/behrouz-rad/hcp-log-viewer.git rad-log-viewer
cd rad-log-viewer
dotnet build
dotnet run --project src/Rad.LogViewer.App/Rad.LogViewer.App.csproj
```

## Log File Format

Rad Log Viewer expects JSON-formatted log files with each log entry on a separate line. The expected format is:

```json
{"time":"2023-06-15T14:22:10.123Z","level":"INFO","message":"Application started","traceId":"abc123","spanId":"span456","attributes":{"userId":"user123","action":"login"}}
```

Required fields:
- `time`: ISO 8601 timestamp
- `level`: Log level string
- `message`: Log message text

Optional fields:
- `traceId`: Distributed tracing ID
- `spanId`: Span ID for distributed tracing
- `attributes`: Object containing additional structured data

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Built with [Avalonia UI](https://avaloniaui.net/)
- Uses [ReactiveUI](https://www.reactiveui.net/) for reactive programming
