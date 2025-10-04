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
using RimWorld;
using Verse;

namespace TwitchToolkit.GameConditions;

public class GameCondition_VomitRain : GameCondition_Flashstorm
{
	public override void GameConditionTick()
	{
		IntVec3 newFilthLoc = CellFinderLoose.RandomCellWith((Predicate<IntVec3>)((IntVec3 sq) => GenGrid.Standable(sq, this.AffectedMaps[0]) && !this.AffectedMaps[0].roofGrid.Roofed(sq)), this.AffectedMaps[0], 1000);
		FilthMaker.TryMakeFilth(newFilthLoc, this.AffectedMaps[0], ThingDefOf.Filth_Vomit, 1, (FilthSourceFlags)0);
	}
}
