/*
 * Project: TwitchToolkit
 * File: BuyPrisoner.cs
 */

/*
 * TwitchToolkit Mod - Community Preservation Fork
 * 
 * Original Source: TwitchToolkit (GNU Affero GPL v3)
 * Original Copyright: 2019 from original repository
 * 
 * MAJOR MODIFICATIONS � 2025 Captolamia:
 * Modifications listed above if any.
 * 
 * This work is licensed under GNU Affero GPL v3
 * This is a community preservation effort to maintain and improve
 * abandoned mod code for the benefit of all users.
 * 
 * See LICENSE file for full terms.
 */
using RimWorld;
using ToolkitCore;
using TwitchToolkit.Incidents;
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.IncidentHelpers.Special;

public class BuyPrisoner : IncidentHelperVariables
{
	private IncidentWorker_PrisonerJoins worker;

	private IncidentParms parms;

	private bool separateChannel;

	public override Viewer Viewer { get; set; }

	public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
	{
		this.separateChannel = separateChannel;
		Viewer = viewer;
		string[] command = message.Split(' ');
		GameComponentPawns gameComponent = Current.Game.GetComponent<GameComponentPawns>();
		if (gameComponent.HasUserBeenNamed(viewer.username))
		{
			TwitchWrapper.SendChatMessage("@" + viewer.username + " you are in colony and cannot be a prisoner.");
			return false;
		}
		worker = new IncidentWorker_PrisonerJoins(viewer);
		((IncidentWorker)worker).def = IncidentDefOf.WandererJoin;
		parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, (IIncidentTarget)(object)Helper.AnyPlayerMap);
		return true;
	}

	public override void TryExecute()
	{
		((IncidentWorker)worker).TryExecute(parms);
		Viewer.TakeViewerCoins(storeIncident.cost);
		Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);
		VariablesHelpers.SendPurchaseMessage("@" + Viewer.username + " has escaped from maximum security space prison.");
	}
}
