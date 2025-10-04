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
using ToolkitCore;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.IncidentHelpers.Special;

public class Inspiration : IncidentHelperVariables
{
	public bool separateChannel = false;

	public override Viewer Viewer { get; set; }

	public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
	{
		this.separateChannel = separateChannel;
		string[] command = message.Split(' ');
		if (command.Length - 2 < storeIncident.variables)
		{
			TwitchWrapper.SendChatMessage("@" + viewer.username + " syntax is " + storeIncident.syntax);
			return false;
		}
		return true;
	}

	public override void TryExecute()
	{
		List<Pawn> pawns = Helper.AnyPlayerMap.mapPawns.FreeColonistsSpawned.ToList();
		pawns.Shuffle();
		bool successfulInspiration = false;
		foreach (Pawn pawn in pawns)
		{
			if (pawn.Inspired)
			{
				continue;
			}
			InspirationDef randomAvailableInspirationDef = GenCollection.RandomElementByWeightWithFallback<InspirationDef>(from x in DefDatabase<InspirationDef>.AllDefsListForReading
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
		if (successfulInspiration)
		{
			Viewer.TakeViewerCoins(storeIncident.cost);
			Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);
			VariablesHelpers.SendPurchaseMessage("@" + Viewer.username + " purchased a random inspiration.");
		}
		else
		{
			VariablesHelpers.SendPurchaseMessage("@" + Viewer.username + " attempted to inspired a pawn, but none were successful.");
		}
	}
}
