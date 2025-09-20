/*
 * Project: TwitchToolkit
 * File: GiftCoins.cs
 * 
 * Updated: September 20, 2025
 * 
 * Purpose: Allows viewers to gift coins to other viewers via chat command
 * 
 * Summary of Changes:
 * 1. Added comprehensive error handling with try-catch
 * 2. Improved input validation and user feedback
 * 3. Added detailed logging for tracking gift transactions
 * 4. Enhanced karma requirement checking with user feedback
 * 5. Added cooldown prevention for self-gifting
 */

using System;
using System.Linq;
using ToolkitCore;
using TwitchLib.Client.Models;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class GiftCoins : CommandDriver
{
    public override void RunCommand(ChatMessage chatMessage)
    {
        try
        {
            ToolkitLogger.Debug($"GiftCoins command requested by {chatMessage.Username}");

            Viewer viewer = Viewers.GetViewer(chatMessage.Username);
            if (viewer == null)
            {
                ToolkitLogger.Warning($"Could not find viewer for username: {chatMessage.Username}");
                return;
            }

            string[] command = chatMessage.Message.Split(' ');
            if (command.Count() < 3)
            {
                ToolkitLogger.Debug($"Invalid gift command format from {chatMessage.Username}: {chatMessage.Message}");
                TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Usage: !gift <username> <amount>");
                return;
            }

            string target = command[1].Replace("@", "");

            // Prevent self-gifting
            if (target.ToLower() == chatMessage.Username.ToLower())
            {
                ToolkitLogger.Debug($"Viewer {chatMessage.Username} attempted to gift themselves");
                TwitchWrapper.SendChatMessage($"@{chatMessage.Username} You cannot gift coins to yourself.");
                return;
            }

            if (!int.TryParse(command[2], out int amount) || amount <= 0)
            {
                ToolkitLogger.Debug($"Invalid amount format in gift command from {chatMessage.Username}: {command[2]}");
                TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Please provide a valid positive number of coins to gift.");
                return;
            }

            Viewer giftee = Viewers.GetViewer(target);
            if (giftee == null)
            {
                ToolkitLogger.Debug($"Gift recipient not found: {target}");
                TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Viewer '{target}' not found.");
                return;
            }

            // Check karma requirements
            if (ToolkitSettings.KarmaReqsForGifting)
            {
                if (giftee.GetViewerKarma() < ToolkitSettings.MinimumKarmaToRecieveGifts)
                {
                    ToolkitLogger.Debug($"Gift recipient {target} doesn't meet karma requirements");
                    TwitchWrapper.SendChatMessage($"@{chatMessage.Username} {target} doesn't have enough karma to receive gifts.");
                    return;
                }

                if (viewer.GetViewerKarma() < ToolkitSettings.MinimumKarmaToSendGifts)
                {
                    ToolkitLogger.Debug($"Gift sender {chatMessage.Username} doesn't meet karma requirements");
                    TwitchWrapper.SendChatMessage($"@{chatMessage.Username} You don't have enough karma to send gifts.");
                    return;
                }
            }

            // Check if sender has enough coins
            if (viewer.GetViewerCoins() < amount)
            {
                ToolkitLogger.Debug($"Viewer {chatMessage.Username} has insufficient coins for gift");
                TwitchWrapper.SendChatMessage($"@{chatMessage.Username} You don't have enough coins to gift {amount} coins.");
                return;
            }

            // Process the gift
            viewer.TakeViewerCoins(amount);
            giftee.GiveViewerCoins(amount);

            string response = Helper.ReplacePlaceholder(
                Translator.Translate("TwitchToolkitGiftCoins"),
                null, null, null, null, null, null, null, null,
                amount: amount.ToString(),
                from: viewer.username
            );

            TwitchWrapper.SendChatMessage($"@{giftee.username} {response}");
            Store_Logger.LogGiftCoins(viewer.username, giftee.username, amount);

            ToolkitLogger.Debug($"Gift completed: {viewer.username} gifted {amount} coins to {giftee.username}");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in GiftCoins command: {ex.Message}");
            TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Error processing gift command.");
        }
    }
}