/*
 * File: HerdMigration.cs
 * Project: TwitchToolkit
 * 
 * Changes:
 * 1.  // private Map map; removed unused field
 * 2. removed using Verse; as it was unused
 * 
 */
using RimWorld;
using TwitchToolkit.Votes;

namespace TwitchToolkit.VotingHelpers.VotingHelpers_Animals;

public class HerdMigration : VotingHelper
{
	// private Map map;

	private IncidentWorker worker;

	private IncidentParms parms;

	public override bool IsPossible()
	{
		worker = (IncidentWorker)new IncidentWorker_HerdMigration();
		worker.def = IncidentDef.Named("HerdMigration");
		parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, target);
		return worker.CanFireNow(parms);
	}

	public override void TryExecute()
	{
		worker.TryExecute(parms);
	}
}
