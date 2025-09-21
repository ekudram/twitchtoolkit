/*
 * Project: TwitchToolkit
 * File: AnimalTame.cs
 * 
 * Usage: Incident helper to tame a random animal on the map
 * 
 */
using RimWorld;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.IncidentHelpers.Misc;

public class AnimalTame : IncidentHelper
{
	private IncidentParms parms = null;

	private IncidentWorker worker = null;

	public override bool IsPossible()
	{
		worker = (IncidentWorker)new IncidentWorker_SelfTame();
		worker.def = IncidentDef.Named("SelfTame");
		Map map = Helper.AnyPlayerMap;
		if (map != null)
		{
			parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, (IIncidentTarget)(object)map);
			bool canFire = worker.CanFireNow(parms);
			ToolkitLogger.Log("Can fire " + canFire);
			return canFire;
		}
		return false;
	}

	public override void TryExecute()
	{
		worker.TryExecute(parms);
	}
}
