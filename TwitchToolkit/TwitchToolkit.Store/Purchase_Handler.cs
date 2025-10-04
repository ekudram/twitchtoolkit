/*
 * File: Purchase_Handler.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 20, 2025
 * 
 * Summary of Changes:
 * 1. Replaced ITwitchMessage with TwitchMessageWrapper for unified message handling
 * 2. Replaced Helper.Log calls with ToolkitLogger for consistent logging
 * 3. Improved error handling and null checking
 * 4. Enhanced code readability and maintainability
 * 5. Added more detailed debug logging for purchase processing
 * 
 * Regarding main thread assertions:
 * 1. RimWorld Object Access: Methods accessing RimWorld objects (like Current.Game, DefDatabase, etc.) should be called from the main thread
 * 2. Twitch Integration: Twitch message handling can run on background threads
 * 3. Synchronization: Use LongEventHandler when needed to ensure main thread execution
 */

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
using System.Text;
using ToolkitCore;
using TwitchToolkit.Incidents;
using Verse;

namespace TwitchToolkit.Store
{
    [StaticConstructorOnStartup]
    public static class Purchase_Handler
    {
        public static List<StoreIncidentSimple> allStoreIncidentsSimple;
        public static List<StoreIncidentVariables> allStoreIncidentsVariables;
        public static List<string> viewerNamesDoingVariableCommands;
        private static readonly object commandLock = new object();

        static Purchase_Handler()
        {
            try
            {
                allStoreIncidentsSimple = DefDatabase<StoreIncidentSimple>.AllDefs.ToList();
                allStoreIncidentsVariables = DefDatabase<StoreIncidentVariables>.AllDefs.ToList();
                ToolkitLogger.Log("Loaded store incidents after def database loaded");

                // Safely access mod settings
                if (Toolkit.Mod != null)
                {
                    var settings = ((Mod)Toolkit.Mod).GetSettings<ToolkitSettings>();
                    ToolkitLogger.Log("Toolkit settings loaded successfully");
                }
                else
                {
                    ToolkitLogger.Log("Initializing mod...");
                }

                viewerNamesDoingVariableCommands = new List<string>();
            }
            catch (Exception ex)
            {
                ToolkitLogger.Error("Error in Purchase_Handler static constructor: " + ex);
            }
        }

        public static void ResolvePurchase(Viewer viewer, TwitchMessageWrapper messageWrapper, bool separateChannel = false)
        {
            if (viewer == null)
            {
                ToolkitLogger.Warning("Null viewer in ResolvePurchase");
                return;
            }

            if (messageWrapper == null)
            {
                ToolkitLogger.Warning("Null messageWrapper in ResolvePurchase");
                return;
            }

            try
            {
                ToolkitLogger.Debug($"Processing purchase for viewer: {viewer.username}, message: {messageWrapper.Message}");

                List<string> command = messageWrapper.Message.Split(' ').ToList();
                if (command.Count < 2)
                {
                    TwitchWrapper.SendChatMessage($"@{viewer.username} Invalid command format. Usage: !buy <item>");
                    return;
                }

                // Handle !levelskill alias
                if (command[0] == "!levelskill")
                {
                    command[0] = "levelskill";
                    command.Insert(0, "!buy");
                }

                string productKey = command[1].ToLower();
                string formattedMessage = string.Join(" ", command.ToArray());

                // Find matching incident
                StoreIncidentSimple incident = allStoreIncidentsSimple.Find(s =>
                    productKey.ToLower() == s.abbreviation);

                if (incident != null)
                {
                    ResolvePurchaseSimple(viewer, messageWrapper, incident, formattedMessage);
                    return;
                }

                StoreIncidentVariables incidentVariables = allStoreIncidentsVariables.Find(s =>
                    productKey.ToLower() == s.abbreviation);

                if (incidentVariables != null)
                {
                    ResolvePurchaseVariables(viewer, messageWrapper, incidentVariables, formattedMessage);
                    return;
                }

                // Check for items
                Item item = StoreInventory.items.Find(s => s.abr == productKey);
                if (item != null)
                {
                    ToolkitLogger.Debug($"Found item: {item.defname} for product key: {productKey}");

                    List<string> commandSplit = messageWrapper.Message.Split(' ').ToList();
                    commandSplit.Insert(1, "item");

                    if (commandSplit.Count < 4)
                    {
                        commandSplit.Add("1");
                    }

                    if (!int.TryParse(commandSplit[3], out var _))
                    {
                        commandSplit.Insert(3, "1");
                    }

                    formattedMessage = string.Join(" ", commandSplit.ToArray());
                    ResolvePurchaseVariables(viewer, messageWrapper, StoreIncidentDefOf.Item, formattedMessage);
                    return;
                }

                ToolkitLogger.Debug($"Unknown item: {productKey} requested by {viewer.username}");
                TwitchWrapper.SendChatMessage($"@{viewer.username} Unknown item: {productKey}");
            }
            catch (Exception ex)
            {
                ToolkitLogger.Error($"Error in ResolvePurchase for viewer {viewer.username}: " + ex);
                TwitchWrapper.SendChatMessage($"@{viewer.username} Error processing your purchase.");
            }
        }
        public static void ResolvePurchaseSimple(Viewer viewer, TwitchMessageWrapper messageWrapper,
            StoreIncidentSimple incident, string formattedMessage, bool separateChannel = false)
        {
            if (incident == null)
            {
                ToolkitLogger.Warning("Null incident in ResolvePurchaseSimple");
                return;
            }

            int cost = incident.cost;
            ToolkitLogger.Debug($"Processing simple purchase: {incident.defName}, cost: {cost} for viewer: {viewer.username}");

            // Check all conditions before proceeding
            if (cost < 1 ||
                CheckIfViewerIsInVariableCommandList(viewer.username) ||
                !CheckIfViewerHasEnoughCoins(viewer, cost) ||
                CheckIfKarmaTypeIsMaxed(incident, viewer.username) ||
                CheckIfIncidentIsOnCooldown(incident, viewer.username))
            {
                return;
            }

            IncidentHelper helper = StoreIncidentMaker.MakeIncident(incident);
            if (helper == null)
            {
                ToolkitLogger.Warning("Missing helper for incident " + incident.defName);
                TwitchWrapper.SendChatMessage($"@{viewer.username} Error: Could not create incident helper.");
                return;
            }

            if (!helper.IsPossible())
            {
                TwitchWrapper.SendChatMessage($"@{viewer.username} {Translator.Translate("TwitchToolkitEventNotPossible")}");
                return;
            }

            // Process purchase
            if (!ToolkitSettings.UnlimitedCoins)
            {
                viewer.TakeViewerCoins(cost);
                ToolkitLogger.Debug($"Deducted {cost} coins from {viewer.username}. New balance: {viewer.GetViewerCoins()}");
            }

            Store_Component component = Current.Game.GetComponent<Store_Component>();
            helper.Viewer = viewer;
            helper.message = formattedMessage;

            // Use thread-safe enqueue
            lock (commandLock)
            {
                Ticker.IncidentHelpers.Enqueue(helper);
            }

            Store_Logger.LogPurchase(viewer.username, formattedMessage);
            component.LogIncident(incident);
            viewer.CalculateNewKarma(incident.karmaType, cost);

            if (ToolkitSettings.PurchaseConfirmations)
            {
                string message = Helper.ReplacePlaceholder(
                    Translator.Translate("TwitchToolkitEventPurchaseConfirm"),
                    null, null, null, null, null, null, null, null, null, null, null, null,
                    first: GenText.CapitalizeFirst(incident.label),
                    viewer: viewer.username
                );
                TwitchWrapper.SendChatMessage(message);
            }

            ToolkitLogger.Debug($"Successfully processed purchase of {incident.defName} for {viewer.username}");
        }

        public static void ResolvePurchaseVariables(Viewer viewer, TwitchMessageWrapper messageWrapper,
            StoreIncidentVariables incident, string formattedMessage, bool separateChannel = false)
        {
            if (incident == null)
            {
                ToolkitLogger.Warning("Null incident in ResolvePurchaseVariables");
                return;
            }

            int cost = incident.cost;
            ToolkitLogger.Debug($"Processing variable purchase: {incident.defName}, cost: {cost} for viewer: {viewer.username}");

            if ((cost < 1 && incident.defName != "Item") ||
                CheckIfViewerIsInVariableCommandList(viewer.username) ||
                !CheckIfViewerHasEnoughCoins(viewer, cost))
            {
                return;
            }

            if (incident != DefDatabase<StoreIncidentVariables>.GetNamed("Item", true))
            {
                if (CheckIfKarmaTypeIsMaxed(incident, viewer.username))
                {
                    return;
                }
            }
            else if (CheckIfCarePackageIsOnCooldown(viewer.username))
            {
                return;
            }

            if (CheckIfIncidentIsOnCooldown(incident, viewer.username))
            {
                return;
            }

            AddViewerToVariableCommandList(viewer.username);

            IncidentHelperVariables helper = StoreIncidentMaker.MakeIncidentVariables(incident);
            if (helper == null)
            {
                ToolkitLogger.Log("Missing helper for incident " + incident.defName);
                RemoveViewerFromVariableCommandList(viewer.username);
                return;
            }

            if (!helper.IsPossible(formattedMessage, viewer))
            {
                RemoveViewerFromVariableCommandList(viewer.username);
                return;
            }

            Store_Component component = Current.Game.GetComponent<Store_Component>();
            helper.Viewer = viewer;
            helper.message = formattedMessage;

            lock (commandLock)
            {
                Ticker.IncidentHelperVariables.Enqueue(helper);
            }

            Store_Logger.LogPurchase(viewer.username, messageWrapper.Message);
            component.LogIncident(incident);

            ToolkitLogger.Debug($"Successfully processed variable purchase of {incident.defName} for {viewer.username}");
        }
        public static bool CheckIfViewerIsInVariableCommandList(string username, bool separateChannel = false)
        {
            lock (commandLock)
            {
                if (viewerNamesDoingVariableCommands.Contains(username))
                {
                    TwitchWrapper.SendChatMessage($"@{username} you must wait for the game to unpause to buy something else.");
                    return true;
                }
            }
            return false;
        }

        public static bool CheckIfViewerHasEnoughCoins(Viewer viewer, int finalPrice, bool separateChannel = false)
        {
            if (!ToolkitSettings.UnlimitedCoins && viewer.GetViewerCoins() < finalPrice)
            {
                int currentCoins = viewer.GetViewerCoins();
                int coinsNeeded = finalPrice - currentCoins;
                TwitchWrapper.SendChatMessage(Helper.ReplacePlaceholder(
                    Translator.Translate("TwitchToolkitNotEnoughCoins"),
                    null, null, null, null, null, null, null, null, null, null,
                    viewer: viewer.username,
                    amount: finalPrice.ToString(),
                    mod: null,
                    newbalance: currentCoins.ToString(),
                    karma: null,
                    first: coinsNeeded.ToString()
                 ));

                ToolkitLogger.Debug($"Viewer {viewer.username} has {currentCoins} coins but needs {finalPrice} coins ({coinsNeeded} more needed)");
                return false;
            }

            ToolkitLogger.Debug($"Viewer {viewer.username} has sufficient coins: {viewer.GetViewerCoins()}/{finalPrice}");
            return true;
        }

        public static bool CheckIfKarmaTypeIsMaxed(StoreIncident incident, string username, bool separateChannel = false)
        {
            bool maxed = CheckTimesKarmaTypeHasBeenUsedRecently(incident);
            if (maxed)
            {
                Store_Component component = Current.Game.GetComponent<Store_Component>();
                TwitchWrapper.SendChatMessage($"@{username} {GenText.CapitalizeFirst(incident.label)} is maxed from karmatype, wait {component.DaysTillIncidentIsPurchaseable(incident)} days to purchase.");
            }
            return maxed;
        }

        public static bool CheckTimesKarmaTypeHasBeenUsedRecently(StoreIncident incident)
        {
            if (!ToolkitSettings.MaxEvents)
            {
                return false;
            }

            Store_Component component = Current.Game.GetComponent<Store_Component>();
            return incident.karmaType switch
            {
                KarmaType.Bad => component.KarmaTypesInLogOf(incident.karmaType) >= ToolkitSettings.MaxBadEventsPerInterval,
                KarmaType.Good => component.KarmaTypesInLogOf(incident.karmaType) >= ToolkitSettings.MaxGoodEventsPerInterval,
                KarmaType.Neutral => component.KarmaTypesInLogOf(incident.karmaType) >= ToolkitSettings.MaxNeutralEventsPerInterval,
                KarmaType.Doom => component.KarmaTypesInLogOf(incident.karmaType) >= ToolkitSettings.MaxBadEventsPerInterval,
                _ => false,
            };
        }

        public static bool CheckIfCarePackageIsOnCooldown(string username, bool separateChannel = false)
        {
            if (!ToolkitSettings.MaxEvents)
            {
                return false;
            }

            Store_Component component = Current.Game.GetComponent<Store_Component>();
            StoreIncidentVariables incident = DefDatabase<StoreIncidentVariables>.GetNamed("Item", true);

            if (component.IncidentsInLogOf(incident.abbreviation) >= ToolkitSettings.MaxCarePackagesPerInterval)
            {
                float daysTill = component.DaysTillIncidentIsPurchaseable(incident);
                TwitchWrapper.SendChatMessage($"@{username} care packages are on cooldown, wait {daysTill} day{(daysTill != 1f ? "s" : "")}.");
                return true;
            }
            return false;
        }

        public static bool CheckIfIncidentIsOnCooldown(StoreIncident incident, string username, bool separateChannel = false)
        {
            if (!ToolkitSettings.EventsHaveCooldowns)
            {
                return false;
            }

            Store_Component component = Current.Game.GetComponent<Store_Component>();
            bool maxed = component.IncidentsInLogOf(incident.abbreviation) >= incident.eventCap;

            if (maxed)
            {
                float days = component.DaysTillIncidentIsPurchaseable(incident);
                TwitchWrapper.SendChatMessage($"@{username} {GenText.CapitalizeFirst(incident.label)} is maxed, wait {days} day{(days != 1f ? "s" : "")} to purchase.");
            }
            return maxed;
        }

        // Thread-safe version of viewerNamesDoingVariableCommands modification
        public static void AddViewerToVariableCommandList(string username)
        {
            lock (commandLock)
            {
                if (!viewerNamesDoingVariableCommands.Contains(username))
                {
                    viewerNamesDoingVariableCommands.Add(username);
                }
            }
        }

        public static void RemoveViewerFromVariableCommandList(string username)
        {
            lock (commandLock)
            {
                viewerNamesDoingVariableCommands.Remove(username);
            }
        }

        public static void QueuePlayerMessage(Viewer viewer, string message, int variables = 0)
        {
            string colorCode = Viewer.GetViewerColorCode(viewer.username);
            string userNameTag = "<color=#" + colorCode + ">" + viewer.username + "</color>";
            string[] command = message.Split(' ');
            string output = "\n\n";

            if (command.Length - 2 == variables)
            {
                output = output + "<i>from</i> " + userNameTag;
            }
            else
            {
                output = output + userNameTag + ":";
                for (int i = 2 + variables; i < command.Length; i++)
                {
                    output = ((!(viewer.username.ToLower() == "hodlhodl") && !(viewer.username.ToLower() == "sirrandoo") && !(viewer.username.ToLower() == "saschahi")) ?
                        ((!viewer.IsSub) ?
                            ((!viewer.IsVIP) ?
                                ((!viewer.mod) ?
                                    (output + " " + command[i]) :
                                    (output + " " + ModText(command[i]))) :
                                (output + " " + VIPText(command[i]))) :
                            (output + " " + SubText(command[i]))) :
                        (output + " " + AdminText(command[i])));
                }
            }

            Helper.playerMessages.Add(output);
        }

        private static string AdminText(string input)
        {
            char[] chars = input.ToCharArray();
            StringBuilder output = new StringBuilder();

            foreach (char str in chars)
            {
                output.Append($"<color=#{Helper.GetRandomColorCode()}>{str}</color>");
            }

            return output.ToString();
        }

        private static string SubText(string input)
        {
            return "<color=#D9BB25>" + input + "</color>";
        }

        private static string VIPText(string input)
        {
            return "<color=#5F49F2>" + input + "</color>";
        }

        private static string ModText(string input)
        {
            return "<color=#238C48>" + input + "</color>";
        }
    }
}
