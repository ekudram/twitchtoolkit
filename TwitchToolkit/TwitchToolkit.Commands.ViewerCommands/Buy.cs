/*
 * File: ToggleCoins.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 20, 2025
 * 
 * Summary of Changes:
 * 1, 
 * 
 */

using System.Linq;
using TwitchLib.Client.Models;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class Buy : CommandDriver
{
	public override void RunCommand(ChatMessage ChatMessage)
	{
		ToolkitLogger.Warning("reached the override");
		Viewer viewer = Viewers.GetViewer(ChatMessage.Username);
		if (ChatMessage.Message.Split(' ').Count() >= 2)
		{
			Purchase_Handler.ResolvePurchase(viewer, ChatMessage);
		}
	}
}
