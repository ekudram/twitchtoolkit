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
using RimWorld;
using ToolkitCore;
using TwitchToolkit.Store;
using Verse;
using IncidentWorker_RaidEnemy = RimWorld.IncidentWorker_RaidEnemy;

namespace TwitchToolkit.IncidentHelpers.Raids;

public class Raid : IncidentHelperVariables
{
	public int pointsWager = 0;

	public IIncidentTarget target = null;

	public IncidentWorker worker = null;

	public IncidentParms parms = null;

	private bool separateChannel = false;

	public override Viewer Viewer { get; set; }

	public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
	{
		this.separateChannel = separateChannel;
		Viewer = viewer;
		string[] command = message.Split(' ');
		if (command.Length < 3)
		{
			TwitchWrapper.SendChatMessage("@" + viewer.username + " syntax is " + storeIncident.syntax);
			return false;
		}
		if (!VariablesHelpers.PointsWagerIsValid(command[2], viewer,  ref pointsWager, ref storeIncident, separateChannel))
		{
			return false;
		}
		target = (IIncidentTarget)(object)Current.Game.AnyPlayerHomeMap;
		if (target == null)
		{
			ToolkitLogger.Debug("Raid is possilble no map!");
			return false;
		}
		parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatSmall, target);
		parms.points = IncidentHelper_PointsHelper.RollProportionalGamePoints(storeIncident, pointsWager, parms.points);
		parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
		parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
		worker = new IncidentWorker_RaidEnemy();
		worker.def = IncidentDefOf.RaidEnemy;
		return worker.CanFireNow(parms);
	}

	public override void TryExecute()
	{
		if (worker.TryExecute(parms))
		{
			Viewer.TakeViewerCoins(pointsWager);
			Viewer.CalculateNewKarma(storeIncident.karmaType, pointsWager);
			VariablesHelpers.SendPurchaseMessage($"Starting raid with {pointsWager} points wagered and {(int)parms.points} raid points purchased by {Viewer.username}");
		}
		else
		{
			TwitchWrapper.SendChatMessage("@" + Viewer.username + " could not generate parms for raid.");
		}
	}
}
