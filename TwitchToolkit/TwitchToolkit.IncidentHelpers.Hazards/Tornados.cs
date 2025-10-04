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

namespace TwitchToolkit.IncidentHelpers.Hazards;

public class Tornados : Tornado
{
	public override void TryExecute()
	{
		int count = 0;
		List<Thing> tornados = new List<Thing>();
		while (CellFinder.TryFindRandomCellInsideWith(cellRect, ((IntVec3 x) => CanSpawnTornadoAt(x, map)), out loc) && count < 3)
		{
			count++;
			Tornado tornado = (Tornado)GenSpawn.Spawn(ThingDef.Named("Tornado"), loc, map);
			tornados.Add((Thing)(object)tornado);
		}
		string text = "A  mobile, destructive vortex of violently rotating winds have appeard. Seek safe shelter!";
		Find.LetterStack.ReceiveLetter((TaggedString)("Tornados"), (TaggedString)(text), LetterDefOf.NegativeEvent, (LookTargets)(tornados));
	}
}
