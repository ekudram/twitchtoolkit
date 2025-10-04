
/*
 * File: FarmAnimalsWanderIn.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 16, 2025
 * 
 * Changes: 	// private Map map; removed unused field
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

public class FarmAnimalsWanderIn : VotingHelper
{
	// private Map map;
	private IncidentWorker worker;
	private IncidentParms parms;
	public override bool IsPossible()
	{
		worker = (IncidentWorker)new IncidentWorker_FarmAnimalsWanderIn();
		worker.def = IncidentDef.Named("FarmAnimalsWanderIn");
		parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, target);
		return worker.CanFireNow(parms);
	}

	public override void TryExecute()
	{
		worker.TryExecute(parms);
	}
}
