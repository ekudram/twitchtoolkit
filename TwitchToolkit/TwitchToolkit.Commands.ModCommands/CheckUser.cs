/*
 * File: CheckUser.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 20, 2025
 * 
 * Summary of Changes:
 * 1. Updated to use ChatMessage instead of ITwitchMessage for TwitchLib 3.4.0 compatibility
 * 2. Improved error handling with ToolkitLogger
 * 3. Added null checking for targeted viewer
 * 4. Added more descriptive error messages
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

namespace TwitchToolkit.Commands.ModCommands;

public class CheckUser : CommandDriver
{
    public override void RunCommand(TwitchMessageWrapper messageWrapper)
    {
        try
        {
            string[] command = messageWrapper.Message.Split(' ');
            if (command.Length < 2)
            {
                ToolkitLogger.Debug("CheckUser command called with insufficient arguments");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Usage: !checkuser <username>");
                return;
            }

            string target = command[1].Replace("@", "");
            Viewer targeted = Viewers.GetViewer(target);

            if (targeted == null)
            {
                ToolkitLogger.Debug($"CheckUser command: Targeted viewer '{target}' not found");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Viewer '{target}' not found.");
                return;
            }

            string response = Helper.ReplacePlaceholder(
                Translator.Translate("TwitchToolkitCheckUser"),
                null, null, null, null, null, null, null, null, null, null,
                viewer: targeted.username,
                amount: targeted.coins.ToString(),
                mod: null,
                newbalance: null,
                karma: targeted.GetViewerKarma().ToString()
            );

            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} {response}");
        }
        catch (Exception e)
        {
            ToolkitLogger.Error($"Error in CheckUser command: {e.Message}");
            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Error processing check user command.");
        }
    }
}