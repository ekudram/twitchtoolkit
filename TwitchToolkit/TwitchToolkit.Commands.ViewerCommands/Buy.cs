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

using System.Linq;
using ToolkitCore;
using TwitchLib.Client.Models;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class Buy : CommandDriver
{
    public override void RunCommand(ChatMessage chatMessage)
    {
        ToolkitLogger.Debug("Buy command received and processing");

        Viewer viewer = Viewers.GetViewer(chatMessage.Username);
        if (viewer == null)
        {
            ToolkitLogger.Warning($"Could not find viewer for username: {chatMessage.Username}");
            return;
        }

        if (viewer.IsBanned)
        {
            ToolkitLogger.Debug($"Viewer {chatMessage.Username} is banned, skipping purchase");
            return;
        }

        string[] commandParts = chatMessage.Message.Split(' ');
        if (commandParts.Length < 2)
        {
            ToolkitLogger.Debug($"Invalid buy command format from {chatMessage.Username}: {chatMessage.Message}");
            TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Usage: !buy <item>");
            return;
        }

        ToolkitLogger.Debug($"Processing purchase request from {chatMessage.Username} for: {commandParts[1]}");
        Purchase_Handler.ResolvePurchase(viewer, chatMessage);
    }
}
