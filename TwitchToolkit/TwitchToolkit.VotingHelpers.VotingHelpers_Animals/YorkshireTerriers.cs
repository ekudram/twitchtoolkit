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
using TwitchToolkit.Incidents;
using TwitchToolkit.Votes;
using Verse;

namespace TwitchToolkit.VotingHelpers.VotingHelpers_Animals;

public class YorkshireTerriers : VotingHelper
{
	private readonly Map map;

	private IncidentWorker worker;

	private IncidentParms parms;

	private static readonly IntRange AnimalsCount = new IntRange(3, 7);

	public override bool IsPossible()
	{
		PawnKindDef animalKind = PawnKindDef.Named("YorkshireTerrier");
		IntRange animalsCount = AnimalsCount;
		int num = ((IntRange)( animalsCount)).RandomInRange;
		worker = (IncidentWorker)(object)new IncidentWorker_SpecificAnimalsWanderIn(null, animalKind, joinColony: true, num, manhunter: false, defaultText: true);
		worker.def = IncidentDef.Named("SelfTame");
		parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, target);
		return worker.CanFireNow(parms);
	}

	public override void TryExecute()
	{
		worker.TryExecute(parms);
	}
}
