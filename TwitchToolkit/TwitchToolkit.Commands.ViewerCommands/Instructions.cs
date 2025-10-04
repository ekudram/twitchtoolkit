/*
 * Project: Twitch Toolkit
 * File: Instructions.cs
 * 
 * Usage: Implements the !instructions command to provide viewers with information about the mod and available commands.
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
using ToolkitCore;
using TwitchToolkit.Utilities;

namespace TwitchToolkit.Commands.ViewerCommands;

public class Instructions : CommandDriver
{
   
        public override void RunCommand(TwitchMessageWrapper messageWrapper)
        {
            TwitchWrapper.SendChatMessage(MessageHelpers.GetInstructionsMessage(messageWrapper.Username));
        }
   
}
