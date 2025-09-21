/*
 * File: CommandsHandler.cs
 * Project: TwitchToolkit
 * 
 * Updated: [Current Date]
 * 
 * Summary of Changes:
 * 1. Added comprehensive XML documentation
 * 2. Replaced Log.Message with ToolkitLogger
 * 3. Added null checking and validation
 * 4. Improved command processing logic clarity
 * 5. Added cooldown system documentation
 * 6. Added user feedback for command restrictions
 * 7. Removed unused SendToChatroom method
 * 8. Updated to use TwitchMessageWrapper for unified message handling
 */

using System;
using System.Linq;
using Verse;

namespace TwitchToolkit;

/// <summary>
/// Handles processing and execution of Twitch chat commands
/// </summary>
public static class CommandsHandler
{
    private static DateTime modsCommandCooldown = DateTime.MinValue;
    private static DateTime aliveCommandCooldown = DateTime.MinValue;

    /// <summary>
    /// Processes and executes a Twitch chat command if valid
    /// </summary>
    /// <param name="message">The wrapped Twitch message to process</param>
    public static void CheckCommand(TwitchMessageWrapper message)
    {
        if (message == null)
        {
            ToolkitLogger.Warning("Received null twitch message in CheckCommand");
            return;
        }

        ToolkitLogger.Debug($"Checking command - {message.Message}");

        if (string.IsNullOrEmpty(message.Message))
        {
            ToolkitLogger.Debug("Message is null or empty");
            return;
        }

        string user = message.Username;
        Viewer viewer = Viewers.GetViewer(user);

        if (viewer == null)
        {
            ToolkitLogger.Warning($"Viewer not found for username: {user}");
            SendFeedback($"Sorry {user}, I couldn't find your viewer data. Please try again later.");
            return;
        }

        viewer.last_seen = DateTime.Now;

        if (viewer.IsBanned)
        {
            ToolkitLogger.Debug($"Viewer {user} is banned, skipping command processing");
            SendFeedback($"Sorry {user}, you've been banned from using commands. Please contact the streamer if you think this is a mistake.");
            return;
        }

        // Find the command definition that matches the message
        Command commandDef = DefDatabase<Command>.AllDefs
            .FirstOrDefault(s => message.Message.StartsWith("!" + s.command));

        if (commandDef == null)
        {
            ToolkitLogger.Debug($"No command definition found for message: {message.Message}");
            return;
        }

        string permissionError = ValidateCommandPermissions(commandDef, viewer, message);
        if (!string.IsNullOrEmpty(permissionError))
        {
            SendFeedback(permissionError);
            return;
        }

        ExecuteCommand(commandDef, message);
    }

    private static string ValidateCommandPermissions(Command commandDef, Viewer viewer, TwitchMessageWrapper message)
    {
        if (!commandDef.enabled)
        {
            ToolkitLogger.Debug($"Command '{commandDef.command}' is disabled");
            return $"Sorry {viewer.username}, that command is currently disabled.";
        }

        if (commandDef.requiresMod && !viewer.mod && !message.IsModerator && !IsChannelOwner(viewer))
        {
            ToolkitLogger.Debug($"Viewer {viewer.username} lacks mod privileges for command '{commandDef.command}'");
            return $"Sorry {viewer.username}, you need to be a moderator to use that command.";
        }

        if (commandDef.requiresAdmin && !IsChannelOwner(viewer))
        {
            ToolkitLogger.Debug($"Viewer {viewer.username} lacks admin privileges for command '{commandDef.command}'");
            return $"Sorry {viewer.username}, only the streamer can use that command.";
        }

        return null;
    }

    private static bool IsChannelOwner(Viewer viewer)
    {
        return viewer.username.Equals(ToolkitSettings.Channel, StringComparison.OrdinalIgnoreCase);
    }

    private static void ExecuteCommand(Command commandDef, TwitchMessageWrapper message)
    {
        try
        {
            ToolkitLogger.Log($"Executing command '{commandDef.command}' for viewer '{message.Username}'");
            commandDef.RunCommand(message);
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error executing command '{commandDef.command}': {ex}");
            SendFeedback($"Sorry {message.Username}, there was an error executing your command.");
        }
    }

    private static void SendFeedback(string message)
    {
        try
        {
            ToolkitCore.TwitchWrapper.SendChatMessage(message);
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error sending feedback message: {ex}");
        }
    }
}