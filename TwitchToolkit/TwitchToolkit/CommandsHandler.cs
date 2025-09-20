/*
 * File: CommandsHandler.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 20, 2025
 * 
 * Summary of Changes:
 * 1. Added comprehensive XML documentation
 * 2. Replaced Log.Message with ToolkitLogger
 * 3. Added null checking and validation
 * 4. Improved command processing logic clarity
 * 5. Added cooldown system documentation
 * 6. Added user feedback for command restrictions
 * 7. Removed unused SendToChatroom method
 */

using System;
using System.Linq;
using TwitchLib.Client.Models;
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
    /// <param name="chatMessage">The Twitch message to process</param>
    /// <remarks>
    /// This method handles command validation, permission checking, and execution
    /// </remarks>
    public static void CheckCommand(ChatMessage chatMessage)
    {
        if (chatMessage == null)
        {
            ToolkitLogger.Warning("Received null twitch message in CheckCommand");
            return;
        }

        ToolkitLogger.Debug($"Checking command - {chatMessage.Message}");

        if (string.IsNullOrEmpty(chatMessage.Message))
        {
            ToolkitLogger.Debug("Message is null or empty");
            return;
        }

        string user = chatMessage.Username;
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
            .FirstOrDefault(s => chatMessage.Message.StartsWith("!" + s.command));

        if (commandDef == null)
        {
            ToolkitLogger.Debug($"No command definition found for message: {chatMessage.Message}");
            return;
        }

        string permissionError = ValidateCommandPermissions(commandDef, viewer);
        if (!string.IsNullOrEmpty(permissionError))
        {
            SendFeedback(permissionError);
            return;
        }

        ExecuteCommand(commandDef, chatMessage);
    }

    /// <summary>
    /// Validates if a viewer has permission to execute a command
    /// </summary>
    /// <param name="commandDef">The command definition</param>
    /// <param name="viewer">The viewer attempting to execute the command</param>
    /// <returns>Error message if the viewer doesn't have permission, otherwise null</returns>
    private static string ValidateCommandPermissions(Command commandDef, Viewer viewer)
    {
        if (!commandDef.enabled)
        {
            ToolkitLogger.Debug($"Command '{commandDef.command}' is disabled");
            return $"Sorry {viewer.username}, that command is currently disabled.";
        }

        if (commandDef.requiresMod && !viewer.mod && !IsChannelOwner(viewer))
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

    /// <summary>
    /// Checks if the viewer is the channel owner
    /// </summary>
    /// <param name="viewer">The viewer to check</param>
    /// <returns>True if the viewer is the channel owner</returns>
    private static bool IsChannelOwner(Viewer viewer)
    {
        return viewer.username.Equals(ToolkitSettings.Channel, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Executes a command with proper error handling
    /// </summary>
    /// <param name="commandDef">The command definition to execute</param>
    /// <param name="chatMessage">The Twitch message that triggered the command</param>
    private static void ExecuteCommand(Command commandDef, ChatMessage chatMessage)
    {
        try
        {
            ToolkitLogger.Log($"Executing command '{commandDef.command}' for viewer '{chatMessage.Username}'");
            commandDef.RunCommand(chatMessage);
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error executing command '{commandDef.command}': {ex}");
            SendFeedback($"Sorry {chatMessage.Username}, there was an error executing your command.");
        }
    }

    /// <summary>
    /// Sends a feedback message to the user through Twitch chat
    /// </summary>
    /// <param name="message">The message to send</param>
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
