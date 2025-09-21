/**
 * Project: TwitchToolkit
 * File: JoinQueue.cs
 * 
 * usage: Command to allow viewers to join the pawn naming queue using !joinqueue
 * 
 * */
using ToolkitCore;
using TwitchToolkit.PawnQueue;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class JoinQueue : CommandDriver
{
	public override void RunCommand(TwitchMessageWrapper messageWrapper)
	{
		Viewer viewer = Viewers.GetViewer(messageWrapper.Username);
		GameComponentPawns pawnComponent = Current.Game.GetComponent<GameComponentPawns>();
		if (pawnComponent.HasUserBeenNamed(messageWrapper.Username) || pawnComponent.UserInViewerQueue(messageWrapper.Username))
		{
			return;
		}
		if (ToolkitSettings.ChargeViewersForQueue)
		{
			if (viewer.GetViewerCoins() < ToolkitSettings.CostToJoinQueue)
			{
				TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} you do not have enough coins to purchase a ticket, it costs {ToolkitSettings.CostToJoinQueue} and you have {viewer.GetViewerCoins()}.");
				return;
			}
			viewer.TakeViewerCoins(ToolkitSettings.CostToJoinQueue);
		}
		pawnComponent.AddViewerToViewerQueue(messageWrapper.Username);
		TwitchWrapper.SendChatMessage("@" + messageWrapper.Username + " you have purchased a ticket and are in the queue!");
	}
}
