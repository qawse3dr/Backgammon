

using System;
using System.IO;
using UnityEngine;

namespace LNAR {
  public enum LogLevel {
    DEBUG,  // Display Everything
    INFO,   // Display most useful infomation
    WARN,   // Display warning messages if anything goes wrong that won't break the game
    ERROR,  // Something went wrong display error msg
  }

  /**
  This class will be used to replace Debug.Log
  as there is no way to display this in the final release
  and atleast for now we want a way to display this information to stdout
  in the final build, while also being able to display it to debug.log in the editor
  **/
  public class Logger {
    public static LogLevel Level {
      get { return _level; }
      set { _level = value; }
    }
    // The Current level of the logger
    // For now it makes most sense to default to debug but this will
    // most likely change in the future.
    private static LogLevel _level = LogLevel.DEBUG;

    // Debug. NOTE: it's spelt DEBG so it lines up with the other Log Levels
    public static void Debug(string msg, string loggerName = "LNAR") {
      Output(msg, loggerName, "DEBG", LogLevel.DEBUG);
    }

    // Info
    public static void Info(string msg, string loggerName = "LNAR") {
      Output(msg, loggerName, "INFO", LogLevel.INFO);
    }

    // Warning
    public static void Warn(string msg, string loggerName = "LNAR") {
      Output(msg, loggerName, "WARN", LogLevel.WARN);
    }

    // Error NOTE: it's spelt "ERR " so it lines up with the other Log Levels
    public static void Error(string msg, string loggerName = "LNAR") {
      Output(msg, loggerName, "ERR ", LogLevel.ERROR);
    }

    /**
     * messages will be displayed in the format only if the log level is greater than or equal
     * requested log level loggerName::LogLevel - : msg if no loggerName is supplied it will default
     * to LNAR
     */
    private static void Output(string msg, string loggerName, string logLevelStr,
                               LogLevel logLevel) {
      if (Level <= logLevel) {
        string formatedMsg = $"{loggerName}::{logLevelStr} : {msg}";

        // replace newlines with LoggerName and level so every line starts with
        // the same format. this makes parsing logs a lot easier using grep
        // IE (./build -logfile stdout | grep "loggerName")
        formatedMsg = formatedMsg.Replace("\n", $"\n{loggerName}::{logLevelStr} : ");
#if UNITY_EDITOR
        UnityEngine.Debug.Log(formatedMsg);
#else
        Console.WriteLine(formatedMsg);
#endif
      }
    }
  }

}
