/*
 * Project: TwitchToolkit
 * File: MessageHelpers.cs
 * 
 * Created: September 20, 2025
 * 
 * Purpose: Helper class for generating standardized messages
 * 
 */
/*
 * TwitchToolkit Mod - Community Preservation Fork
 * 
 * Original Source: TwitchToolkit (GNU Affero GPL v3)
 * Original Copyright: 2019 hodlhodl from original repository
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
using Verse;

namespace TwitchToolkit.Utilities
{
    internal class MessageHelpers
    {
        public static string GetInstructionsMessage(string username)
        {
            Command allCommandsCommand = DefDatabase<Command>.GetNamed("AvailableCommands", true);
            return $"@{username} the toolkit is a mod where you earn coins while you watch. Check out the bit.ly/toolkit-guide or use !{allCommandsCommand.command} for a short list. {ToolkitSettings.Channel.CapitalizeFirst()} has a list of items/events to purchase at {ToolkitSettings.CustomPricingSheetLink}";
        }
    }
}
