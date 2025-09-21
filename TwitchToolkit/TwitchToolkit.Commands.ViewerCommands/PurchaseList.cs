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
	public override void RunCommand(ChatMessage ChatMessage)
	{
		TwitchWrapper.SendChatMessage((TaggedString)("@" + ChatMessage.Username + " " + Translator.Translate("TwitchToolkitPurchaseList") + (" " + ToolkitSettings.CustomPricingSheetLink)));
	}
}
