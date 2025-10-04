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
using RimWorld;
using Verse;

namespace TwitchToolkit.Incidents;

public class IncidentWorker_PrisonerJoins : IncidentWorker_WandererJoin
{
	private Viewer viewer;

	public IncidentWorker_PrisonerJoins(Viewer viewer)
	{
		this.viewer = viewer;
	}

	protected override bool TryExecuteWorker(IncidentParms parms)
	{
		Map map = (Map)parms.target;
		if (!TryFindEntryCell(map, out var _))
		{
			return false;
		}
		Gender? gender = null;
		if ((int)((IncidentWorker)this).def.pawnFixedGender > 0)
		{
			gender = ((IncidentWorker)this).def.pawnFixedGender;
		}
		PawnKindDef pawnKind = PawnKindDefOf.Slave;
		Faction ofAncients = Faction.OfAncients;
		bool pawnMustBeCapableOfViolence = ((IncidentWorker)this).def.pawnMustBeCapableOfViolence;
		Gender? fixedGender = gender;
		PawnGenerationRequest request = new PawnGenerationRequest(
			pawnKind, 
			ofAncients, 
			(PawnGenerationContext)2, 
			map.Tile, 
			false, 
			false, 
			false, 
			false, 
			true, 
			1f, 
			true, 
			false, 
			true, 
			true, 
			true, 
			false, 
			false, 
			false, 
			false, 
			0f, 
			0f, 
			(Pawn)null, 
			1f);
		List<Pawn> prisoners = new List<Pawn>();
		Pawn pawn = PawnGenerator.GeneratePawn(request);
		Name name = pawn.Name;
		NameTriple oldName = (NameTriple)(object)((name is NameTriple) ? name : null);
		NameTriple newName = new NameTriple(oldName.First, GenText.CapitalizeFirst(viewer.username), oldName.Last);
		pawn.Name = ((Name)(object)newName);
		pawn.guest.SetGuestStatus(Faction.OfPlayer, (GuestStatus)0);
		prisoners.Add(pawn);
		parms.raidArrivalMode = PawnsArrivalModeDefOf.CenterDrop;
		if (!parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
		{
			return false;
		}
		parms.raidArrivalMode.Worker.Arrive(prisoners, parms);
		TaggedString text = (TaggedString)("A prisoner named " + GenText.CapitalizeFirst(viewer.username) + " has escaped from maximum security space prison. Will you capture or let them go?");
		TaggedString label = (TaggedString)("Prisoner: " + GenText.CapitalizeFirst(viewer.username));
		PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref label, pawn);
		Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, (LookTargets)((Thing)(object)pawn), (Faction)null, (Quest)null, (List<ThingDef>)null, (string)null);
		return true;
	}

	private bool TryFindEntryCell(Map map, out IntVec3 cell)
	{
		return CellFinder.TryFindRandomEdgeCellWith((Predicate<IntVec3>)((IntVec3 c) => map.reachability.CanReachColony(c) && !GridsUtility.Fogged(c, map)), map, CellFinder.EdgeRoadChance_Neutral, out cell);
	}
}
