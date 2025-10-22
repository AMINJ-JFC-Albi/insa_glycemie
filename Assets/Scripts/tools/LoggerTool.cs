using System;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

namespace Tools {
    public static class LoggerTool {
        public enum Level {
            Info,
            Warning,
            Error,
            Temporised
        }

        private static Dictionary<Level, DateTime> lastLogTimes = new Dictionary<Level, DateTime>();

        public static void Log(string message, Level level = Level.Info, double waitingTime = 1) {
            if (level == Level.Temporised) {
                if (lastLogTimes.TryGetValue(level, out DateTime lastLogTime)) {
                    if ((DateTime.Now - lastLogTime).TotalSeconds < waitingTime) {
                        return;
                    }
                }
                lastLogTimes[level] = DateTime.Now;
            } else {
                lastLogTimes[level] = DateTime.Now;
            }

            string logMessage = level switch {
                Level.Info => $"[INFO]: {message}",
                Level.Warning => $"[WARNING]: {message}",
                Level.Error => $"[ERROR]: {message}",
                Level.Temporised => $"[TEMPORISED]: {message}",
                _ => message
            };

#if UNITY_EDITOR || UNITY_STANDALONE
            /* DEBUG!!! c'est pas que c'est chiant mais marre des spams dans la console !
            switch (level) {
                case Level.Info:
                    Debug.Log(logMessage);
                    break;
                case Level.Warning:
                    Debug.LogWarning(logMessage);
                    break;
                case Level.Error:
                    Debug.LogError(logMessage);
                    break;
                case Level.Temporised:
                    Debug.Log(logMessage);
                    break;
            }
            */
#else
            Console.WriteLine(logMessage);
#endif
        }
    }
}
