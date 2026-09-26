using App.Core.Consts;
using System.Diagnostics;

namespace App.Core.LoggerFactory
{
    public partial class Logger : ILoggerMessageManager
    {
        private string FormatLogMessage(Containers.OutputConfig output_config, LogLevel level, StackFrame? caller_frame, int caller_line_number, object message)
        {
            string formatted_message = output_config.Format;
            string color_code = string.Empty;
            string reset_code = string.Empty;

            if (output_config.IsConsole && output_config.ColorHighlight)
            {
                color_code = level switch
                {
                    LogLevel.TRACE => LoggerFactoryConsts.COLOR_TRACE,
                    LogLevel.DEBUG => LoggerFactoryConsts.COLOR_DEBUG,
                    LogLevel.INFO => LoggerFactoryConsts.COLOR_INFO,
                    LogLevel.WARN => LoggerFactoryConsts.COLOR_WARN,
                    LogLevel.ERROR => LoggerFactoryConsts.COLOR_ERROR,
                    LogLevel.FATAL => LoggerFactoryConsts.COLOR_FATAL,
                    _ => string.Empty
                };
                reset_code = LoggerFactoryConsts.COLOR_RESET;
            }

            formatted_message = formatted_message.Replace("${timestamp}", DateTime.Now.ToString(output_config.TimestampFormat));
            formatted_message = formatted_message.Replace("${level}", $"{color_code}{level,-5}{reset_code}");
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