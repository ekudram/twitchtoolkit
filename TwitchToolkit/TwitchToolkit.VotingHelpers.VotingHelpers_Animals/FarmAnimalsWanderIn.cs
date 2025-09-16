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
