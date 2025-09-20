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

using System;
using ToolkitCore;
using TwitchLib.Client.Models;
using Verse;

namespace TwitchToolkit.Commands.ModCommands;

public class KarmaRound : CommandDriver
{
    public override void RunCommand(ChatMessage chatMessage)
    {
        try
        {
            // Award coins to all viewers based on karma system
            Viewers.AwardViewersCoins();
            TwitchWrapper.SendChatMessage($"@{chatMessage.Username} rewarded all active viewers with karma-based coins.");
            ToolkitLogger.Debug("KarmaRound command executed: karma-based coins awarded to all viewers");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in KarmaRound command: {ex.Message}");
            TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Error processing karmaround command.");
        }
    }
}