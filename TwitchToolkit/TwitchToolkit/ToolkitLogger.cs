/*
 * File: ToolkitLogger.cs
 * Project: TwitchToolkit
 * 
 * Created: September 13, 2025
 * 
 * Helper class to standardize logging with color-coded messages
 */

using ToolkitCore;
using Verse;

namespace TwitchToolkit
{
    public static class ToolkitLogger
    {
        private const string Prefix = "<color=#800080>[TwitchToolkit]</color>";

        public static void Log(string message)
        {
            Verse.Log.Message($"{Prefix} {message}");
        }

        public static void Warning(string message)
        {
            Verse.Log.Warning($"{Prefix} <color=#FFFF00>{message}</color>");
        }

        public static void Error(string message)
        {
            Verse.Log.Error($"{Prefix} <color=#FF0000>{message}</color>");
        }

        public static void Message(string message)
        {
            Verse.Log.Message($"{Prefix} <color=#00FF00>{message}</color>");
        }

        // Optional: Add a method for debug messages that only show in development
        public static void Debug(string message)
        {
#if DEBUG
            Verse.Log.Message($"{Prefix} <color=#888888>[DEBUG] {message}</color>");
#endif

            // Runtime toggle for debug logging in any build
            if (ToolkitCoreSettings.enableDebugLogging)
                Verse.Log.Message($"{Prefix} <color=#888888>[DEBUG] {message}</color>");
        }

    }
}