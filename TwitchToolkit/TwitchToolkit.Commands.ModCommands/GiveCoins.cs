/*
 * File: GiveCoins.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 20, 2025
 * 
 * Summary of Changes:
 * 1. Updated to use ChatMessage instead of ITwitchMessage for TwitchLib 3.4.0 compatibility
 * 2. Improved error handling with ToolkitLogger
 * 3. Added validation for command arguments
 * 4. Added user feedback for various error conditions
 * 5. Improved logging for better traceability
 * 6. Added permission checking for non-channel owners
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
using TwitchLib.Client.Models;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.Commands.ModCommands;

public class GiveCoins : CommandDriver
{
    public override void RunCommand(TwitchMessageWrapper messageWrapper)
    {
        try
        {
            string[] command = messageWrapper.Message.Split(' ');

            // Validate command format
            if (command.Length < 3)
            {
                ToolkitLogger.Debug("GiveCoins command called with insufficient arguments");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Usage: !givecoins <username> <amount>");
                return;
            }

            string receiver = command[1].Replace("@", "");

            // Validate amount
            if (!int.TryParse(command[2], out int amount) || amount <= 0)
            {
                ToolkitLogger.Debug("GiveCoins command: Invalid amount format");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Please provide a valid positive number of coins to give.");
                return;
            }

            // Check if user is trying to give coins to themselves (unless they're the channel owner)
            if (messageWrapper.Username.ToLower() != ToolkitSettings.Channel.ToLower() &&
                receiver.ToLower() == messageWrapper.Username.ToLower())
            {
                ToolkitLogger.Debug($"GiveCoins command: User {messageWrapper.Username} tried to give coins to themselves");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} {Translator.Translate("TwitchToolkitModCannotGiveCoins")}");
                return;
            }

            // Get the recipient viewer
            Viewer giftee = Viewers.GetViewer(receiver);
            if (giftee == null)
            {
                ToolkitLogger.Debug($"GiveCoins command: Recipient '{receiver}' not found");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Viewer '{receiver}' not found.");
                return;
            }

            // Give the coins
            ToolkitLogger.Debug($"Giving viewer {giftee.username} {amount} coins");
            giftee.GiveViewerCoins(amount);

            // Send confirmation message
            string response = Helper.ReplacePlaceholder(
                Translator.Translate("TwitchToolkitGivingCoins"),
                null, null, null, null, null, null, null, null, null, null,
                viewer: giftee.username,
                amount: amount.ToString(),
                mod: null,
                newbalance: giftee.coins.ToString()
            );

            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} {response}");

            // Log the transaction
            Store_Logger.LogGiveCoins(messageWrapper.Username, giftee.username, amount);
            ToolkitLogger.Debug($"GiveCoins command executed: {messageWrapper.Username} gave {amount} coins to {giftee.username}");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in GiveCoins command: {ex.Message}");
            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Error processing givecoins command.");
        }
    }
}