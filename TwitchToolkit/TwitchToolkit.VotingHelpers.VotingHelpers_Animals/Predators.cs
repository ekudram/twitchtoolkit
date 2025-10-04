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
using System.Collections.Generic;
using RimWorld;
using TwitchToolkit.Incidents;
using TwitchToolkit.Votes;
using Verse;

namespace TwitchToolkit.VotingHelpers.VotingHelpers_Animals;

public class Predators : VotingHelper
{
	private IncidentWorker worker;

	private IncidentParms parms;

	public override bool IsPossible()
	{
		List<string> animals = new List<string> { "Bear_Grizzly", "Bear_Polar", "Rhinoceros", "Elephant", "Megasloth", "Thrumbo" };
		animals.Shuffle();
		ThingDef def = ThingDef.Named(animals[0]);
		float averagePower = 0f;
		if (def != null && def.race != null)
		{
			foreach (Tool t in def.tools)
			{
				averagePower += t.power;
			}
			averagePower /= (float)def.tools.Count;
		}
		float animalCount = 2.5f;
		if (averagePower > 18f)
		{
			animalCount = 2f;
		}
		worker = (IncidentWorker)(object)new IncidentWorker_SpecificAnimalsWanderIn("TwitchStoriesLetterLabelPredators", PawnKindDef.Named(animals[0]), joinColony: false, (int)animalCount, manhunter: true, defaultText: true);
		worker.def = IncidentDef.Named("HerdMigration");
		parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, target);
		return worker.CanFireNow(parms);
	}

	public override void TryExecute()
	{
		worker.TryExecute(parms);
	}
}
