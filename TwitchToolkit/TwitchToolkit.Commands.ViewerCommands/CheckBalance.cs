/*
 * Project: TwitchToolkit
 * File: CheckBalance.cs
 * 
 * Updated: September 20, 2025
 * 
 * Purpose: Command to check viewer's coin and karma balance and respond in chat
 * 
 * Summary of Changes:
 * 1. Updated to use ChatMessage instead of ITwitchMessage for TwitchLib 3.4.0 compatibility
 * 2. Added error handling with try-catch
 * 3. Added null checking for viewer
 * 4. Improved logging for debugging
 * 5. Used string interpolation for cleaner code
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
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class CheckBalance : CommandDriver
{
    public override void RunCommand(TwitchMessageWrapper messageWrapper)
    {
        try
        {
            ToolkitLogger.Debug($"CheckBalance command requested by {messageWrapper.Username}");

            Viewer viewer = Viewers.GetViewer(messageWrapper.Username);
            if (viewer == null)
            {
                ToolkitLogger.Warning($"Could not find viewer for username: {messageWrapper.Username}");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Error: Could not retrieve your balance.");
                return;
            }

            int coins = viewer.GetViewerCoins();
            int karma = viewer.GetViewerKarma();

            string response = Helper.ReplacePlaceholder(
                Translator.Translate("TwitchToolkitBalanceMessage"),
                null, null, null, null, null, null, null, null, null, null,
                amount: coins.ToString(),
                karma: karma.ToString()
            );

            TwitchWrapper.SendChatMessage($"@{viewer.username} {response}");
            ToolkitLogger.Debug($"Sent balance info to {viewer.username}: {coins} coins, {karma} karma");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in CheckBalance command: {ex.Message}");
            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Error retrieving your balance.");
        }
    }
}
