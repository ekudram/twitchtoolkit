/*
 * Project: TwitchToolkit
 * File: RefreshViewers.cs
 * 
 * Updated: September 20, 2025
 * 
 * Purpose: Admin/mod command to refresh the list of viewers from Twitch API
 * 
 * Summary of Changes:
 * 1. Added error handling with try-catch
 * 2. Added validation for channel username
 * 3. Improved logging for debugging
 * 4. Added user feedback for errors
 */

using System;
using ToolkitCore;
using TwitchLib.Client.Models;
using TwitchToolkitDev;
using Verse;

namespace TwitchToolkit.Commands.ModCommands;

public class RefreshViewers : CommandDriver
{
    public override void RunCommand(ChatMessage chatMessage)
    {
        try
        {
            ToolkitLogger.Debug($"RefreshViewers command requested by {chatMessage.Username}");

            // Validate channel username
            if (string.IsNullOrEmpty(ToolkitCoreSettings.channel_username))
            {
                ToolkitLogger.Error("Channel username is not set in settings");
                TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Error: Channel username is not configured.");
                return;
            }

            string channel = ToolkitCoreSettings.channel_username.ToLower();
            string apiUrl = $"https://tmi.twitch.tv/group/user/{channel}/chatters";

            ToolkitLogger.Debug($"Refreshing viewers from API: {apiUrl}");

            // Make the API request
            WebRequest_BeginGetResponse.Main(apiUrl, Viewers.SaveUsernamesFromJsonResponse);

            TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Viewer refresh initiated. It may take a few moments to complete.");
            ToolkitLogger.Debug($"Viewer refresh initiated by {chatMessage.Username}");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in RefreshViewers command: {ex.Message}");
            TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Error refreshing viewers. Check logs for details.");
        }
    }
}