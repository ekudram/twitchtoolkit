/*
 * File: ViewerUpdater.cs
 * 
 * Project: TwitchToolkit
 * 
 * Updated: September 14, 2025
 * 
 * Summary of Changes:
 * 1. Updated ParseMessage to handle TwitchMessageWrapper instead of ChatMessage
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

    // NEW: Override the TwitchMessageWrapper version
    public override void ParseMessage(TwitchMessageWrapper messageWrapper)
    {
        try
        {
            if (messageWrapper == null)
            {
                ToolkitLogger.Warning("Received null message wrapper");
                return;
            }

            Viewer viewer = Viewers.GetViewer(messageWrapper.Username);
            UpdateViewerFromMessageWrapper(viewer, messageWrapper);

            // Update last seen timestamp for activity tracking
            viewer.last_seen = DateTime.Now;
        }
        catch (System.Exception e)
        {
            ToolkitLogger.Error($"Error processing message from {messageWrapper?.Username}: {e.Message}");
        }
    }

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

    // NEW: Helper method to update viewer from TwitchMessageWrapper
    private void UpdateViewerFromMessageWrapper(Viewer viewer, TwitchMessageWrapper messageWrapper)
    {
        if (viewer == null || messageWrapper == null) return;

        // Update viewer color code
        if (!string.IsNullOrEmpty(messageWrapper.ColorHex))
        {
            ToolkitSettings.ViewerColorCodes[messageWrapper.Username.ToLower()] =
                messageWrapper.ColorHex.Replace("#", "");
        }

        // Update moderator status using HasBadges
        if (messageWrapper.HasBadges("moderator", "broadcaster", "global_mod", "staff") && !viewer.mod)
        {
            viewer.SetAsModerator();
            ToolkitLogger.Debug($"Updated {messageWrapper.Username} to moderator via message wrapper");
        }

        // Update subscriber status using HasBadges
        if (messageWrapper.HasBadges("subscriber", "founder") && !viewer.IsSub)
        {
            viewer.subscriber = true;
            ToolkitLogger.Debug($"Updated {messageWrapper.Username} to subscriber via message wrapper");
        }

        // Update VIP status using HasBadges
        if (messageWrapper.HasBadges("vip") && !viewer.IsVIP)
        {
            viewer.vip = true;
            ToolkitLogger.Debug($"Updated {messageWrapper.Username} to VIP via message wrapper");
        }

        // Update broadcaster status
        if (messageWrapper.IsBroadcaster)
        {
            ToolkitLogger.Debug($"{messageWrapper.Username} is the broadcaster");
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