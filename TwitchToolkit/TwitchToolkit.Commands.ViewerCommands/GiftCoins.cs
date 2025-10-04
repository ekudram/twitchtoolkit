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
using System.Linq;
using ToolkitCore;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class GiftCoins : CommandDriver
{
    public override void RunCommand(TwitchMessageWrapper messageWrapper)
    {
        try
        {
            ToolkitLogger.Debug($"GiftCoins command requested by {messageWrapper.Username}");

            Viewer viewer = Viewers.GetViewer(messageWrapper.Username);
            if (viewer == null)
            {
                ToolkitLogger.Warning($"Could not find viewer for username: {messageWrapper.Username}");
                return;
            }

            string[] command = messageWrapper.Message.Split(' ');
            if (command.Count() < 3)
            {
                ToolkitLogger.Debug($"Invalid gift command format from {messageWrapper.Username}: {messageWrapper.Message}");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Usage: !gift <username> <amount>");
                return;
            }

            string target = command[1].Replace("@", "");

            // Prevent self-gifting
            if (target.ToLower() == messageWrapper.Username.ToLower())
            {
                ToolkitLogger.Debug($"Viewer {messageWrapper.Username} attempted to gift themselves");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} You cannot gift coins to yourself.");
                return;
            }

            if (!int.TryParse(command[2], out int amount) || amount <= 0)
            {
                ToolkitLogger.Debug($"Invalid amount format in gift command from {messageWrapper.Username}: {command[2]}");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Please provide a valid positive number of coins to gift.");
                return;
            }

            Viewer giftee = Viewers.GetViewer(target);
            if (giftee == null)
            {
                ToolkitLogger.Debug($"Gift recipient not found: {target}");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Viewer '{target}' not found.");
                return;
            }

            // Check karma requirements
            if (ToolkitSettings.KarmaReqsForGifting)
            {
                if (giftee.GetViewerKarma() < ToolkitSettings.MinimumKarmaToRecieveGifts)
                {
                    ToolkitLogger.Debug($"Gift recipient {target} doesn't meet karma requirements");
                    TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} {target} doesn't have enough karma to receive gifts.");
                    return;
                }

                if (viewer.GetViewerKarma() < ToolkitSettings.MinimumKarmaToSendGifts)
                {
                    ToolkitLogger.Debug($"Gift sender {messageWrapper.Username} doesn't meet karma requirements");
                    TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} You don't have enough karma to send gifts.");
                    return;
                }
            }

            // Check if sender has enough coins
            if (viewer.GetViewerCoins() < amount)
            {
                ToolkitLogger.Debug($"Viewer {messageWrapper.Username} has insufficient coins for gift");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} You don't have enough coins to gift {amount} coins.");
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
            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Error processing gift command.");
        }
    }
}