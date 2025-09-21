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
                ToolkitCoreLogger.Debug("CheckUser command called with insufficient arguments");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Usage: !checkuser <username>");
                return;
            }

            string target = command[1].Replace("@", "");
            Viewer targeted = Viewers.GetViewer(target);

            if (targeted == null)
            {
                ToolkitCoreLogger.Debug($"CheckUser command: Targeted viewer '{target}' not found");
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