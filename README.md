# HCP Log Viewer

A cross-platform desktop application for viewing and analyzing structured JSON log files.

<!-- ![HCP Log Viewer](https://via.placeholder.com/800x450.png?text=HCP+Log+Viewer+Screenshot) -->

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

### Prerequisites

- .NET 9.0 SDK or later

### Installation

#### Windows

1. Download the latest release from the [Releases](https://github.com/behrouz-rad/hcp-log-viewer/releases) page
2. Run the installer or extract the zip file
3. Launch `Hcp.LogViewer.App.exe`

<!-- #### Linux

```bash
# Download the AppImage or use the tar.gz
chmod +x HcpLogViewer.AppImage
./HcpLogViewer.AppImage
```

#### macOS

1. Download the .dmg file from the [Releases](https://github.com/behrouz-rad/hcp-log-viewer/releases) page
2. Open the .dmg file and drag the application to your Applications folder
3. Launch HCP Log Viewer from your Applications -->

### Building from Source

```bash
git clone https://github.com/yourusername/hcp-log-viewer.git
cd hcp-log-viewer
dotnet build
dotnet run --project Hcp.LogViewer.App
```

## Log File Format

HCP Log Viewer expects JSON-formatted log files with each log entry on a separate line. The expected format is:

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

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Built with [Avalonia UI](https://avaloniaui.net/)
- Uses [ReactiveUI](https://www.reactiveui.net/) for reactive programming