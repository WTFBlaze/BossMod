namespace CheetoClient;

using System;
using System.Diagnostics;

public static class Performance
{
	public static void ApplyTweaks()
	{
		try
		{
			using Process process = Process.GetCurrentProcess();
			process.PriorityClass = ProcessPriorityClass.RealTime;
			Log.Write($"Process priority: set to RealTime");
		}
		catch (Exception e)
		{
			Log.Error("Failed to set process priority");
			Log.Exception(e);
		}
	}
}