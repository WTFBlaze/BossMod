namespace CheetoClient;

using System;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// Methods meant to only be ran on Windows machines.
/// </summary>
public static class WindowsUtils
{
	private const string Kernel32 = "kernel32.dll";
	private const string User32 = "user32.dll";

	[DllImport(User32)]
	internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

	[DllImport(User32)]
	internal static extern IntPtr GetForegroundWindow();

	[DllImport(Kernel32, EntryPoint = "GetVersion", SetLastError = true)]
	internal static extern int GetVersion();

	[DllImport(Kernel32, EntryPoint = "SetConsoleMode", SetLastError = true)]
	internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

	[DllImport(Kernel32, EntryPoint = "GetConsoleMode", SetLastError = true)]
	internal static extern bool GetConsoleMode(IntPtr handle, out int mode);

	internal static void Initialize()
	{
		IntPtr handle = GetStdHandle(-11);
		GetConsoleMode(handle, out int mode);
		SetConsoleMode(handle, mode | 0x4);
	}

	[DllImport(Kernel32, EntryPoint = "GetStdHandle", SetLastError = true)]
	internal static extern IntPtr GetStdHandle(int handle);

	[DllImport(User32)]
	internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	private static IntPtr _handle;

	/// <summary>
	/// Gets or sets whether the console should be shown.
	/// </summary>
	/// <param name="flag"></param>
	public static void ShowConsole(bool flag)
	{
		const int SW_HIDE = 0;
		const int SW_SHOW = 5;

		_handle = GetStdHandle(-11);
		if (flag)
		{
			ShowWindow(_handle, SW_SHOW);
		}
		else
		{
			ShowWindow(_handle, SW_HIDE);
		}
	}

	/// <summary>
	/// Gets or sets the console mode.
	/// </summary>
	public static int ConsoleMode
	{
		get
		{
			GetConsoleMode(_handle, out int mode);
			return mode;
		}
		set
		{
			SetConsoleMode(_handle, value);
		}
	}
}
