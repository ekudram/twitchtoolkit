/*
 * Project: TwitchToolkit
 * File: TwitchMessageWrapper.cs
 * 
 * Used to wrap TwitchLib ChatMessage and WhisperMessage for unified access
 * 
 * Updated: Added viewer badge lookup for whisper messages
 * 
 * Key Features:
 * 1. Unified interface for chat and whisper messages
 * 2. Badge status checking
 * 3. Implicit conversion operators for ease of use
 * 4. Immutable properties for safety
 * 5. Use this instead of ChatMessage or WhisperMessage directly in commands and other handlers
 * 6. Replaces previous ITwitchMessage class
 *
 * Copyright (c) 2025 Captolamia
 * 
 *  * This file is part of TwitchToolkit.
 * TwitchToolkit is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * TwitchToolkit is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with TwitchToolkit. If not, see <http://www.gnu.org/licenses/>.

 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;

namespace TwitchToolkit
{
    public class TwitchMessageWrapper
    {
        public string Username { get; }
        public string DisplayName { get; }
        public string UserId { get; }
        public string BotUsername { get; }
        public string Message { get; }
        public bool IsWhisper { get; }
        public bool IsModerator { get; }
        public bool IsSubscriber { get; }
        public bool IsVip { get; }
        public bool IsBroadcaster { get; }
        public string ColorHex { get; }

        // Add badge collections for more granular badge checking
        public List<KeyValuePair<string, string>> Badges { get; }
        public List<string> BadgeKeys { get; }

        // Constructor for ChatMessage
        public TwitchMessageWrapper(ChatMessage chatMessage)
        {
            Username = chatMessage.Username;
            DisplayName = chatMessage.DisplayName;
            UserId = chatMessage.UserId;
            BotUsername = chatMessage.BotUsername;
            Message = chatMessage.Message;
            IsWhisper = false;
            IsModerator = chatMessage.IsModerator;
            IsSubscriber = chatMessage.IsSubscriber;
            IsVip = chatMessage.IsVip;
            IsBroadcaster = chatMessage.IsBroadcaster;
            ColorHex = chatMessage.ColorHex;

            // Initialize badges from ChatMessage
            Badges = chatMessage.Badges?.ToList() ?? new List<KeyValuePair<string, string>>();
            BadgeKeys = Badges.Select(b => b.Key).ToList();
        }

        // Constructor for WhisperMessage
        public TwitchMessageWrapper(WhisperMessage whisperMessage)
        {
            Username = whisperMessage.Username;
            DisplayName = whisperMessage.DisplayName;
            UserId = whisperMessage.UserId;
            BotUsername = whisperMessage.BotUsername;
            Message = whisperMessage.Message;
            IsWhisper = true;
            ColorHex = whisperMessage.ColorHex;

            // For whispers, we need to check our viewer database for badge status
            var viewerBadgeInfo = GetViewerBadgeStatus(whisperMessage.Username);

            IsModerator = viewerBadgeInfo.IsModerator;
            IsSubscriber = viewerBadgeInfo.IsSubscriber;
            IsVip = viewerBadgeInfo.IsVip;
            IsBroadcaster = viewerBadgeInfo.IsBroadcaster;

            // Whisper messages typically don't have real-time badges
            Badges = new List<KeyValuePair<string, string>>();
            BadgeKeys = new List<string>();
        }
        public TwitchMessageWrapper WithMessage(string newMessage)
        {
            return new TwitchMessageWrapper(this, newMessage);
        }

        // Private constructor for creating copies with modified messages
        private TwitchMessageWrapper(TwitchMessageWrapper original, string newMessage)
        {
            Username = original.Username;
            DisplayName = original.DisplayName;
            UserId = original.UserId;
            BotUsername = original.BotUsername;
            Message = newMessage;
            IsWhisper = original.IsWhisper;
            IsModerator = original.IsModerator;
            IsSubscriber = original.IsSubscriber;
            IsVip = original.IsVip;
            IsBroadcaster = original.IsBroadcaster;
            ColorHex = original.ColorHex;

            // Copy the badge collections (they're immutable so sharing is safe)
            Badges = original.Badges;
            BadgeKeys = original.BadgeKeys;
        }

        // Helper method to get badge status from viewer database
        private (bool IsModerator, bool IsSubscriber, bool IsVip, bool IsBroadcaster) GetViewerBadgeStatus(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                    return (false, false, false, false);

                // Get the viewer from your database
                Viewer viewer = Viewers.GetViewer(username);
                if (viewer == null)
                    return (false, false, false, false);

                // Check if user is broadcaster (streamer)
                bool isBroadcaster = !string.IsNullOrEmpty(ToolkitCore.ToolkitCoreSettings.channel_username) &&
                                    string.Equals(username, ToolkitCore.ToolkitCoreSettings.channel_username, StringComparison.OrdinalIgnoreCase);

                return (viewer.mod, viewer.subscriber, viewer.vip, isBroadcaster);
            }
            catch (Exception ex)
            {
                // Log error but don't crash - return default values
                ToolkitLogger.Warning($"Error getting badge status for {username}: {ex.Message}");
                return (false, false, false, false);
            }
        }

        // Method to check if user has specific badges
        public bool HasBadges(params string[] badgeTypes)
        {
            if (badgeTypes == null || !badgeTypes.Any())
                return false;

            return badgeTypes.Any(badgeType =>
                BadgeKeys.Contains(badgeType, StringComparer.OrdinalIgnoreCase) ||
                CheckSpecialBadges(badgeType));
        }

        // Helper method to handle special badge cases (mod, sub, vip, broadcaster)
        private bool CheckSpecialBadges(string badgeType)
        {
            switch (badgeType.ToLowerInvariant())
            {
                case "moderator":
                case "mod":
                    return IsModerator;
                case "subscriber":
                case "sub":
                    return IsSubscriber;
                case "vip":
                    return IsVip;
                case "broadcaster":
                    return IsBroadcaster;
                default:
                    return false;
            }
        }


        // Implicit conversion operators for convenience
        public static implicit operator TwitchMessageWrapper(ChatMessage chatMessage) => new TwitchMessageWrapper(chatMessage);
        public static implicit operator TwitchMessageWrapper(WhisperMessage whisperMessage) => new TwitchMessageWrapper(whisperMessage);
    }
}