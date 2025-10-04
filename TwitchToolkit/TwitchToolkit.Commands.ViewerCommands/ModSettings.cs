/*
 * Project: TwitchToolkit
 * File: ModSettings.cs
 * 
 * Usage: Command to display current mod settings to the user in chat using !modsettings
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
using TwitchLib.Client.Models;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class ModSettings : CommandDriver
{
	public override void RunCommand(TwitchMessageWrapper messageWrapper)
	{
		Command buyCommand = DefDatabase<Command>.GetNamed("Buy", true);
		string minutess = ((ToolkitSettings.CoinInterval > 1) ? "s" : "");
		string storeon = (TaggedString)(buyCommand.enabled ? Translator.Translate("TwitchToolkitOn") : Translator.Translate("TwitchToolkitOff"));
		string earningcoins = (TaggedString)(ToolkitSettings.EarningCoins ? Translator.Translate("TwitchToolkitOn") : Translator.Translate("TwitchToolkitOff"));
		string quote = (TaggedString)(Translator.Translate("TwitchToolkitModSettings"));
		string amount = ToolkitSettings.CoinAmount.ToString();
		string first = ToolkitSettings.CoinInterval.ToString();
		string second = storeon;
		string third = earningcoins;
		string stats_message = Helper.ReplacePlaceholder(quote, null, null, null, null, null, null, null, null, null, null, amount, null, null, null, ToolkitSettings.KarmaCap.ToString(), first, second, third);
		TwitchWrapper.SendChatMessage(stats_message);
	}
}
