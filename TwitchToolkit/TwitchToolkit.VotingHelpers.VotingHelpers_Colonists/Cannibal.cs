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
using System;
using System.Collections.Generic;
using System.Linq;
using TwitchToolkit.Votes;
using Verse;

namespace TwitchToolkit.VotingHelpers.VotingHelpers_Colonists;

public class Cannibal : VotingHelper
{
	private Pawn pawn;

	private TraitDef traitDef;

	public override bool IsPossible()
	{
		IIncidentTarget obj = target;
		Map map;
		if ((map = (Map)(object)((obj is Map) ? obj : null)) != null)
		{
			IIncidentTarget obj2 = target;
			map = (Map)(object)((obj2 is Map) ? obj2 : null);
			List<Pawn> candidates = map.mapPawns.FreeColonistsSpawned.ToList();
            ToolkitLogger.Log("finding candidates");
			traitDef = DefDatabase<TraitDef>.GetNamed("Cannibal", true);
            ToolkitLogger.Log("finding specific candidate");
			if (candidates != null && candidates.Count > 0)
			{
                ToolkitLogger.Log("Randomizing Candiates");
				candidates.Shuffle();
				foreach (Pawn candidate in candidates)
				{
					if (pawn.story.traits.allTraits == null || pawn.story.traits.allTraits.Count > 3 || GenCollection.Any<Trait>(pawn.story.traits.allTraits, (Predicate<Trait>)((Trait s) => s.def == traitDef)))
					{
						continue;
					}
					pawn = candidate;
					return true;
				}
			}
		}
		return false;
	}

	public override void TryExecute()
	{
		Trait trait = new Trait();
		pawn.story.traits.GainTrait(trait);
		string text = string.Concat(pawn.Name, " has a sudden appetite for human flesh and now has the Cannibal Trait.");
		Find.LetterStack.ReceiveLetter((TaggedString)("Cannibal"), (TaggedString)(text), LetterDefOf.NeutralEvent, (LookTargets)((Thing)(object)pawn), (Faction)null, (Quest)null, (List<ThingDef>)null, (string)null);
	}
}
