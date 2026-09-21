using System.Diagnostics;

namespace App.Core.LoggerFactory
{
    public partial class Logger : ILoggerMessageManager
    {
        private string FormatLogMessage(Containers.OutputConfig output_config, LogLevel level, StackFrame? caller_frame, int caller_line_number, object message)
        {
            string formatted_message = output_config.Format;

            formatted_message = formatted_message.Replace("${timestamp}", DateTime.Now.ToString(output_config.TimestampFormat));
            formatted_message = formatted_message.Replace("${level}", $"{level,-5}");
            formatted_message = formatted_message.Replace("${pid}", Environment.ProcessId.ToString());
            formatted_message = formatted_message.Replace("${class_path}", _source);
            formatted_message = formatted_message.Replace("${caller}", caller_frame?.GetMethod()?.Name ?? "Unknown");
            formatted_message = formatted_message.Replace("${line}", caller_line_number.ToString());
            formatted_message = formatted_message.Replace("${message}", message.ToString());

            return formatted_message;
        }
    }

    public interface ILoggerMessageManager
    {
    }
}