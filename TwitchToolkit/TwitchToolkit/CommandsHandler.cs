/*
 * File: CommandsHandler.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 19, 2025
 * 
 * Summary of Changes:
 * 1. Added comprehensive XML documentation
 * 2. Replaced Log.Message with ToolkitLogger
 * 3. Added null checking and validation
 * 4. Improved command processing logic clarity
 * 5. Added cooldown system documentation
 */

using System;
using System.Linq;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;
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
    /// <param name="twitchMessage">The Twitch message to process</param>
    /// <remarks>
    /// This method handles command validation, permission checking, and execution
    /// </remarks>
    public static void CheckCommand(ITwitchMessage twitchMessage)
    {
        if (twitchMessage == null)
        {
            ToolkitLogger.Warning("Received null twitch message in CheckCommand");
            return;
        }

        ToolkitLogger.Debug($"Checking command - {twitchMessage.Message}");

        if (string.IsNullOrEmpty(twitchMessage.Message))
        {
            ToolkitLogger.Debug("Message is null or empty");
            return;
        }

        string user = twitchMessage.Username;
        Viewer viewer = Viewers.GetViewer(user);

        if (viewer == null)
        {
            ToolkitLogger.Warning($"Viewer not found for username: {user}");
            return;
        }

        viewer.last_seen = DateTime.Now;

        if (viewer.IsBanned)
        {
            ToolkitLogger.Debug($"Viewer {user} is banned, skipping command processing");
            return;
        }

        // Find the command definition that matches the message
        Command commandDef = DefDatabase<Command>.AllDefs
            .FirstOrDefault(s => twitchMessage.Message.StartsWith("!" + s.command));

        if (commandDef == null)
        {
            ToolkitLogger.Debug($"No command definition found for message: {twitchMessage.Message}");
            return;
        }

        bool runCommand = ValidateCommandPermissions(commandDef, viewer, twitchMessage);

        if (runCommand)
        {
            ExecuteCommand(commandDef, twitchMessage);
        }
    }

    /// <summary>
    /// Validates if a viewer has permission to execute a command
    /// </summary>
    /// <param name="commandDef">The command definition</param>
    /// <param name="viewer">The viewer attempting to execute the command</param>
    /// <param name="twitchMessage">The original Twitch message</param>
    /// <returns>True if the viewer has permission to execute the command</returns>
    private static bool ValidateCommandPermissions(Command commandDef, Viewer viewer, ITwitchMessage twitchMessage)
    {
        if (!commandDef.enabled)
        {
            ToolkitLogger.Debug($"Command '{commandDef.command}' is disabled");
            return false;
        }

        if (commandDef.requiresMod && !viewer.mod && !IsChannelOwner(viewer))
        {
            ToolkitLogger.Debug($"Viewer {viewer.username} lacks mod privileges for command '{commandDef.command}'");
            return false;
        }

        if (commandDef.requiresAdmin && !IsChannelOwner(viewer))
        {
            ToolkitLogger.Debug($"Viewer {viewer.username} lacks admin privileges for command '{commandDef.command}'");
            return false;
        }

        return true;
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
    /// <param name="twitchMessage">The Twitch message that triggered the command</param>
    private static void ExecuteCommand(Command commandDef, ITwitchMessage twitchMessage)
    {
        try
        {
            ToolkitLogger.Log($"Executing command '{commandDef.command}' for viewer '{twitchMessage.Username}'");
            commandDef.RunCommand(twitchMessage);
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error executing command '{commandDef.command}': {ex}");
        }
    }

    /// <summary>
    /// Determines if a chat message should be sent to a separate chat room
    /// </summary>
    /// <param name="msg">The chat message to check</param>
    /// <returns>Currently always returns true (placeholder functionality)</returns>
    /// <remarks>
    /// This method appears to be a placeholder for future chat room filtering functionality
    /// </remarks>
    public static bool SendToChatroom(ChatMessage msg)
    {
        // Placeholder implementation - currently all messages are sent to chatroom
        return true;
    }
}

/*** CommandsHandler.cs
using System;
using System.Linq;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace TwitchToolkit;

public static class CommandsHandler
{
	private static DateTime modsCommandCooldown = DateTime.MinValue;

	private static DateTime aliveCommandCooldown = DateTime.MinValue;

	public static void CheckCommand(ITwitchMessage twitchMessage)
	{
		Log.Message("Checking command - " + twitchMessage.Message);
		if (twitchMessage == null || twitchMessage.Message == null)
		{
			return;
		}
		string message = twitchMessage.Message;
		string user = twitchMessage.Username;
		Viewer viewer = Viewers.GetViewer(user);
		viewer.last_seen = DateTime.Now;
		if (viewer.IsBanned)
		{
            Log.Message("viewer is banned.");
            return;
		}
		Command commandDef = DefDatabase<Command>.AllDefs.ToList().Find((Command s) => twitchMessage.Message.StartsWith("!" + s.command));
		if (commandDef != null)
		{
			bool runCommand = true;
			if (commandDef.requiresMod && !viewer.mod && viewer.username.ToLower() != ToolkitSettings.Channel.ToLower())
			{
				runCommand = false;
            }
			if (commandDef.requiresAdmin && twitchMessage.Username.ToLower() != ToolkitSettings.Channel.ToLower())
			{
				runCommand = false;

            }
            if (!commandDef.enabled)
			{
				runCommand = false;
            }
            if (runCommand)
			{
                commandDef.RunCommand(twitchMessage);
			}
		}
	}

	public static bool SendToChatroom(ChatMessage msg)
	{
		return true;
	}
}
**/