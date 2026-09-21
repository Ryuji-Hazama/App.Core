namespace App.Core.Consts;

public static class LoggerFactoryConsts
{
    public const string DEFAULT_LOG_FILE_NAME = "App.log";
    public const string DEFAULT_LOG_FILE_PATH = "./logs";
    public const string DEFAULT_CONSOLE_LOG_FORMAT = "${level}: ${message}";
    public const string DEFAULT_FILE_LOG_FORMAT = "${timestamp} [${level}] ${class_path}.${caller}: ${message}";
    public const string DEFAULT_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";
}