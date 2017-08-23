using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegrationSys.LogUtil
{
    public class Log
    {
        const bool DEBUG_ENABLE = true;
        const bool INFO_ENABLE = true;
        const bool WARN_ENABLE = true;
        const bool ERROR_ENABLE = true;
        const bool FATAL_ENABLE = true;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Debug(string message)
        {
            if (DEBUG_ENABLE)
            {
                log.Debug(message);
            }
        }

        public static void Debug(string message, Exception e)
        {
            if (DEBUG_ENABLE)
            {
                log.Debug(message, e);
            }
        }

        public static void Info(string message)
        {
            if (INFO_ENABLE)
            {
                log.Info(message);
            }
        }

        public static void Info(string message, Exception e)
        {
            if (INFO_ENABLE)
            {
                log.Info(message, e);
            }
        }

        public static void Warn(string message)
        {
            if (WARN_ENABLE)
            {
                log.Warn(message);
            }
        }

        public static void Warn(string message, Exception e)
        {
            if (WARN_ENABLE)
            {
                log.Warn(message, e);
            }
        }

        public static void Error(string message)
        {
            if (ERROR_ENABLE)
            {
                log.Error(message);
            }
        }

        public static void Error(string message, Exception e)
        {
            if (ERROR_ENABLE)
            {
                log.Error(message, e);
            }
        }

        public static void Fatal(string message)
        {
            if (FATAL_ENABLE)
            {
                log.Fatal(message);
            }
        }

        public static void Fatal(string message, Exception e)
        {
            if (FATAL_ENABLE)
            {
                log.Fatal(message, e);
            }
        }
        
    }
}
