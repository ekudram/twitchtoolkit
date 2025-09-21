/* 
 * Project: TwitchToolkit
 * File: ModInfo.cs
 * 
 * Usage: Command to display mod information to the user in chat using !modinfo
 * 
 */
using ToolkitCore;
using TwitchLib.Client.Models;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class ModInfo : CommandDriver
{
	public override void RunCommand(TwitchMessageWrapper messageWrapper)
	{
		TwitchWrapper.SendChatMessage((TaggedString)("@" + messageWrapper.Username + " " + Translator.Translate("TwitchToolkitModInfo") + " https://discord.gg/qrtg224 !"));
	}
}
