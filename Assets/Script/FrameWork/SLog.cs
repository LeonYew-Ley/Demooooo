using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public enum LogLevel { Info, Warning, Error, Debug }

public static class SLog
{
    public static bool EnableLog = true;
    public static bool EnableDebug = true;
    public static bool EnableFileLog = true;

    private static readonly string logDirectory = Path.Combine(Application.persistentDataPath, "Logs");

    public static void Info(string message, UnityEngine.Object context = null) => Log(LogLevel.Info, message, context);
    public static void Warn(string message, UnityEngine.Object context = null) => Log(LogLevel.Warning, message, context);
    public static void Error(string message, UnityEngine.Object context = null) => Log(LogLevel.Error, message, context);
    public static void DebugLog(string message, UnityEngine.Object context = null)
    {
        if (EnableDebug)
            Log(LogLevel.Debug, message, context);
    }

    private static void Log(LogLevel level, string message, UnityEngine.Object context)
    {
        if (!EnableLog && level != LogLevel.Error) return;

        // 改进前缀逻辑：如果 context 为空，获取调用者类名
        string prefix;
        if (context != null)
        {
            prefix = $"[{context.name}] ";
        }
        else
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            var frame = stackTrace.GetFrame(2); // 获取调用 Log 方法的上两层堆栈
            var method = frame.GetMethod();
            var declaringType = method.DeclaringType;
            prefix = declaringType != null ? $"[{declaringType.Name}.cs] " : "[Unknown Script] ";
        }

        string formattedForFile = Format(level, prefix + message);
        string formattedForUnity = prefix + message;

        WriteToUnityConsole(level, formattedForUnity, context);
        if (EnableFileLog)
            WriteToFile(formattedForFile);
    }

    private static string Format(LogLevel level, string message)
    {
        return $"[{DateTime.Now:HH:mm:ss.fff}] [{level}] {message}";
    }
    private static void WriteToUnityConsole(LogLevel level, string msg, UnityEngine.Object context)
    {
        switch (level)
        {
            case LogLevel.Info:
                UnityEngine.Debug.Log(msg, context); break;
            case LogLevel.Warning:
                UnityEngine.Debug.LogWarning($"{msg}", context); break;
            case LogLevel.Error:
                UnityEngine.Debug.LogError($"<color=red>{msg}</color>", context); break;
            case LogLevel.Debug:
#if UNITY_EDITOR
                UnityEngine.Debug.Log($"<color=grey>{msg}</color>", context);
#endif
                break;
        }
    }

    private static void WriteToFile(string msg)
    {
        try
        {
            // 确保日志目录存在
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // 创建按日期命名的文件路径
            string dateString = DateTime.Now.ToString("yyyy-MM-dd");
            string dailyLogFilePath = Path.Combine(logDirectory, $"log_{dateString}.txt");

            // 写入文件
            File.AppendAllText(dailyLogFilePath, msg + Environment.NewLine);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning("日志写入文件失败: " + e.Message);
        }
    }

    public static string GetLogDirectory()
    {
        return logDirectory;
    }

    public static string GetTodayLogFilePath()
    {
        string dateString = DateTime.Now.ToString("yyyy-MM-dd");
        return Path.Combine(logDirectory, $"log_{dateString}.txt");
    }

    public static void Hello()
    {
        Info("Hello from SLog!");
    }
}
