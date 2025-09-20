/*
 * File: ViewerUpdater.cs
 * 
 * Project: TwitchToolkit
 * 
 * Updated: September 14, 2025
 * 
 * Summary of Changes:
 * 1. Updated ParseMessage to handle ChatMessage instead of ITwitchMessage
 * 2. Added ParseWhisper method implementation for handling whispers
 * 3. Added additional null checks and error handling
 * 4. Added debug logging for troubleshooting
 */

using ToolkitCore;
using TwitchLib.Client.Models;
using TwitchToolkit.PawnQueue;
using Verse;

namespace TwitchToolkit.Twitch;

public class ViewerUpdater : TwitchInterfaceBase
{
    public ViewerUpdater(Game game) : base(game)
    {
        ToolkitLogger.Debug("ViewerUpdater initialized");
    }

    public override void ParseMessage(ChatMessage chatMessage)
    {
        try
        {
            if (chatMessage == null)
            {
                ToolkitLogger.Warning("Received null chat message");
                return;
            }

            Viewer viewer = Viewers.GetViewer(chatMessage.Username);

            // Update viewer color code
            if (!string.IsNullOrEmpty(chatMessage.ColorHex))
            {
                ToolkitSettings.ViewerColorCodes[chatMessage.Username.ToLower()] =
                    chatMessage.ColorHex.Replace("#", "");
            }

            // Update moderator status
            if (chatMessage.IsModerator && !viewer.mod)
            {
                viewer.SetAsModerator();
                ToolkitLogger.Debug($"Updated {chatMessage.Username} to moderator");
            }

            // Update subscriber status
            if (chatMessage.IsSubscriber && !viewer.IsSub)
            {
                viewer.subscriber = true;
                ToolkitLogger.Debug($"Updated {chatMessage.Username} to subscriber");
            }

            // Update VIP status
            if (chatMessage.IsVip && !viewer.IsVIP)
            {
                viewer.vip = true;
                ToolkitLogger.Debug($"Updated {chatMessage.Username} to VIP");
            }
        }
        catch (System.Exception e)
        {
            ToolkitLogger.Error($"Error processing chat message from {chatMessage?.Username}: {e.Message}");
        }
    }

    public override void ParseWhisper(WhisperMessage whisperMessage)
    {
        try
        {
            if (whisperMessage == null)
            {
                ToolkitLogger.Warning("Received null whisper message");
                return;
            }

            ToolkitLogger.Debug($"Received whisper from {whisperMessage.Username}: {whisperMessage.Message}");
            // Add whisper-specific handling here if needed
        }
        catch (System.Exception e)
        {
            ToolkitLogger.Error($"Error processing whisper from {whisperMessage?.Username}: {e.Message}");
        }
    }
}