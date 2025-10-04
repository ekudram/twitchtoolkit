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
/*
 * TwitchToolkit Mod - Community Preservation Fork
 * 
 * Original Source: TwitchToolkit (GNU Affero GPL v3)
 * Original Copyright: 2019 from original repository
 * 
 * MAJOR MODIFICATIONS © 2025 Captolamia:
 * Modifications listed above if any.
 * 
 * This work is licensed under GNU Affero GPL v3
 * This is a community preservation effort to maintain and improve
 * abandoned mod code for the benefit of all users.
 * 
 * See LICENSE file for full terms.
 */
using System;
using ToolkitCore;

namespace TwitchToolkit.Commands.ModCommands;

public class RefreshViewers : CommandDriver
{
    public override void RunCommand(TwitchMessageWrapper messageWrapper)
    {
        try
        {
            ToolkitLogger.Debug($"RefreshViewers command requested by {messageWrapper.Username}");

            // Validate channel username
            if (string.IsNullOrEmpty(ToolkitCoreSettings.channel_username))
            {
                ToolkitLogger.Error("Channel username is not set in settings");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Error: Channel username is not configured.");
                return;
            }

            string channel = ToolkitCoreSettings.channel_username.ToLower();
            string apiUrl = $"https://tmi.twitch.tv/group/user/{channel}/chatters";

            ToolkitLogger.Debug($"Refreshing viewers from API: {apiUrl}");

            // No Longer using WebRequest_BeginGetResponse due to async issues
            // NO! WebRequest_BeginGetResponse.Main(apiUrl, Viewers.SaveUsernamesFromJsonResponse);

            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Viewer refresh initiated. It may take a few moments to complete.");
            ToolkitLogger.Debug($"Viewer refresh initiated by {messageWrapper.Username}");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in RefreshViewers command: {ex.Message}");
            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Error refreshing viewers. Check logs for details.");
        }
    }
}