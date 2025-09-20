/*
 * Project: Twitch Toolkit
 * File: SetKarma.cs
 * Updated: September 20, 2025
 * 
 * Key Changes:
 * 1. Changed from ITwitchMessage to ChatMessage for TwitchLib 3.4.0 compatibility
 * 2. Added validation for command format and amount parameter
 * 3. Added user feedback for incorrect usage and errors
 * 4. Added null checking for the target viewer
 * 5. Improved error handling with more specific error messages
 * 6. Added debug logging for tracking command execution
 * 7. Fixed the exception message to reference "Set Karma" instead of "Check User"
 * 8. Used string interpolation for cleaner code where appropriate
 */

using System;
using ToolkitCore;
using TwitchLib.Client.Models;
using Verse;

namespace TwitchToolkit.Commands.ModCommands;

public class SetKarma : CommandDriver
{
    public override void RunCommand(ChatMessage chatMessage)
    {
        try
        {
            string[] command = chatMessage.Message.Split(' ');

            // Validate command format
            if (command.Length < 3)
            {
                ToolkitLogger.Debug("SetKarma command called with insufficient arguments");
                TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Usage: !setkarma <username> <amount>");
                return;
            }

            string target = command[1].Replace("@", "");

            // Validate amount
            if (!int.TryParse(command[2], out int amount))
            {
                ToolkitLogger.Debug("SetKarma command: Invalid amount format");
                TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Please provide a valid number for karma amount.");
                return;
            }

            // Get the target viewer
            Viewer targeted = Viewers.GetViewer(target);
            if (targeted == null)
            {
                ToolkitLogger.Debug($"SetKarma command: Target viewer '{target}' not found");
                TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Viewer '{target}' not found.");
                return;
            }

            // Set the karma
            targeted.SetViewerKarma(amount);

            // Send confirmation message
            string response = Helper.ReplacePlaceholder(
                Translator.Translate("TwitchToolkitSetKarma"),
                null, null, null, null, null, null, null, null, null, null,
                viewer: targeted.username,
                amount: amount.ToString()
            );

            TwitchWrapper.SendChatMessage($"@{chatMessage.Username} {response}");
            ToolkitLogger.Debug($"SetKarma command executed: Set {targeted.username}'s karma to {amount}");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in SetKarma command: {ex.Message}");
            TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Error processing setkarma command.");
        }
    }
}