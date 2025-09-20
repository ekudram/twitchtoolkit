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

using System;
using ToolkitCore;
using TwitchLib.Client.Models;
using Verse;

namespace TwitchToolkit.Commands.ModCommands;

public class ToggleCoins : CommandDriver
{
    public override void RunCommand(ChatMessage chatMessage)
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
            string response = $"@{chatMessage.Username} {Translator.Translate("TwitchToolkitEarningCoinsMessage")} {statusMessage}";
            TwitchWrapper.SendChatMessage(response);

            // Log the action
            ToolkitLogger.Debug($"ToggleCoins command executed by {chatMessage.Username}. EarningCoins is now {statusMessage}");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in ToggleCoins command: {ex.Message}");
            TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Error processing togglecoins command.");
        }
    }
}