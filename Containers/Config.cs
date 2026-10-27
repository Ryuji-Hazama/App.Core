namespace App.Core.Containers;


public class AppConfig
{
    public string AssemblyPath { get; set; } = string.Empty;
    public string ComponentsList { get; set; } = string.Empty;
}

public class LoggerConfig
{
    public string? LogConfig { get; set; }
}

public class Config
{
    public AppConfig App { get; set; } = new AppConfig();
    public LoggerConfig Logger { get; set; } = new LoggerConfig();
}
