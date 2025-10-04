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
using System.Linq;
using RimWorld;
using TwitchToolkit.Votes;
using Verse;
using static Verse.DamageWorker;

namespace TwitchToolkit.VotingHelpers.VotingHelpers_Colonists;

public class Luciferium : VotingHelper
{
	private Map map;

	private Pawn pawn;

	public override bool IsPossible()
	{
		IIncidentTarget obj = target;
		Map map;
		if ((map = (Map)(object)((obj is Map) ? obj : null)) != null)
		{
			IIncidentTarget obj2 = target;
			map = (Map)(object)((obj2 is Map) ? obj2 : null);
			List<Pawn> candidates = map.mapPawns.FreeColonistsSpawned.ToList();
			if (candidates != null && candidates.Count > 0)
			{
				candidates.Shuffle();
				foreach (Pawn candidate in candidates)
				{
					if (!candidate.health.hediffSet.HasHediff(HediffDef.Named("LuciferiumAddiction"), false))
					{
						pawn = candidate;
						return true;
					}
				}
			}
		}
		return false;
	}

	public override void TryExecute()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0073: Unknown result type (might be due to invalid IL or missing erences)
		pawn.health.AddHediff(HediffDef.Named("LuciferiumHigh"), (BodyPartRecord)null, (DamageInfo?)null, (DamageResult)null);
		pawn.health.AddHediff(HediffDef.Named("LuciferiumAddiction"), (BodyPartRecord)null, (DamageInfo?)null, (DamageResult)null);
		string text = string.Concat(pawn.Name, " has taken a secret red pill they had hiding away. They are now addicted to Luciferium.");
		Find.LetterStack.ReceiveLetter((TaggedString)("Luciferium"), (TaggedString)(text), LetterDefOf.NegativeEvent, (LookTargets)((Thing)(object)pawn), (Faction)null, (Quest)null, (List<ThingDef>)null, (string)null);
	}
}
