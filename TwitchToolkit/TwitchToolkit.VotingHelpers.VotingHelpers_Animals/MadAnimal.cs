/*
 * File: MadAnimal.cs
 * Project: TwitchToolkit
 * 
 * Changes:
 * 1. 
 */

using RimWorld;
using TwitchToolkit.Votes;
using Verse;

namespace TwitchToolkit.VotingHelpers.VotingHelpers_Animals;

public class MadAnimal : VotingHelper
{
	private Map map;
	private IncidentWorker worker;
	private IncidentParms parms;
	public override bool IsPossible()
	{

		if (target is Map)
		{
			Map reference =  map;
			IIncidentTarget obj = target;
            reference = (Map)(object)((obj is Map) ? obj : null);
			worker = (IncidentWorker)new IncidentWorker_AnimalInsanitySingle();
			worker.def = IncidentDef.Named("AnimalInsanitySingle");
			parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatSmall, (IIncidentTarget)(object)map);
			return worker.CanFireNow(parms);
		}
		return false;
	}

	public override void TryExecute()
	{
		worker.TryExecute(parms);
	}
}
