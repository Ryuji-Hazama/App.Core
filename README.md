# Application Core

This project contains logger and external component integrations for the application core.

## Features

- Simple logger
- External component integrations

## Logger

The logger provides simple logging capabilities for the application core. It supports different log levels and can be easily integrated with external components.

### Log Levels

- **Trace**: Detailed information, typically of interest only when diagnosing problems.
- **Debug**: Information useful for debugging the application.
- **Info**: General operational information about the application.
- **Warn**: Indications that something unexpected happened, or indicative of some problem in the near future.
- **Error**: Error events that might still allow the application to continue running.
- **Fatal**: Very severe error events that will presumably lead the application to abort.

### Logger Usage

To use the logger, you need to import LoggerFactory namespace first:

```csharp
using App.Core.LoggerFactory;
using System.Reflection;
```

then

```csharp
ILogger logger = Logger.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.ToString() ?? typeof(Program).ToString());

logger.Info("Logger initialized successfully.");
```

### Logger Configuration

If you want to use configuration file for logger, you also need to configure `App.json` for `App.Core` library.

Example `App.json` :

```json
{
    "Logger": {
        "LogConfig": "path/to/log/config/file.json"
    }
}
```

Then create the log configuration file at the specified path with the necessary settings for the logger.

```json
{
    "Outputs": [
        {
            "Type": "Console",
            "MinLogLevel": "WARN",
            "MaxLogLevel": "FATAL"
        },
        {
            "Type": "File",
            "LogFileName": "App.log",
            "LogFilePath": "logs",
            "Mode": "Append",
            "MinLogLevel": "TRACE",
            "MaxLogLevel": "FATAL",
            "MaxFileSize": "3MB"
        },
        {
            "Type": "File",
            "LogFileName": "Network.log",
            "LogFilePath": "smb://path/to/network/logs",
            "Mode": "Daily",
            "MinLogLevel": "INFO",
            "MaxLogLevel": "FATAL",
            "MaxFileSize": "3MB"
        }
    ],
    "NameSpaces": [
        {
            "Name": "App.TestBatch",
            "MinLogLevel": "TRACE",
            "MaxLogLevel": "FATAL"
        }
    ]
}
```