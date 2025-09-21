/*
 * Project: TwitchToolkit
 * File: PurchaseList.cs
 * 
 * Usage: Implements the !purchaselist command to provide viewers with a link to the custom pricing sheet.
 * 
 */

using ToolkitCore;
using TwitchLib.Client.Models;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class PurchaseList : CommandDriver
{
	public override void RunCommand(TwitchMessageWrapper messageWrapper)
	{
		TwitchWrapper.SendChatMessage((TaggedString)("@" + messageWrapper.Username + " " + Translator.Translate("TwitchToolkitPurchaseList") + (" " + ToolkitSettings.CustomPricingSheetLink)));
	}
}
