using log4net;
using System;
using System.Reflection;

namespace IntegrationSys.LogUtil
{
	public class Log
	{
		private const bool DEBUG_ENABLE = true;

		private const bool INFO_ENABLE = true;

		private const bool WARN_ENABLE = true;

		private const bool ERROR_ENABLE = true;

		private const bool FATAL_ENABLE = true;

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static void Debug(string message)
		{
			Log.log.Debug(message);
		}

		public static void Debug(string message, Exception e)
		{
			Log.log.Debug(message, e);
		}

		public static void Info(string message)
		{
			Log.log.Info(message);
		}

		public static void Info(string message, Exception e)
		{
			Log.log.Info(message, e);
		}

		public static void Warn(string message)
		{
			Log.log.Warn(message);
		}

		public static void Warn(string message, Exception e)
		{
			Log.log.Warn(message, e);
		}

		public static void Error(string message)
		{
			Log.log.Error(message);
		}

		public static void Error(string message, Exception e)
		{
			Log.log.Error(message, e);
		}

		public static void Fatal(string message)
		{
			Log.log.Fatal(message);
		}

		public static void Fatal(string message, Exception e)
		{
			Log.log.Fatal(message, e);
		}
	}
}
