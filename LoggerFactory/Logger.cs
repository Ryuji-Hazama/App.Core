using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace App.Core.LoggerFactory;

#region LogLevel Enum

/// <summary>
/// Represents the severity level of a log message.
/// TRACE, DEBUG, INFO, WARN, ERROR, FATAL, *ALL*, *NONE*
/// </summary>
public enum LogLevel
{
    NONE,
    TRACE,
    DEBUG,
    INFO,
    WARN,
    ERROR,
    FATAL,
    ALL
}

#endregion LogLevel Enum

#region Logger Class

public partial class Logger : ILogger
{
    #region Class Members

    private readonly string Source;
    public ILoggerConfig Config { get; set; } = new LoggerConfig();

    #endregion Class Members

    #region Constructor

    public Logger(string source)
    {
        Source = source;
    }

    #endregion Constructor

    #region Private Methods

    private void Log(LogLevel level, object message, int caller_depth = 2, int caller_line_number = 0)
    {
        StackFrame? caller_frame = new StackTrace().GetFrame(caller_depth);

        LogToConsole(level, caller_frame, caller_line_number, message);
        OutputToFile(level, caller_frame, caller_line_number, message);
    }

    private void LogError(LogLevel level, object message, Exception ex, int caller_line_number)
    {
        Log(level, message, 3, caller_line_number);
        Log(level, ex.ToString(), 3, caller_line_number);
    }

    private void LogToConsole(LogLevel level, StackFrame? caller_frame, int caller_line_number, object message)
    {
        string target_namespace = $"{Source}.{GetCallerMethodName(caller_frame)}";
        LogLevel console_min_log_level = Config.FindMinLogLevel(Config.ConsoleOutput.MinLogLevel, target_namespace);
        LogLevel console_max_log_level = Config.FindMaxLogLevel(Config.ConsoleOutput.MaxLogLevel, target_namespace);

        if (level >= console_min_log_level && level <= console_max_log_level)
        {
            string logMessage = FormatLogMessage(Config.ConsoleOutput, level, caller_frame, caller_line_number, message);
            Console.WriteLine(logMessage);
        }
    }

    private string GetCallerMethodName(StackFrame? caller_frame)
    {
        return caller_frame?.GetMethod()?.Name ?? "Unknown";
    }

    #endregion Private Methods

    #region Public Methods

    /// <summary>
    /// Logs a trace message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="line">The line number from which the log method was called.</param>
    public void Trace(object message, [CallerLineNumber] int line = 0) => Log(LogLevel.TRACE, message, caller_line_number: line);

    /// <summary>
    /// Logs a trace message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="ex">The exception to log.</param>
    /// <param name="line">The line number from which the log method was called.</param>
    public void Trace(object message, Exception ex, [CallerLineNumber] int line = 0) => LogError(LogLevel.TRACE, message, ex, line);

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="line">The line number from which the log method was called.</param>
    public void Debug(object message, [CallerLineNumber] int line = 0) => Log(LogLevel.DEBUG, message, caller_line_number: line);

    /// <summary>
    /// Logs a debug message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="ex">The exception to log.</param>
    /// <param name="line">The line number from which the log method was called.</param>
    public void Debug(object message, Exception ex, [CallerLineNumber] int line = 0) => LogError(LogLevel.DEBUG, message, ex, line);

    /// <summary>
    /// Logs an info message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="line">The line number from which the log method was called.</param>
    public void Info(object message, [CallerLineNumber] int line = 0) => Log(LogLevel.INFO, message, caller_line_number: line);

    /// <summary>
    /// Logs an info message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="ex">The exception to log.</param>
    /// <param name="line">The line number from which the log method was called.</param>
    public void Info(object message, Exception ex, [CallerLineNumber] int line = 0) => LogError(LogLevel.INFO, message, ex, line);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="line">The line number from which the log method was called.</param>
    public void Warn(object message, [CallerLineNumber] int line = 0) => Log(LogLevel.WARN, message, caller_line_number: line);

    /// <summary>
    /// Logs a warning message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="ex">The exception to log.</param>
    /// <param name="line">The line number from which the log method was called.</param>
    public void Warn(object message, Exception ex, [CallerLineNumber] int line = 0) => LogError(LogLevel.WARN, message, ex, line);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="line">The line number from which the log method was called.</param>
    public void Error(object message, [CallerLineNumber] int line = 0) => Log(LogLevel.ERROR, message, caller_line_number: line);

    /// <summary>
    /// Logs an error message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="ex">The exception to log.</param>
    /// <param name="line">The line number from which the log method was called.</param>
    public void Error(object message, Exception ex, [CallerLineNumber] int line = 0) => LogError(LogLevel.ERROR, message, ex, line);

    /// <summary>
    /// Logs a fatal message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="line">The line number from which the log method was called.</param>
    public void Fatal(object message, [CallerLineNumber] int line = 0) => Log(LogLevel.FATAL, message, caller_line_number: line);

    /// <summary>
    /// Logs a fatal message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="ex">The exception to log.</param>
    /// <param name="line">The line number from which the log method was called.</param>
    public void Fatal(object message, Exception ex, [CallerLineNumber] int line = 0) => LogError(LogLevel.FATAL, message, ex, line);

    #region Statics for Logger Instances
    // Static methods for managing logger instances

    /// <summary>
    /// Gets the dictionary of all logger instances.
    /// </summary>
    public static Dictionary<string, ILogger> Loggers { get; } = new Dictionary<string, ILogger>();

    /// <summary>
    /// Gets a logger instance by name. If the logger does not exist, it will be created.
    /// </summary>
    /// <param name="name">The name of the logger.</param>
    /// <returns>The logger instance.</returns>
    public static ILogger GetLogger(string name)
    {
        if (!Loggers.ContainsKey(name))
        {
            Loggers[name] = new Logger(name);
        }
        return Loggers[name];
    }

    #endregion Statics for Logger Instances

    #endregion Public Methods
}

#endregion Logger Class

#region ILogger Interface

/// <summary>
/// Interface for a logger that supports different log levels and exception logging.
/// </summary>
public interface ILogger
{
    void Trace(object message, [CallerLineNumber] int line = 0);
    void Trace(object message, Exception ex, [CallerLineNumber] int line = 0);
    void Debug(object message, [CallerLineNumber] int line = 0);
    void Debug(object message, Exception ex, [CallerLineNumber] int line = 0);
    void Info(object message, [CallerLineNumber] int line = 0);
    void Info(object message, Exception ex, [CallerLineNumber] int line = 0);
    void Warn(object message, [CallerLineNumber] int line = 0);
    void Warn(object message, Exception ex, [CallerLineNumber] int line = 0);
    void Error(object message, [CallerLineNumber] int line = 0);
    void Error(object message, Exception ex, [CallerLineNumber] int line = 0);
    void Fatal(object message, [CallerLineNumber] int line = 0);
    void Fatal(object message, Exception ex, [CallerLineNumber] int line = 0);
}

#endregion ILogger Interface