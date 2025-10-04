/*
 * File: ToggleCoins.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 20, 2025
 * 
 * Summary of Changes:
 * 1. Added comprehensive error handling with try-catch
 * 2. Improved logging with ToolkitLogger
 * 3. Added user feedback for command execution
 * 4. Used string interpolation for cleaner code
 * 5. Ensured compatibility with TwitchLib 3.4.0 by using ChatMessage
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
using Verse;

namespace TwitchToolkit.Commands.ModCommands;

public class ToggleCoins : CommandDriver
{
    public override void RunCommand(TwitchMessageWrapper messageWrapper)
    {
        try
        {
            // Toggle the coin earning setting
            ToolkitSettings.EarningCoins = !ToolkitSettings.EarningCoins;

            // Get the translated status message
            string statusMessage = ToolkitSettings.EarningCoins ?
                Translator.Translate("TwitchToolkitOn") :
                Translator.Translate("TwitchToolkitOff");

            // Send confirmation message
            string response = $"@{messageWrapper.Username} {Translator.Translate("TwitchToolkitEarningCoinsMessage")} {statusMessage}";
            TwitchWrapper.SendChatMessage(response);

            // Log the action
            ToolkitLogger.Debug($"ToggleCoins command executed by {messageWrapper.Username}. EarningCoins is now {statusMessage}");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in ToggleCoins command: {ex.Message}");
            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Error processing togglecoins command.");
        }
    }
}