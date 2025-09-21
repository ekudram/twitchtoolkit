/**
 * Project: TwitchToolkit
 * File: JoinQueue.cs
 * 
 * usage: Command to allow viewers to join the pawn naming queue using !joinqueue
 * 
 * */
using ToolkitCore;
using TwitchLib.Client.Models;
using TwitchToolkit.PawnQueue;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class JoinQueue : CommandDriver
{
	public override void RunCommand(ChatMessage ChatMessage)
	{
		Viewer viewer = Viewers.GetViewer(ChatMessage.Username);
		GameComponentPawns pawnComponent = Current.Game.GetComponent<GameComponentPawns>();
		if (pawnComponent.HasUserBeenNamed(ChatMessage.Username) || pawnComponent.UserInViewerQueue(ChatMessage.Username))
		{
			return;
		}
		if (ToolkitSettings.ChargeViewersForQueue)
		{
			if (viewer.GetViewerCoins() < ToolkitSettings.CostToJoinQueue)
			{
				TwitchWrapper.SendChatMessage($"@{ChatMessage.Username} you do not have enough coins to purchase a ticket, it costs {ToolkitSettings.CostToJoinQueue} and you have {viewer.GetViewerCoins()}.");
				return;
			}
			viewer.TakeViewerCoins(ToolkitSettings.CostToJoinQueue);
		}
		pawnComponent.AddViewerToViewerQueue(ChatMessage.Username);
		TwitchWrapper.SendChatMessage("@" + ChatMessage.Username + " you have purchased a ticket and are in the queue!");
	}
}
