/*
 * Project: TwitchToolkit
 * File: MessageHelpers.cs
 * 
 * Created: September 20, 2025
 * 
 * Purpose: Helper class for generating standardized messages
 * 
 */

using Verse;

namespace TwitchToolkit.TwitchToolkit.Utilities
{
    internal class MessageHelpers
    {
        public static string GetInstructionsMessage(string username)
        {
            Command allCommandsCommand = DefDatabase<Command>.GetNamed("AvailableCommands", true);
            return $"@{username} the toolkit is a mod where you earn coins while you watch. Check out the bit.ly/toolkit-guide or use !{allCommandsCommand.command} for a short list. {GenText.CapitalizeFirst(ToolkitSettings.Channel)} has a list of items/events to purchase at {ToolkitSettings.CustomPricingSheetLink}";
        }
    }
}
