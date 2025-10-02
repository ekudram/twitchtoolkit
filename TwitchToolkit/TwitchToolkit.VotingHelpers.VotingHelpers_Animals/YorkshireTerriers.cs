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
