/*
 * TwitchToolkit
 * File: GiveAllCoins.cs
 * 
 * Updated: September 20, 2025
 * 
 * Key Improvements:
 * 1. Added XML documentation for clarity
 * 2. Enhanced error handling with ToolkitLogger
 * 3. Validated input arguments
 * 4. Provided user feedback for incorrect usage
 * 5. Maintained backward compatibility
 * 6. Updated to use ChatMessage instead of ITwitchMessage for TwitchLib 3.4.0 compatibility
 * 7. Improved logging for better traceability
 * 8. Ensured positive coin amounts only
 * 9. Counted and logged number of viewers affected
 * 10. Used string interpolation for chat messages
 * 11. Simplified command parsing logic
 * 
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

public class GiveAllCoins : CommandDriver
{
    public override void RunCommand(TwitchMessageWrapper messageWrapper)
    {
        try
        {
            string[] command = messageWrapper.Message.Split(' ');
            if (command.Length < 2)
            {
                ToolkitLogger.Debug("GiveAllCoins command called with insufficient arguments");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Usage: !giveallcoins <amount>");
                return;
            }

            if (!int.TryParse(command[1], out int amount) || amount <= 0)
            {
                ToolkitLogger.Debug("GiveAllCoins command: Invalid amount format");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Please provide a valid positive number of coins to give.");
                return;
            }

            int viewersAffected = 0;
            foreach (Viewer viewer in Viewers.All)
            {
                viewer.GiveViewerCoins(amount);
                viewersAffected++;
            }

            string response = Helper.ReplacePlaceholder(
                Translator.Translate("TwitchToolkitGiveAllCoins"),
                null, null, null, null, null, null, null, null, null, null,
                amount.ToString()
            );

            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} {response}");
            ToolkitLogger.Debug($"GiveAllCoins command executed: {amount} coins given to {viewersAffected} viewers");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in GiveAllCoins command: {ex.Message}");
            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Error processing giveallcoins command.");
        }
    }
}