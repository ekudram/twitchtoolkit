/*
 * File: Buy.cs
 * Project: TwitchToolkit
 * 
 * Updated: [Current Date]
 * 
 * Summary of Changes:
 * 1. Changed warning log to debug log for "reached the override" message
 * 2. Added null checking for viewer
 * 3. Added input validation for command format
 * 4. Improved error handling
 * 5. Added more descriptive debug logging
 * 6. Updated to use TwitchMessageWrapper for unified message handling
 */

using System.Linq;
using ToolkitCore;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class Buy : CommandDriver
{
    public override void RunCommand(TwitchMessageWrapper messageWrapper)
    {
        ToolkitLogger.Debug("Buy command received and processing");

        Viewer viewer = Viewers.GetViewer(messageWrapper.Username);
        if (viewer == null)
        {
            ToolkitLogger.Warning($"Could not find viewer for username: {messageWrapper.Username}");
            return;
        }

        if (viewer.IsBanned)
        {
            ToolkitLogger.Debug($"Viewer {messageWrapper.Username} is banned, skipping purchase");
            return;
        }

        string[] commandParts = messageWrapper.Message.Split(' ');
        if (commandParts.Length < 2)
        {
            ToolkitLogger.Debug($"Invalid buy command format from {messageWrapper.Username}: {messageWrapper.Message}");
            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Usage: !buy <item>");
            return;
        }

        ToolkitLogger.Debug($"Processing purchase request from {messageWrapper.Username} for: {commandParts[1]}");

        Purchase_Handler.ResolvePurchase(viewer, messageWrapper);
    }
}