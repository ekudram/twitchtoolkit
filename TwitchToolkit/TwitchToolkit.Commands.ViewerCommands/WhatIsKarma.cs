/*
 * Project: TwitchToolkit
 * File: WhatIsKarma.cs
 * 
 * Updated: September 20, 2025
 * 
 * Purpose: Responds to viewers asking about their karma percentage in chat
 * <defName>WhatIsKarma</defName>
 * 
 * Summary of Changes:
 * 1. Added error handling with try-catch
 * 2. Added null checking for viewer
 * 3. Improved logging for debugging
 * 4. Used string interpolation for cleaner code
 */

using System;
using ToolkitCore;
using TwitchLib.Client.Models;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class WhatIsKarma : CommandDriver
{
    public override void RunCommand(TwitchMessageWrapper messageWrapper)
    {
        try
        {
            ToolkitLogger.Debug($"WhatIsKarma command requested by {messageWrapper.Username}");

            Viewer viewer = Viewers.GetViewer(messageWrapper.Username);
            if (viewer == null)
            {
                ToolkitLogger.Warning($"Could not find viewer for username: {messageWrapper.Username}");
                TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Error: Could not retrieve your karma information.");
                return;
            }

            int karma = viewer.GetViewerKarma();
            string message = $"@{viewer.username} {Translator.Translate("TwitchToolkitWhatIsKarma")} {karma}%";

            TwitchWrapper.SendChatMessage(message);
            ToolkitLogger.Debug($"Sent karma info to {viewer.username}: {karma}%");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in WhatIsKarma command: {ex.Message}");
            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Error retrieving your karma information.");
        }
    }
}