/*
 * Project: Twitch Toolkit
 * File: Instructions.cs
 * 
 * Usage: Implements the !instructions command to provide viewers with information about the mod and available commands.
 * 
 */

using ToolkitCore;
using TwitchLib.Client.Models;
using TwitchToolkit.Utilities;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class Instructions : CommandDriver
{
   
        public override void RunCommand(TwitchMessageWrapper messageWrapper)
        {
            TwitchWrapper.SendChatMessage(MessageHelpers.GetInstructionsMessage(messageWrapper.Username));
        }
   
}
