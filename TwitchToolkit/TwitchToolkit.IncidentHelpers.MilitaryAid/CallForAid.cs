/*
 * Project: TwitchToolkit
 * File: CallForAid.cs
 * 
 * 
 */

using RimWorld;
using ToolkitCore;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.IncidentHelpers.MilitaryAid;

public class CallForAid : IncidentHelper
{
    public override bool IsPossible()
    {
        return true;
    }

    public override void TryExecute()
    {
        Map currentMap = Find.CurrentMap;
        IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Special, (IIncidentTarget)(object)currentMap);
        incidentParms.forced = true;
        incidentParms.target = (IIncidentTarget)(object)currentMap;
        incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
        incidentParms.raidStrategy = RaidStrategyDefOf.ImmediateAttackFriendly;

        // Debug logging
        ToolkitLogger.Debug($"[CallForAid] Attempting to execute CallForAid incident");

        //ToolkitLogger.Debug($"[CallForAid] Points: {incidentParms.points}");
        //ToolkitLogger.Debug($"[CallForAid] RaidArrivalMode: {incidentParms.raidArrivalMode?.defName ?? "null"}");
        //ToolkitLogger.Debug($"[CallForAid] RaidStrategy: {incidentParms.raidStrategy?.defName ?? "null"}");

        IncidentWorker_CallForAid incident = new IncidentWorker_CallForAid
        {
            def = IncidentDef.Named("RaidFriendly")
        };

        bool success = ((IncidentWorker)incident).TryExecute(incidentParms);

        // Handle chat feedback here instead of in the incident worker
        if (!success)
        {
            TwitchWrapper.SendChatMessage($"@{this.Viewer} → Call for aid failed: No friendly factions available to help");
        }
        //else
        //{
           // nothing we sent it in the Incident worker.
           // string factionName = incidentParms.faction?.Name ?? "Allies";
           // TwitchWrapper.SendChatMessage($"@{this.Viewer} → Call for aid successful! {factionName} is sending reinforcements!");
        //}
    }
}