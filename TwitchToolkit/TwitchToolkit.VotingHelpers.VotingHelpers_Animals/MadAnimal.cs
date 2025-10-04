/*
 * File: MadAnimal.cs
 * Project: TwitchToolkit
 * 
 * Changes:
 * 1. 
 */
/*
 * TwitchToolkit Mod - Community Preservation Fork
 * 
 * Original Source: TwitchToolkit (GNU Affero GPL v3)
 * Original Copyright: 2019 hodlhodl from original repository
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
