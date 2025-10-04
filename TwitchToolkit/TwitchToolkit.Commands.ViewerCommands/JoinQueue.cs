/**
 * Project: TwitchToolkit
 * File: JoinQueue.cs
 * 
 * usage: Command to allow viewers to join the pawn naming queue using !joinqueue
 * 
 * */

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
