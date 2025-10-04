/*
 * File: KarmaRound.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 20, 2025
 * 
 * Summary of Changes:
 * 1. Simplified to use only karma-based reward system
 * 2. Improved error handling with ToolkitLogger
 * 3. Added user feedback for command execution
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

namespace TwitchToolkit.Commands.ModCommands;

public class KarmaRound : CommandDriver
{
    public override void RunCommand(TwitchMessageWrapper messageWrapper)
    {
        try
        {
            // Award coins to all viewers based on karma system
            Viewers.AwardViewersCoins();
            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} rewarded all active viewers with karma-based coins.");
            ToolkitLogger.Debug("KarmaRound command executed: karma-based coins awarded to all viewers");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in KarmaRound command: {ex.Message}");
            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Error processing karmaround command.");
        }
    }
}