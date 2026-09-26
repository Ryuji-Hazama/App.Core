using App.Core.Consts;

namespace App.Core.LoggerFactory.Containers;

public class ConfigurationFile
{
    public List<Output> Outputs { get; set; } = new List<Output>();

    public List<NameSpace> NameSpaces { get; set; } = new List<NameSpace>();

    public class Output
    {
        public object Type { get; set; } = "Unset";
        public string LogFileName { get; set; } = LoggerFactoryConsts.DEFAULT_LOG_FILE_NAME;
        public string LogFilePath { get; set; } = LoggerFactoryConsts.DEFAULT_LOG_FILE_PATH;
        public object Mode { get; set; } = LogFileMode.Append.ToString();
        public object MinLogLevel { get; set; } = LogLevel.TRACE.ToString();
        public object MaxLogLevel { get; set; } = LogLevel.FATAL.ToString();
        public object MaxFileSize { get; set; } = 0;

        // If the Type is int, it represents the maximum file size in bytes.
        // If the Type is string, it can be a human-readable format like "10MB", "1GB", etc.

        /* public int FileMaxCount { get; set; } = 0; */
        public string? Format { get; set; }
        // ${pid} -> Process ID
        // ${timestamp} -> Current timestamp
        // ${level} -> Log level
        // ${class_path} -> Full class path including namespace
        // ${caller} -> The method or function that called the logger.
        // ${line} -> The line number in the source code where the log was generated.
        // ${message} -> The log message content
        public string? TimestampFormat { get; set; }
        // Use standard .NET date and time format strings for the timestamp format.
        public bool ColorHighlight { get; set; } = true;
    }

    public class NameSpace
    {
        public string Name { get; set; } = string.Empty;
        public object MinLogLevel { get; set; } = LogLevel.TRACE.ToString();
        public object MaxLogLevel { get; set; } = LogLevel.FATAL.ToString();
    }
}

public class OutputConfig
{
    public bool IsConsole { get; set; } = false;
    public bool ColorHighlight { get; set; } = true;
    public string LogFileName { get; set; } = string.Empty;
    public string LogFilePath { get; set; } = string.Empty;
    public LogFileMode Mode { get; set; } = LogFileMode.Append;
    public LogLevel MinLogLevel { get; set; } = LogLevel.TRACE;
    public LogLevel MaxLogLevel { get; set; } = LogLevel.FATAL;
    public string Format { get; set; } = LoggerFactoryConsts.DEFAULT_FILE_LOG_FORMAT;
    public string TimestampFormat { get; set; } = LoggerFactoryConsts.DEFAULT_DATETIME_FORMAT;
    public long MaxFileSize { get; set; } = 0;
}

public class NameSpace
{
    public string Name { get; set; } = string.Empty;
    public LogLevel MinLogLevel { get; set; } = LogLevel.TRACE;
    public LogLevel MaxLogLevel { get; set; } = LogLevel.FATAL;
}