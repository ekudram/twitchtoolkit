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
 */

using System;
using System.Linq;
using ToolkitCore;
using TwitchLib.Client.Models.Interfaces;
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

        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            if (twitchMessage == null)
            {
                Log.Warning("[TwitchToolkit] Received null twitch message in InstalledMods command");
                return;
            }

            // Check cooldown and notify user if still cooling down
            TimeSpan timeSinceLastUse = DateTime.Now - Cooldowns.modsCommandCooldown;
            if (timeSinceLastUse.TotalSeconds <= COOLDOWN_SECONDS)
            {
                double remainingCooldown = COOLDOWN_SECONDS - timeSinceLastUse.TotalSeconds;
                TwitchWrapper.SendChatMessage(
                    $"@{twitchMessage.Username} Command is on cooldown. " +
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

                // Send mod list in chunks that fit Twitch's message limit
                string currentMessage = modmsg;
                for (int i = 0; i < mods.Length; i++)
                {
                    string modAddition = mods[i] + ", ";

                    // If adding this mod would exceed the limit, send current message and start new one
                    if (currentMessage.Length + modAddition.Length > TWITCH_MESSAGE_MAX_LENGTH)
                    {
                        // Remove trailing comma and space
                        currentMessage = currentMessage.TrimEnd(',', ' ');
                        TwitchWrapper.SendChatMessage(currentMessage);
                        currentMessage = "";
                    }

                    currentMessage += modAddition;

                    // If this is the last mod, send the message
                    if (i == mods.Length - 1)
                    {
                        currentMessage = currentMessage.TrimEnd(',', ' ');
                        TwitchWrapper.SendChatMessage(currentMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                ToolkitLogger.Error($"Error in InstalledMods command: {ex}");
                TwitchWrapper.SendChatMessage($"@{twitchMessage.Username} Error retrieving mod list.");
            }
        }
    }
}

/**
using System;
using System.Linq;
using ToolkitCore;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class InstalledMods : CommandDriver
{
	public override void RunCommand(ITwitchMessage twitchMessage)
	{
		if ((DateTime.Now - Cooldowns.modsCommandCooldown).TotalSeconds <= 15.0)
		{
			return;
		}
		Cooldowns.modsCommandCooldown = DateTime.Now;
		string modmsg = "Version: " + Toolkit.Mod.Version + ", Mods: ";
		string[] mods = (from m in LoadedModManager.RunningMods
			select m.Name).ToArray();
		for (int i = 0; i < mods.Length; i++)
		{
			modmsg = modmsg + mods[i] + ", ";
			if (i == mods.Length - 1 || modmsg.Length > 256)
			{
				modmsg = modmsg.Substring(0, modmsg.Length - 2);
				TwitchWrapper.SendChatMessage(modmsg);
				modmsg = "";
			}
		}
	}
}
**/