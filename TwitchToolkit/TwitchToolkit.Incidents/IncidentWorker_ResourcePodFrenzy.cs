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
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TwitchToolkit.Incidents;

public class IncidentWorker_ResourcePodFrenzy : IncidentWorker
{
	private readonly string Quote;

	public IncidentWorker_ResourcePodFrenzy(string quote)
	{
		Quote = quote;
	}

	protected override bool TryExecuteWorker(IncidentParms parms)
	{
		Map map = (Map)parms.target;
		for (int x = 0; x < 10; x++)
		{
			List<Thing> things = ThingSetMakerDefOf.ResourcePod.root.Generate();
			IntVec3 intVec = DropCellFinder.RandomDropSpot(map, true);
			DropPodUtility.DropThingsNear(intVec, map, (IEnumerable<Thing>)things, 110, false, true, true, true);
		}
		TaggedString text = Translator.Translate("TwitchToolkitCargoPodFrenzyInc");
		if (Quote != null)
		{
			text += "\n\n";
			text += Helper.ReplacePlaceholder(Quote);
		}
		Find.LetterStack.ReceiveLetter(Translator.Translate("TwitchToolkitCargoPodFrenzyInc"), text, LetterDefOf.PositiveEvent, (LookTargets)null, (Faction)null, (Quest)null, (List<ThingDef>)null, (string)null);
		return true;
	}
}
