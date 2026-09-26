namespace App.Core.Consts;

public static class LoggerFactoryConsts
{
    public const string DEFAULT_LOG_FILE_NAME = "App.log";
    public const string DEFAULT_LOG_FILE_PATH = "./logs";
    public const string DEFAULT_CONSOLE_LOG_FORMAT = "${level}: ${message}";
    public const string DEFAULT_FILE_LOG_FORMAT = "${timestamp} [${level}] ${class_path}.${caller}: ${message}";
    public const string DEFAULT_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

    public const string COLOR_TRACE = "\x1b[90m";
    public const string COLOR_DEBUG = "\x1b[32m";
    //bLightBlue
    public const string COLOR_INFO = "\x1b[96m";
    //bRed
    public const string COLOR_WARN = "\x1b[91m";
    //Red
    public const string COLOR_ERROR = "\x1b[31m";
    //Bold Red
    public const string COLOR_FATAL = "\x1b[1;31m";
    public const string COLOR_RESET = "\x1b[0m";
}