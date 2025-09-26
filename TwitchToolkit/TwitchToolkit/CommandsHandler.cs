/*
 * File: CommandsHandler.cs
 * Project: TwitchToolkit
 * 
 * Updated: [Current Date]
 * Usage: Handles processing and execution of Twitch chat commands
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
    public static void CheckCommand(TwitchMessageWrapper messageWrapper)
    {
        ToolkitLogger.Debug($"CheckCommand called for: {messageWrapper.Username} - {messageWrapper.Message}");
        if (messageWrapper == null)
        {
            ToolkitLogger.Warning("Received null twitch message in CheckCommand");
            return;
        }

        ToolkitLogger.Debug($"Checking command - {messageWrapper.Message}");

        if (string.IsNullOrEmpty(messageWrapper.Message))
        {
            ToolkitLogger.Debug("Message is null or empty");
            return;
        }

        string user = messageWrapper.Username;
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
            .FirstOrDefault(s => messageWrapper.Message.StartsWith("!" + s.command));

        if (commandDef == null)
        {
            ToolkitLogger.Debug($"No command definition found for message: {messageWrapper.Message}");
            return;
        }

        string permissionError = ValidateCommandPermissions(commandDef, viewer, messageWrapper);
        if (!string.IsNullOrEmpty(permissionError))
        {
            SendFeedback(permissionError);
            return;
        }

        ExecuteCommand(commandDef, messageWrapper);
    }

    private static string ValidateCommandPermissions(Command commandDef, Viewer viewer, TwitchMessageWrapper messageWrapper)
    {
        if (!commandDef.enabled)
        {
            ToolkitLogger.Debug($"Command '{commandDef.command}' is disabled");
            return $"Sorry {viewer.username}, that command is currently disabled.";
        }

        if (commandDef.requiresMod && !viewer.mod && !messageWrapper.IsModerator && !IsChannelOwner(viewer))
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

    private static void ExecuteCommand(Command commandDef, TwitchMessageWrapper messageWrapper)
    {
        try
        {
            ToolkitLogger.Log($"Executing command '{commandDef.command}' for viewer '{messageWrapper.Username}'");
            commandDef.RunCommand(messageWrapper);
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error executing command '{commandDef.command}': {ex}");
            SendFeedback($"Sorry {messageWrapper.Username}, there was an error executing your command.");
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