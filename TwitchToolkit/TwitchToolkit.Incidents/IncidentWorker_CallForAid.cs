
/* IncidentWorker_CallForAid.cs
 * 
 * Updated: September 16, 2025
 * 
 * Change log:
 * 1. Line 128: This change checks if any worn apparel has a CompShield component instead of checking if the apparel itself
 * 2. Added debug logging for raid arrival mode resolution
 * 3. Added chat feedback for failures
 * 4. Now checks for Odessy space maps and uses drop pods if there is a faction that can do this.
 * 
 */

using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using ToolkitCore;

namespace TwitchToolkit.Incidents;

public class IncidentWorker_CallForAid : IncidentWorker_RaidFriendly
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return true;
    }

    protected override bool TryResolveRaidFaction(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (parms.faction != null)
        {
            return true;
        }

        // Get base candidate factions safely
        IEnumerable<Faction> baseFactions;
        try
        {
            baseFactions = base.CandidateFactions(parms, desperate: true);
        }
        catch (Exception e)
        {
            ToolkitLogger.Error($"[CallForAid] Error getting base candidate factions: {e}");
            return false;
        }

        if (baseFactions == null)
        {
            return false;
        }

        // Filter factions safely
        var validFactions = new List<Faction>();
        foreach (var faction in baseFactions)
        {
            if (faction == null)
                continue;

            try
            {
                // Skip player factions (including GravshipCrew)
                if (faction.def.isPlayer || faction.def == FactionDefOf.PlayerColony || faction.def == FactionDefOf.PlayerTribe)
                    continue;

                var relationKind = faction.PlayerRelationKind;
                var goodwill = faction.PlayerGoodwill;

                // Only include factions with good relations
                if ((int)relationKind >= 1)
                {
                    validFactions.Add(faction);
                }
            }
            catch (Exception e)
            {
                ToolkitLogger.Warning($"[CallForAid] Skipping faction {faction.Name} due to relation error: {e.Message}");
                continue;
            }
        }

        if (!validFactions.Any())
        {
            ToolkitLogger.Error("[CallForAid] No valid factions after filtering");
            return false;
        }

        parms.faction = GenCollection.RandomElementByWeight<Faction>(
            validFactions,
            (Func<Faction, float>)((Faction fac) => (float)fac.PlayerGoodwill + 120.000008f)
        );
        return true;
    }

    public override bool FactionCanBeGroupSource(Faction f, IncidentParms parms, bool desperate = true)
    {
        try
        {
            // Skip player factions (including GravshipCrew)
            if (f.def.isPlayer || f.def == FactionDefOf.PlayerColony || f.def == FactionDefOf.PlayerTribe)
                return false;

            return !f.def.hidden && (int)f.PlayerRelationKind >= 1;
        }
        catch (Exception e)
        {
            ToolkitLogger.Warning($"[CallForAid] Error in FactionCanBeGroupSource for {f.Name}: {e.Message}");
            return false;
        }
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {

        //ToolkitLogger.Debug($"[CallForAid] TryExecuteWorker started");
        ToolkitLogger.Debug($"[CallForAid] Target: {parms.target}");
        //ToolkitLogger.Debug($"[CallForAid] Faction: {parms.faction?.Name ?? "null"}");
        //ToolkitLogger.Debug($"[CallForAid] Points before resolve: {parms.points}");

        ResolveRaidPoints(parms);
        ToolkitLogger.Debug($"[CallForAid] Points after resolve: {parms.points}");

        if (!TryResolveRaidFaction(parms))
        {
            ToolkitLogger.Debug("[CallForAid] Failed to resolve raid faction");
            return false;
        }

        ToolkitLogger.Debug($"[CallForAid] Resolved faction: {parms.faction?.Name ?? "null"}");

        PawnGroupKindDef combat = PawnGroupKindDefOf.Combat;
        ResolveRaidStrategy(parms, combat);
        ToolkitLogger.Debug($"[CallForAid] Raid strategy: {parms.raidStrategy?.defName ?? "null"}");

        // Smart selection based on map type
        ToolkitLogger.Debug($"[CallForAid] Selecting raid arrival mode for map type");

        Map map = (Map)parms.target;

        // Get all arrival modes that can be used with current parameters
        var validArrivalModes = DefDatabase<PawnsArrivalModeDef>.AllDefs
            .Where(mode => mode.Worker.CanUseWith(parms))
            .ToList();

        ToolkitLogger.Debug($"[CallForAid] Valid arrival modes: {string.Join(", ", validArrivalModes.Select(m => m.defName))}");

        // Smart selection based on map type
        if (IsSpaceMap(map))
        {
            ToolkitLogger.Debug($"[CallForAid] Space map detected - forcing drop mode selection");

            // For space maps, ONLY use drop modes - no fallback to EdgeWalkIn
            var dropModes = validArrivalModes.Where(m =>
                m == PawnsArrivalModeDefOf.CenterDrop ||
                m == PawnsArrivalModeDefOf.EdgeDrop ||
                m == PawnsArrivalModeDefOf.RandomDrop ||
                m.defName.Contains("Drop") // Catch any other drop modes
            ).ToList();

            ToolkitLogger.Debug($"[CallForAid] Available drop modes: {string.Join(", ", dropModes.Select(m => m.defName))}");

            if (dropModes.Any())
            {
                // Prefer CenterDrop, then EdgeDrop, then any other drop mode
                parms.raidArrivalMode = dropModes.FirstOrDefault(m => m == PawnsArrivalModeDefOf.CenterDrop)
                                      ?? dropModes.FirstOrDefault(m => m == PawnsArrivalModeDefOf.EdgeDrop)
                                      ?? dropModes.FirstOrDefault(m => m == PawnsArrivalModeDefOf.RandomDrop)
                                      ?? dropModes.FirstOrDefault();
                ToolkitLogger.Debug($"[CallForAid] Selected drop mode: {parms.raidArrivalMode?.defName}");
            }
            else
            {
                // If no drop modes are available, we can't proceed in space
                ToolkitLogger.Error("[CallForAid] No valid drop modes available for space map - cannot execute");
                return false;
            }
            // Don't allow fallback to EdgeWalkIn for space maps - it won't work
        }
        else
        {
            // For normal maps, prefer edge walk-in
            parms.raidArrivalMode = validArrivalModes.FirstOrDefault(m => m == PawnsArrivalModeDefOf.EdgeWalkIn)
                                  ?? validArrivalModes.FirstOrDefault(m => m == PawnsArrivalModeDefOf.EdgeWalkInGroups)
                                  ?? validArrivalModes.FirstOrDefault()
                                  ?? PawnsArrivalModeDefOf.EdgeWalkIn;
            ToolkitLogger.Debug($"[CallForAid] Normal map detected, selected walk-in mode: {parms.raidArrivalMode?.defName}");
        }
        // Only do these checks for non-space maps
        if (!IsSpaceMap(map))
        {
            // FIX: Double-check that we have a valid arrival mode
            if (parms.raidArrivalMode == null)
            {
                ToolkitLogger.Debug($"[CallForAid] Raid arrival mode still null after selection, forcing EdgeWalkIn");
                parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            }

            // FIX: Check if the arrival mode can actually be used with current parameters
            if (!parms.raidArrivalMode.Worker.CanUseWith(parms))
            {
                ToolkitLogger.Debug($"[CallForAid] Selected arrival mode {parms.raidArrivalMode.defName} cannot be used, falling back to EdgeWalkIn");
                parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            }
        }

        parms.raidStrategy.Worker.TryGenerateThreats(parms);
        if (!parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
        {
            ToolkitLogger.Error("[CallForAid] Failed to resolve raid spawn center");
            return false;
        }

        parms.points = IncidentWorker_Raid.AdjustedRaidPoints(parms.points, parms.raidArrivalMode, parms.raidStrategy, parms.faction, combat);
        ToolkitLogger.Debug($"[CallForAid] Adjusted points: {parms.points}");

        List<Pawn> list = parms.raidStrategy.Worker.SpawnThreats(parms);

        if (list == null)
        {
            list = PawnGroupMakerUtility.GeneratePawns(IncidentParmsUtility.GetDefaultPawnGroupMakerParms(combat, parms, false), true).ToList();
            if (list.Count == 0)
            {
                ToolkitLogger.Error($"Got no pawns spawning raid from parms {parms}");
                return false;
            }
            parms.raidArrivalMode.Worker.Arrive(list, parms);
        }

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Points = " + parms.points.ToString("F0"));
        foreach (Pawn pawn in list)
        {
            string str = ((pawn.equipment != null && pawn.equipment.Primary != null) ? ((Entity)pawn.equipment.Primary).LabelCap : "unarmed");
            stringBuilder.AppendLine(pawn.KindLabel + " - " + str);
        }

        TaggedString baseLetterLabel = (TaggedString)(GetLetterLabel(parms));
        TaggedString baseLetterText = (TaggedString)(GetLetterText(parms, list));
        PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(list, ref baseLetterLabel, ref baseLetterText, GetRelatedPawnsInfoLetterText(parms), true, true);

        List<TargetInfo> list2 = new List<TargetInfo>();
        if (parms.pawnGroups != null)
        {
            List<List<Pawn>> list3 = IncidentParmsUtility.SplitIntoGroups(list, parms.pawnGroups);
            List<Pawn> list4 = GenCollection.MaxBy<List<Pawn>, int>((IEnumerable<List<Pawn>>)list3, (Func<List<Pawn>, int>)((List<Pawn> x) => x.Count));
            if (GenCollection.Any<Pawn>(list4))
            {
                list2.Add((TargetInfo)((Thing)(object)list4[0]));
            }
            for (int i = 0; i < list3.Count; i++)
            {
                if (list3[i] != list4 && GenCollection.Any<Pawn>(list3[i]))
                {
                    list2.Add((TargetInfo)((Thing)(object)list3[i][0]));
                }
            }
        }
        else if (GenCollection.Any<Pawn>(list))
        {
            foreach (Pawn t in list)
            {
                list2.Add((TargetInfo)((Thing)(object)t));
            }
        }

        SendStandardLetter(baseLetterLabel, baseLetterText, GetLetterDef(), parms, (LookTargets)(list2), Array.Empty<NamedArgument>());
        parms.raidStrategy.Worker.MakeLords(parms, list);
        LessonAutoActivator.TeachOpportunity(ConceptDefOf.EquippingWeapons, (OpportunityType)2);

        if (!PlayerKnowledgeDatabase.IsComplete(ConceptDefOf.ShieldBelts))
        {
            for (int j = 0; j < list.Count; j++)
            {
                // old code: f (GenCollection.Any<Apparel>(list[j].apparel.WornApparel, (Predicate<Apparel>)((Apparel ap) => ap is CompShield)))
                if (GenCollection.Any<Apparel>(list[j].apparel.WornApparel, (Predicate<Apparel>)((Apparel ap) => ap.TryGetComp<CompShield>() != null)))
                {
                    LessonAutoActivator.TeachOpportunity(ConceptDefOf.ShieldBelts, (OpportunityType)2);
                    break;
                }
            }
        }

        // SUCCESS MESSAGE
        string factionName = parms.faction?.Name ?? "Allies";
        string arrivalMode = parms.raidArrivalMode?.defName ?? "arriving";
        int pawnCount = list?.Count ?? 0;

        string successMessage = $"Call for aid successful! {factionName} is sending {pawnCount} reinforcements";
        if (arrivalMode.Contains("Drop"))
        {
            successMessage += " via drop pods";
        }
        else if (arrivalMode.Contains("EdgeWalk"))
        {
            successMessage += " marching to your location";
        }

        TwitchWrapper.SendChatMessage(successMessage);

        ToolkitLogger.Debug($"[CallForAid] TryExecuteWorker completed successfully");
        return true;
    }

    private bool IsSpaceMap(Map map)
    {
        if (map == null || map.Biome == null)
            return false;

        // Check for space biomes from Odyssey DLC
        return map.Biome.defName == "Space" ||
               map.Biome.defName == "Orbit";
    }
}
