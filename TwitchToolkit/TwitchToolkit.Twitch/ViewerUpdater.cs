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
 * 5. Fixed UserType reference by adding proper using directive
 * 6. Removed Shutdown method (not in base class)
 * 7. Fixed OnUserJoined method signature
 */

using System;
using System.Linq;
using ToolkitCore;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client.Enums; // Added for UserType
using Verse;

namespace TwitchToolkit.Twitch;

public class ViewerUpdater : TwitchInterfaceBase
{
    public ViewerUpdater(Game game) : base(game)
    {
        ToolkitLogger.Debug("ViewerUpdater initialized");
        // Subscribe to UserStateChanged event for more reliable user information
        if (TwitchWrapper.Client != null)
        {
            TwitchWrapper.Client.OnUserStateChanged += OnUserStateChangedHandler;
        }
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
            UpdateViewerFromMessage(viewer, chatMessage);

            // Update last seen timestamp for activity tracking
            viewer.last_seen = DateTime.Now;
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

            Viewer viewer = Viewers.GetViewer(whisperMessage.Username);
            viewer.last_seen = DateTime.Now;

            ToolkitLogger.Debug($"Received whisper from {whisperMessage.Username}: {whisperMessage.Message}");
            // Add whisper-specific handling here if needed
        }
        catch (System.Exception e)
        {
            ToolkitLogger.Error($"Error processing whisper from {whisperMessage?.Username}: {e.Message}");
        }
    }

    // New method to handle UserState changes
    private void OnUserStateChangedHandler(object sender, OnUserStateChangedArgs e)
    {
        try
        {
            if (e.UserState == null)
            {
                ToolkitLogger.Warning("Received null user state");
                return;
            }

            string username = e.UserState.DisplayName ?? e.UserState.Channel;
            if (string.IsNullOrEmpty(username))
            {
                ToolkitLogger.Warning("UserState has no valid username");
                return;
            }

            Viewer viewer = Viewers.GetViewer(username);
            UpdateViewerFromUserState(viewer, e.UserState);

            ToolkitLogger.Debug($"Updated user state for {username}");
        }
        catch (System.Exception ex)
        {
            ToolkitLogger.Error($"Error processing user state: {ex.Message}");
        }
    }

    // Helper method to update viewer from ChatMessage
    private void UpdateViewerFromMessage(Viewer viewer, ChatMessage chatMessage)
    {
        if (viewer == null || chatMessage == null) return;

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
            ToolkitLogger.Debug($"Updated {chatMessage.Username} to moderator via chat message");
        }

        // Update subscriber status
        if (chatMessage.IsSubscriber && !viewer.IsSub)
        {
            viewer.subscriber = true;
            ToolkitLogger.Debug($"Updated {chatMessage.Username} to subscriber via chat message");
        }

        // Update VIP status
        if (chatMessage.IsVip && !viewer.IsVIP)
        {
            viewer.vip = true;
            ToolkitLogger.Debug($"Updated {chatMessage.Username} to VIP via chat message");
        }

        // Update user type from chat message
        UpdateViewerFromUserType(viewer, chatMessage.UserType);

        // Update badges from chat message if available
        if (chatMessage.Badges != null && chatMessage.Badges.Count > 0)
        {
            UpdateViewerBadges(viewer, chatMessage.Badges);
        }
    }

    // Helper method to update viewer from UserState (more reliable)
    private void UpdateViewerFromUserState(Viewer viewer, UserState userState)
    {
        if (viewer == null || userState == null) return;

        // Update viewer color code
        if (!string.IsNullOrEmpty(userState.ColorHex))
        {
            ToolkitSettings.ViewerColorCodes[userState.DisplayName.ToLower()] =
                userState.ColorHex.Replace("#", "");
        }

        // Update moderator status
        if (userState.IsModerator && !viewer.mod)
        {
            viewer.SetAsModerator();
            ToolkitLogger.Debug($"Updated {userState.DisplayName} to moderator via user state");
        }

        // Update subscriber status
        if (userState.IsSubscriber && !viewer.IsSub)
        {
            viewer.subscriber = true;
            ToolkitLogger.Debug($"Updated {userState.DisplayName} to subscriber via user state");
        }

        // Update user type (includes VIP, Broadcaster, etc.)
        UpdateViewerFromUserType(viewer, userState.UserType);

        // Update badges from user state
        if (userState.Badges != null && userState.Badges.Count > 0)
        {
            UpdateViewerBadges(viewer, userState.Badges);
        }
    }

    // Helper method to update viewer based on UserType
    private void UpdateViewerFromUserType(Viewer viewer, TwitchLib.Client.Enums.UserType userType)
    {
        switch (userType)
        {
            case TwitchLib.Client.Enums.UserType.Broadcaster:
                // Handle broadcaster status if needed
                ToolkitLogger.Debug($"{viewer.username} is the broadcaster");
                break;
            case TwitchLib.Client.Enums.UserType.Moderator:
                if (!viewer.mod)
                {
                    viewer.SetAsModerator();
                    ToolkitLogger.Debug($"Updated {viewer.username} to moderator via user type");
                }
                break;
            //case TwitchLib.Client.Enums.UserType.VIP:
            //    if (!viewer.IsVIP)
            //    {
            //        viewer.vip = true;
            //        ToolkitLogger.Debug($"Updated {viewer.username} to VIP via user type");
            //    }
            //    break;
            case TwitchLib.Client.Enums.UserType.Admin:
            case TwitchLib.Client.Enums.UserType.GlobalModerator:
            case TwitchLib.Client.Enums.UserType.Staff:
                // Handle other user types if needed
                break;
        }
    }

    // Helper method to update viewer badges
    private void UpdateViewerBadges(Viewer viewer, System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>> badges)
    {
        foreach (var badge in badges)
        {
            switch (badge.Key.ToLower())
            {
                case "broadcaster":
                    // Handle broadcaster badge
                    break;
                case "moderator":
                    if (!viewer.mod)
                    {
                        viewer.SetAsModerator();
                        ToolkitLogger.Debug($"Updated {viewer.username} to moderator via badge");
                    }
                    break;
                case "vip":
                    if (!viewer.IsVIP)
                    {
                        viewer.vip = true;
                        ToolkitLogger.Debug($"Updated {viewer.username} to VIP via badge");
                    }
                    break;
                case "subscriber":
                case "founder":
                    if (!viewer.IsSub)
                    {
                        viewer.subscriber = true;
                        ToolkitLogger.Debug($"Updated {viewer.username} to subscriber via badge");
                    }
                    break;
                default:
                    // Handle other badges if needed
                    break;
            }
        }
    }
}
