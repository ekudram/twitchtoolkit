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

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.IncidentHelpers.Misc;

public class Inspiration : IncidentHelper
{
	private bool successfulInspiration = false;

	private InspirationDef randomAvailableInspirationDef = null;

	public override bool IsPossible()
	{
		List<Pawn> pawns = Helper.AnyPlayerMap.mapPawns.FreeColonistsSpawned.ToList();
		pawns.Shuffle();
		foreach (Pawn pawn in pawns)
		{
			if (pawn.Inspired)
			{
				continue;
			}
			randomAvailableInspirationDef = GenCollection.RandomElementByWeightWithFallback<InspirationDef>(from x in DefDatabase<InspirationDef>.AllDefsListForReading
				where true
				select x, (Func<InspirationDef, float>)((InspirationDef x) => x.Worker.CommonalityFor(pawn)), (InspirationDef)null);
			if (randomAvailableInspirationDef != null)
			{
				successfulInspiration = pawn.mindState.inspirationHandler.TryStartInspiration(randomAvailableInspirationDef, (string)null, true);
				if (successfulInspiration)
				{
					break;
				}
			}
		}
		return successfulInspiration;
	}

	public override void TryExecute()
	{
	}
}
