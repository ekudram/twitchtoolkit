/* 
 * Project: TwitchToolkit
 * File: ModInfo.cs
 * 
 * Usage: Command to display mod information to the user in chat using !modinfo
 * Updated to use TwitchMessageWrapper
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
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class ModInfo : CommandDriver
{
	public override void RunCommand(TwitchMessageWrapper messageWrapper)
	{
		TwitchWrapper.SendChatMessage((TaggedString)("@" + messageWrapper.Username + " " + Translator.Translate("TwitchToolkitModInfo") + " https://discord.gg/qrtg224 !"));
	}
}
