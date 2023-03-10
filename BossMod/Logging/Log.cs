namespace CheetoClient;

#region Usings
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
#endregion

public static class Log
{
	public static LogLevel Level { get; set; } = LogLevel.NONE;

	private static bool _initialized;

	private static readonly string _dataPath = Environment.CurrentDirectory + @"\BossMod";
	private static readonly string _folderPath = _dataPath + @"\logs";

	private static readonly string _logPath = _dataPath + @"\latest.log";
	private static readonly string _oldLogPath = $"{_folderPath}{$@"\old_{(DateTime.Now - DateTime.MinValue).TotalMilliseconds}.log"}";

	public enum LogLevel
	{
		SUPER,
		NONE,
		ERROR,
		ATTENTION,
		WARNING,
		INFO,
		DEBUG,
		FINE,
	}

	/// <summary>
	/// Opens the latest log file in Notepad
	/// </summary>
	public static void OpenLatestLogFile()
	{
		try
		{
			Process.Start(_logPath);
		}
		catch (Exception e)
		{
			Error($"Failed to open Log: {e.Message} - {_logPath}");
		}
	}

	/// <summary>
	/// Writes an <see cref="Exception"/> to the logger as <see cref="LogLevel.ERROR"/>
	/// </summary>
	/// <param name="ex"></param>
	/// <param name="code"></param>
	/// <param name="name"></param>
	/// <param name="line"></param>
	public static void Exception<T>(T ex, string code = "", [CallerMemberName] string name = "", [CallerLineNumber] int line = -1)
	{
		Write($"[{name}] [{line}] An Exception Occured -> {ex.GetType()}", LogLevel.ERROR);
		if (!string.IsNullOrEmpty(code))
		{
			Write($"Error Code: {code}");
		}
		Write($"");
		if ((ex as Exception) != null)
		{
			var message = (ex as Exception).Message;
			var stackTrace = (ex as Exception).StackTrace;
			var targetSite = (ex as Exception).TargetSite;
			var source = (ex as Exception).Source;
			if (message != null)
			{
				Write($"Exception Message: {message}", LogLevel.ERROR);
			}

			if (stackTrace != null)
			{
				Write($"Exception StackTrace: {stackTrace}", LogLevel.ERROR);
			}

			if (targetSite != null)
			{
				Write($"Exception TargetSite: {targetSite}", LogLevel.ERROR);
			}

			if (source != null)
			{
				Write($"Exception Source: {source}", LogLevel.ERROR);
			}
		}
		else
		{
			Write($"Exception Was Null!", LogLevel.ERROR);
		}
	}

	/// <summary>
	/// Writes a line to the logger as <see cref="LogLevel.ERROR"/>
	/// </summary>
	/// <param name="line"></param>
	public static void Error(string msg, [CallerMemberName] string name = "", [CallerLineNumber] int line = -1)
	{
		Write($"An Error Occured");
		Write($"CallerName: {name}", LogLevel.ERROR);
		Write($"CallerLine: {line}", LogLevel.ERROR);
		Write($"{msg}", LogLevel.ERROR);
	}

	/// <summary>
	/// Writes a line to the logger as <see cref="LogLevel.ERROR"/>
	/// This particular method does not use the <see cref="CallerMemberName"/> and <see cref="CallerLineNumber"/> attributes
	/// </summary>
	/// <param name="msg"></param>
	public static void ErrorNoCall(string msg)
	{
		Write(!string.IsNullOrEmpty(msg) ? $"An Error Occured: {msg}" : $"An Error Occured");
	}

	/// <summary>
	/// Writes a line to the logger as <see cref="LogLevel.FINE"/>
	/// </summary>
	/// <param name="line"></param>
	public static void Fine(string line)
	{
		Write($"{line}", Color.Grey, LogLevel.FINE);
	}

	/// <summary>
	/// Writes a line to the logger as <see cref="LogLevel.INFO"/>
	/// </summary>
	/// <param name="line"></param>
	public static void Info(string line)
	{
		Write($"{line}", Color.Grey, LogLevel.INFO);
	}

	/// <summary>
	/// Writes a line to the logger as <see cref="LogLevel.ATTENTION"/>
	/// </summary>
	/// <param name="line"></param>
	public static void Attention(string line)
	{
		Write($"{line}", Color.CheetoYellow, LogLevel.ATTENTION);
	}

	/// <summary>
	/// Writes a line to the logger as <see cref="LogLevel.DEBUG"/>
	/// </summary>
	/// <param name="line"></param>
	public static void Debug(string line)
	{
		Write($"{line}", Color.White, LogLevel.DEBUG);
	}

	/// <summary>
	/// Writes a line to the logger as <see cref="LogLevel.DEBUG"/>
	/// </summary>
	/// <param name="line"></param>
	/// <param name="color"></param>
	public static void Debug(string line, System.Drawing.Color color)
	{
		Write($"{line}", new Color(color.R, color.G, color.B), LogLevel.DEBUG);
	}

	/// <summary>
	/// Writes a line to the logger as <see cref="LogLevel.DEBUG"/>
	/// </summary>
	/// <param name="line"></param>
	public static void Debug(string line, Color color)
	{
		Write($"{line}", color, LogLevel.DEBUG);
	}

	/// <summary>
	/// Writes a line to the logger as <see cref="LogLevel.WARNING"/>
	/// </summary>
	/// <param name="line"></param>
	public static void Warn(string line)
	{
		Write($"{line}", LogLevel.WARNING);
	}

	/// <summary>
	/// Logs "----------------------" to the logger.
	/// </summary>
	public static void Bars()
	{
		Write(string.Empty, Color.Black, noNewline: true);
		for (int i = 0; i < 16; i++)
		{
			Append("-", Color.Random);
		}
		Append(Environment.NewLine, Color.Black);
	}

	/// <summary>
	/// Writes a line to the logger.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="level"></param>
	public static void Write(string message, LogLevel level = LogLevel.INFO)
	{
		Write(message, Color.White, level);
	}

	/// <summary>
	/// Writes a line to the logger, with a specific <see cref="System.Drawing.Color"/>.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="color"></param>
	/// <param name="level"></param>
	public static void Write(string message, System.Drawing.Color color, LogLevel level = LogLevel.INFO)
	{
		Write(message, new Color(color.R, color.G, color.B), level);
	}

	public static void Append(string message, Color? color)
	{
		Color lcolor = color ?? Color.White;
		Write(message, lcolor, append: true);
	}

	/// <summary>
	/// Writes a line to the logger, with a specific <see cref="Color"/>.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="color"></param>
	/// <param name="level"></param>
	public static void Write(string message, Color color, LogLevel level = LogLevel.INFO, bool append = false, bool noNewline = false)
	{
		if (append)
		{
			InternalWrite(message, color);
		}
		else
		{
			if (Level >= level)
			{
				var lcolor = Color.White;
				if (level == LogLevel.SUPER)
				{
					lcolor = Color.Html.Teal;
				}
				if (level == LogLevel.FINE)
				{
					lcolor = Color.Html.Olive;
				}
				if (level == LogLevel.INFO)
				{
					lcolor = Color.Html.Gray;
				}
				if (level == LogLevel.DEBUG)
				{
					lcolor = Color.CheetoYellow;
				}
				if (level == LogLevel.ATTENTION)
				{
					lcolor = Color.Crayola.Original.RosePink;
				}
				if (level == LogLevel.WARNING)
				{
					lcolor = Color.Crayola.Original.Yellow;
				}
				if (level == LogLevel.ERROR)
				{
					lcolor = Color.Crayola.Original.Red;
				}

				CultureInfo ci = CultureInfo.InvariantCulture;
				InternalWrite($"[", Color.Random);
				InternalWrite($"{DateTime.Now.ToString("hh:mm:ss.fff", ci)}", Color.Crayola.Present.BananaMania);
				InternalWrite($"] ", Color.Random);

				InternalWrite($"[", Color.Random);
				InternalWrite($"BossMod", Color.CheetoOrange);
				InternalWrite($"] ", Color.Random);

				InternalWrite($"[", Color.Random);
				InternalWrite($"{Enum.GetName(typeof(LogLevel), level)}", lcolor);
				InternalWrite($"] ", Color.Random);
				InternalWriteLine(message, color, noNewline);
			}
		}
	}

	private static void InternalWriteLine(string message, Color color, bool noNewline = false)
	{
		if (noNewline)
		{
			InternalWrite(message, color);
		}
		else
		{
			InternalWrite(message + Environment.NewLine, color);
		}
	}

	private static void InternalWrite(string message, Color color)
	{
		if (!_initialized) Initialize();

		Console.Write($"{ConsoleUtils.ForegroundColor(color)}{message}");
		File.AppendAllText(_logPath, message);
		// TODO: Figure out a way to also write to the melonlogger without duplicate messages
	}

	private static void Initialize()
	{
		if (!Directory.Exists(_folderPath)) Directory.CreateDirectory(_folderPath);
		if (File.Exists(_logPath)) File.Move(_logPath, _oldLogPath);
		_initialized = true;
	}
}