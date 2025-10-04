/*
 * File: InstalledMods.cs
 * Project: TwitchToolkit
 * 
 * Updated: [Current Date]
 * 
 * Summary of Changes:
 * 1. Added user feedback when command is on cooldown
 * 2. Added null checking for message parameters
 * 3. Improved message formatting for Twitch character limits
 * 4. Added XML documentation comments
 * 5. Fixed variable name in catch block
 * 6. Improved message splitting logic
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
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands
{
    /// <summary>
    /// Handles the !mods command to list installed mods
    /// </summary>
    public class InstalledMods : CommandDriver
    {
        private const double COOLDOWN_SECONDS = 15.0;
        private const int TWITCH_MESSAGE_MAX_LENGTH = 500; // Twitch's actual limit is 500 characters

        public override void RunCommand(TwitchMessageWrapper messageWrapper)
        {
            if (messageWrapper == null)
            {
                ToolkitLogger.Error("Received null twitch message in InstalledMods command");
                return;
            }

            // Check cooldown and notify user if still cooling down
            TimeSpan timeSinceLastUse = DateTime.Now - Cooldowns.modsCommandCooldown;
            if (timeSinceLastUse.TotalSeconds <= COOLDOWN_SECONDS)
            {
                double remainingCooldown = COOLDOWN_SECONDS - timeSinceLastUse.TotalSeconds;
                TwitchWrapper.SendChatMessage(
                    $"@{messageWrapper.Username} Command is on cooldown. " +
                    $"Please wait {Math.Ceiling(remainingCooldown)} seconds."
                );
                return;
            }

            // Update cooldown
            Cooldowns.modsCommandCooldown = DateTime.Now;

            try
            {
                // Get mod list
                string modmsg = $"Version: {Toolkit.Mod.Version}, Mods: ";
                string[] mods = LoadedModManager.RunningMods
                    .Select(m => m.Name)
                    .ToArray();

                if (mods.Length == 0)
                {
                    TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} No mods installed.");
                    return;
                }

                // Send mod list in chunks that fit Twitch's message limit
                string currentMessage = modmsg;
                foreach (string mod in mods)
                {
                    string modAddition = mod + ", ";

                    // If adding this mod would exceed the limit, send current message and start new one
                    if (currentMessage.Length + modAddition.Length > TWITCH_MESSAGE_MAX_LENGTH)
                    {
                        // Remove trailing comma and space
                        currentMessage = currentMessage.TrimEnd(',', ' ');
                        TwitchWrapper.SendChatMessage(currentMessage);
                        currentMessage = "Mods continued: ";
                    }

                    currentMessage += modAddition;
                }

                // Send the final message if it has content
                if (currentMessage.Length > "Mods continued: ".Length)
                {
                    currentMessage = currentMessage.TrimEnd(',', ' ');
                    TwitchWrapper.SendChatMessage(currentMessage);
                }
            }
            catch (Exception ex)
            {
                ToolkitLogger.Error($"Error in InstalledMods command: {ex}");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Error retrieving mod list.");
            }
        }
    }
}