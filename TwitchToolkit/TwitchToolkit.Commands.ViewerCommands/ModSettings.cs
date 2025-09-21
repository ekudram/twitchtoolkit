/*
 * Project: TwitchToolkit
 * File: ModSettings.cs
 * 
 * Usage: Command to display current mod settings to the user in chat using !modsettings
 * 
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
