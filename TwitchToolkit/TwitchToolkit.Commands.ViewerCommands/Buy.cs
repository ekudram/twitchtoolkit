/*
 * File: Buy.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 20, 2025
 * 
 * Summary of Changes:
 * 1. Changed warning log to debug log for "reached the override" message
 * 2. Added null checking for viewer
 * 3. Added input validation for command format
 * 4. Improved error handling
 * 5. Added more descriptive debug logging
 */

// Buy.cs (example command driver)
using System.Linq;
using ToolkitCore;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class Buy : CommandDriver
{
    public override void RunCommand(TwitchMessageWrapper message)
    {
        ToolkitLogger.Debug("Buy command received and processing");

        Viewer viewer = Viewers.GetViewer(message.Username);
        if (viewer == null)
        {
            ToolkitLogger.Warning($"Could not find viewer for username: {message.Username}");
            return;
        }

        if (viewer.IsBanned)
        {
            ToolkitLogger.Debug($"Viewer {message.Username} is banned, skipping purchase");
            return;
        }

        string[] commandParts = message.Message.Split(' ');
        if (commandParts.Length < 2)
        {
            ToolkitLogger.Debug($"Invalid buy command format from {message.Username}: {message.Message}");
            TwitchWrapper.SendChatMessage($"@{message.Username} Usage: !buy <item>");
            return;
        }

        ToolkitLogger.Debug($"Processing purchase request from {message.Username} for: {commandParts[1]}");
        Purchase_Handler.ResolvePurchase(viewer, message);
    }
}