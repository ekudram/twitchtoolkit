/*
 * File: Toolkitsettings.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 14, 2025
 * 
 * Summary of Changes:
 * 1. 12 tabs organized in 2 rows
 * 2. Each tab calls the correct drawing method
 * 3. Each drawing method uses Listing_Standard for layout
 * 4. All required drawing methods are implemented (even if some are just stubs for now)
 * 5. Added "Always Show Storyteller Button" option in Storyteller tab
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using TwitchToolkit.Settings;
using TwitchToolkit.Storytellers.StorytellerPackWindows;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace TwitchToolkit;

public class ToolkitSettings : ModSettings
{
    public enum SettingsTab
    {
        Chat,
        Coins,
        Storyteller,
        Patches,
        Store,
        Karma,
        Commands,
        Cooldowns,
        Options,
        Viewers,
        Integrations,
        Votes
    }

    public static bool FirstTimeInstallation = true;
    public static string Channel = "";
    public static string Username = "";
    public static string OAuth = "";
    public static bool AutoConnect = true;
    public static bool UseSeparateChatRoom = false;
    public static bool AllowBothChatRooms = false;
    public static bool WhispersGoToChatRoom = true;
    public static string ChannelID = "";
    public static string ChatroomUUID = "";
    public static int VoteTime = 2;
    public static int VoteOptions = 3;
    public static bool TimedStorytelling = false;
    public static int TimeBetweenStorytellingEvents = 10;
    public static bool VotingChatMsgs = false;
    public static bool VotingWindow = true;
    public static float VotingWindowx = -1f;
    public static float VotingWindowy = -1f;
    public static bool LargeVotingWindow = true;
    public static bool CommandsModsEnabled = true;
    public static int StartingBalance = 150;
    public static int CoinInterval = 2;
    public static int CoinAmount = 30;
    public static int MinimumPurchasePrice = 60;
    public static bool UnlimitedCoins = false;
    public static bool EarningCoins = true;
    public static string CustomPricingSheetLink = "bit.ly/toolkit-list";
    public static bool WhisperCmdsAllowed = true;
    public static bool WhisperCmdsOnly = false;
    public static bool PurchaseConfirmations = true;
    public static bool RepeatViewerNames = false;
    public static bool MinifiableBuildings = false;
    public static bool SyncStreamElements = false;
    public static string AccountID = "";
    public static string JWTToken = "";
    public static bool SyncStreamLabs = false;
    public static int StartingKarma = 100;
    public static int KarmaCap = 140;
    public static bool BanViewersWhoPurchaseAlwaysBad = true;
    public static bool KarmaReqsForGifting = false;
    public static int MinimumKarmaToRecieveGifts = 33;
    public static int MinimumKarmaToSendGifts = 100;
    public static int KarmaMinimum = 10;
    public static int TierOneGoodBonus = 16;
    public static int TierOneNeutralBonus = 36;
    public static int TierOneBadBonus = 24;
    public static int TierTwoGoodBonus = 10;
    public static int TierTwoNeutralBonus = 30;
    public static int TierTwoBadBonus = 20;
    public static int TierThreeGoodBonus = 10;
    public static int TierThreeNeutralBonus = 24;
    public static int TierThreeBadBonus = 18;
    public static int TierFourGoodBonus = 6;
    public static int TierFourNeutralBonus = 18;
    public static int TierFourBadBonus = 12;
    public static int DoomBonus = 67;
    public static bool ChatReqsForCoins = true;
    public static int TimeBeforeHalfCoins = 30;
    public static int TimeBeforeNoCoins = 60;
    public static bool KarmaDecay = false;
    public static int KarmaDecayPeriod = 5;
    public static int MaxBadEventsBeforeDecay = 0;
    public static int MaxGoodEventsBeforeDecay = 0;
    public static int MaxNeutralEventsBeforeDecay = 0;
    public static bool MaxEvents = false;
    public static int MaxEventsPeriod = 5;
    public static int MaxBadEventsPerInterval = 3;
    public static int MaxGoodEventsPerInterval = 10;
    public static int MaxNeutralEventsPerInterval = 10;
    public static int MaxCarePackagesPerInterval = 10;
    public static bool EventsHaveCooldowns = true;
    public static int EventCooldownInterval = 15;
    public static Dictionary<string, string> ViewerColorCodes = new Dictionary<string, string>();
    public static Dictionary<string, bool> ViewerModerators = new Dictionary<string, bool>();
    public static List<string> BannedViewers = new List<string>();
    public static bool EnableViewerQueue = true;
    public static bool ViewerNamedColonistQueue = true;
    public static bool ChargeViewersForQueue = false;
    public static int CostToJoinQueue = 0;
    public static int SubscriberExtraCoins = 10;
    public static float SubscriberCoinMultiplier = 1.25f;
    public static int SubscriberExtraVotes = 1;
    public static int VIPExtraCoins = 5;
    public static float VIPCoinMultiplier = 1.15f;
    public static int VIPExtraVotes = 0;
    public static int ModExtraCoins = 3;
    public static float ModCoinMultiplier = 1.05f;
    public static int ModExtraVotes = 0;
    public static Dictionary<string, int> VoteWeights = new Dictionary<string, int>();
    public static bool HodlBotEnabled = true;
    public static float HodlBotMTBDays = 1f;
    public static Dictionary<string, float> VoteTypeWeights = new Dictionary<string, float>();
    public static Dictionary<string, float> VoteCategoryWeights = new Dictionary<string, float>();
    public static bool ToryTalkerEnabled = false;
    public static float ToryTalkerMTBDays = 2f;
    public static bool UristBotEnabled = false;
    public static float UristBotMTBDays = 6f;
    public static bool MilasandraEnabled = false;
    public static bool MercuriusEnabled = false;
    public static List<string> CustomCommandDefs = new List<string>();
    public static bool NotifiedAboutUtils = false;

    // Added: Always show storyteller button setting
    public static bool AlwaysShowStorytellerButton = false;

    private static Vector2 scrollVector2;
    public static SettingsTab currentTab = SettingsTab.Chat;

    private static string[] PubliclyKnownBots = new string[69]
    {
        "0_applebadapple_0", "activeenergy", "Anotherttvviewer", "apricotdrupefruit", "avocadobadado", "bananennanen", "benutzer", "BloodLustr", "Chloescookieworld", "cleverusernameduh",
        "cogwhistle", "commanderroot", "commanderrott", "communityshowcase", "cutehealgirl", "danCry", "decafsmurf", "djcozby", "dosrev", "electricallongboard",
        "electricalskateboard", "faegwent", "freast", "freddyybot", "himekoelectric", "host_giveaway", "hostgiveaway", "jade_elephant_association", "laf21", "lanfusion",
        "llorx_falso", "luki4fun_bot_master", "M0psy", "mattmongaming", "mwmwmwmwmwmwmwmmwmwmwmwmw", "n0tahacker", "n0tahacker_", "n3td3v", "norkdorf", "nosebleedgg",
        "not47y", "ogqp", "p0sitivitybot", "philderbeast", "royalestreamers", "shoutgamers", "sickfold", "skinnyseahorse", "SkumShop", "slocool",
        "smallstreamersconnect", "spectre_807", "Stay_hydrated_bot", "Stockholm_Sweden", "StreamElixir", "StreamPromoteBot", "Subcentraldotnet", "Texastryhard", "teyd", "thatsprettyokay",
        "thelurkertv", "thronezilla", "tj_target", "twitchprimereminder", "uehebot", "v_and_k", "virgoproz", "woppes", "zanekyber"
    };

    // Tab system implementation
    private static Vector2 tabScrollPosition = Vector2.zero;
    private static float tabButtonWidth = 120f;
    private static float tabButtonHeight = 30f;
    private static Rect tabRect;
    private static Rect contentRect;

    public void DoWindowContents(Rect rect)
    {
        // Draw title
        Text.Font = GameFont.Medium;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(new Rect(0f, 0f, rect.width, 40f), Translator.Translate("TwitchToolkitSettingsTitle"));
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;

        // Setup tab and content areas
        float tabAreaHeight = tabButtonHeight * 2 + 4f; // Two rows + gap
        tabRect = new Rect(0f, 40f, rect.width, tabAreaHeight);
        contentRect = new Rect(0f, 40f + tabAreaHeight, rect.width, rect.height - 40f - tabAreaHeight);

        // Draw tabs
        DrawTabs(tabRect);

        // Draw content for selected tab
        DrawTabContent(contentRect);
    }

    private void DrawTabs(Rect rect)
    {
        // First row of tabs
        float xPos = 0f;
        float yPos = rect.y;

        // Coins tab
        if (Widgets.ButtonText(new Rect(xPos, yPos, tabButtonWidth, tabButtonHeight), "Coins"))
        {
            currentTab = SettingsTab.Coins;
        }
        xPos += tabButtonWidth;

        // Viewers tab
        if (Widgets.ButtonText(new Rect(xPos, yPos, tabButtonWidth, tabButtonHeight), "Viewers"))
        {
            currentTab = SettingsTab.Viewers;
        }
        xPos += tabButtonWidth;

        // Karma tab
        if (Widgets.ButtonText(new Rect(xPos, yPos, tabButtonWidth, tabButtonHeight), "Karma"))
        {
            currentTab = SettingsTab.Karma;
        }
        xPos += tabButtonWidth;

        // Storyteller tab
        if (Widgets.ButtonText(new Rect(xPos, yPos, tabButtonWidth, tabButtonHeight), "Storyteller"))
        {
            currentTab = SettingsTab.Storyteller;
        }
        xPos += tabButtonWidth;

        // Cooldowns tab
        if (Widgets.ButtonText(new Rect(xPos, yPos, tabButtonWidth, tabButtonHeight), "Cooldowns"))
        {
            currentTab = SettingsTab.Cooldowns;
        }
        xPos += tabButtonWidth;

        // Store tab
        if (Widgets.ButtonText(new Rect(xPos, yPos, tabButtonWidth, tabButtonHeight), "Store"))
        {
            currentTab = SettingsTab.Store;
        }

        // Second row of tabs
        xPos = 0f;
        yPos = rect.y + tabButtonHeight + 2f; // Add small gap between rows

        // Patches tab
        if (Widgets.ButtonText(new Rect(xPos, yPos, tabButtonWidth, tabButtonHeight), "Patches"))
        {
            currentTab = SettingsTab.Patches;
        }
        xPos += tabButtonWidth;

        // Chat tab
        if (Widgets.ButtonText(new Rect(xPos, yPos, tabButtonWidth, tabButtonHeight), "Chat"))
        {
            currentTab = SettingsTab.Chat;
        }
        xPos += tabButtonWidth;

        // Commands tab
        if (Widgets.ButtonText(new Rect(xPos, yPos, tabButtonWidth, tabButtonHeight), "Commands"))
        {
            currentTab = SettingsTab.Commands;
        }
        xPos += tabButtonWidth;

        // Options tab
        if (Widgets.ButtonText(new Rect(xPos, yPos, tabButtonWidth, tabButtonHeight), "Options"))
        {
            currentTab = SettingsTab.Options;
        }
        xPos += tabButtonWidth;

        // Integrations tab
        if (Widgets.ButtonText(new Rect(xPos, yPos, tabButtonWidth, tabButtonHeight), "Integrations"))
        {
            currentTab = SettingsTab.Integrations;
        }
        xPos += tabButtonWidth;

        // Votes tab
        if (Widgets.ButtonText(new Rect(xPos, yPos, tabButtonWidth, tabButtonHeight), "Votes"))
        {
            currentTab = SettingsTab.Votes;
        }
    }
    private void DrawTabContent(Rect rect)
    {
        switch (currentTab)
        {
            case SettingsTab.Chat:
                DrawChatSettings(rect);
                break;
            case SettingsTab.Coins:
                DrawCoinSettings(rect);
                break;
            case SettingsTab.Storyteller:
                DrawStorytellerSettings(rect);
                break;
            case SettingsTab.Patches:
                DrawPatchesSettings(rect);
                break;
            case SettingsTab.Store:
                DrawStoreSettings(rect);
                break;
            case SettingsTab.Karma:
                DrawKarmaSettings(rect);
                break;
            case SettingsTab.Viewers:
                DrawViewerSettings(rect);
                break;
            case SettingsTab.Cooldowns:
                DrawCooldownSettings(rect);
                break;
            case SettingsTab.Commands:
                DrawCommandSettings(rect);
                break;
            case SettingsTab.Options:
                DrawOptionsSettings(rect);
                break;
            case SettingsTab.Integrations:
                DrawIntegrationsSettings(rect);
                break;
            case SettingsTab.Votes:
                DrawVotesSettings(rect);
                break;
            default:
                Widgets.Label(rect, "Settings tab not implemented yet.");
                break;
        }
    }
    private void DrawChatSettings(Rect rect)
    {
        Listing_Standard listing = new Listing_Standard();
        listing.Begin(rect);

        // Use the existing coins settings implementation
        Settings_Chat.DoWindowContents(rect, listing);

        listing.End();
    }

    private void DrawCoinSettings(Rect rect)
    {
        Listing_Standard listing = new Listing_Standard();
        listing.Begin(rect);

        // Use the existing coins settings implementation
        Settings_Coins.DoWindowContents(rect, listing);

        listing.End();
    }

    private void DrawStorytellerSettings(Rect rect)
    {
        Listing_Standard listing = new Listing_Standard();
        listing.Begin(rect);

        // Use the external implementation - pass the listing we created
        Settings_Storyteller.DoWindowContents(rect, listing);

        listing.End();
    }

    private void DrawPatchesSettings(Rect rect)
    {
        Listing_Standard listing = new Listing_Standard();
        listing.Begin(rect);

        // Use the patches settings implementation but keep your UI structure
        Settings_Patches.DoWindowContents(rect, listing);

        listing.End();
    }

    private void DrawStoreSettings(Rect rect)
    {
        Listing_Standard listing = new Listing_Standard();
        listing.Begin(rect);

        // Use the store settings implementation
        Settings_Store.DoWindowContents(rect, listing);

        listing.End();
    }

    private void DrawKarmaSettings(Rect rect)
    {
        Listing_Standard listing = new Listing_Standard();
        listing.Begin(rect);

        // Use the existing karma settings implementation
        Settings_Karma.DoWindowContents(rect, listing);
        listing.End();
    }

    private void DrawViewerSettings(Rect rect)
    {

        // Calculate scroll view area
        Rect viewRect = new Rect(0f, 0f, rect.width - 16f, 1200f); // Adjust height based on content
        Vector2 scrollPosition = Vector2.zero;

        // Begin scroll view
        Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);
        Listing_Standard listing = new Listing_Standard();
        listing.Begin(rect);
    
        // Use the comprehensive viewers settings implementation
        Settings_Viewers.DoWindowContents(rect, listing);
    
        listing.End();
        Widgets.EndScrollView();
    }

    private void DrawCooldownSettings(Rect rect)
    {
        Listing_Standard listing = new Listing_Standard();
        listing.Begin(rect);

        // Use the comprehensive cooldowns settings implementation
        Settings_Cooldowns.DoWindowContents(rect, listing);

        listing.End();
    }
    private void DrawCommandSettings(Rect rect)
    {
        Listing_Standard listing = new Listing_Standard();
        listing.Begin(rect);

        // Use the existing karma settings implementation
        Settings_Commands.DoWindowContents(rect, listing);
        listing.End();
    }

    private void DrawOptionsSettings(Rect rect) // Fixed method name (was DrawOptionsSetting)
    {
        Listing_Standard listing = new Listing_Standard();
        listing.Begin(rect);

        // Use the existing karma settings implementation
        Settings_Options.DoWindowContents(rect, listing);
        listing.End();
    }

    private void DrawIntegrationsSettings(Rect rect)
    {
        Listing_Standard listing = new Listing_Standard();
        listing.Begin(rect);
        listing.Label("Integrations settings will be implemented here");
        listing.End();
    }

    private void DrawVotesSettings(Rect rect)
    {
        Listing_Standard listing = new Listing_Standard();
        listing.Begin(rect);
        listing.Label("Votes settings will be implemented here");
        listing.End();
    }
    public override void ExposeData()
    {
        // Expose all settings data here (unchanged from original)
        Scribe_Values.Look(ref FirstTimeInstallation, "FirstTimeInstallation", true);
        Scribe_Values.Look(ref Channel, "Channel", "");
        Scribe_Values.Look(ref Username, "Username", "");
        Scribe_Values.Look(ref OAuth, "OAuth", "");
        Scribe_Values.Look(ref AutoConnect, "AutoConnect", true);
        Scribe_Values.Look(ref UseSeparateChatRoom, "UseSeparateChatRoom", false);
        Scribe_Values.Look(ref AllowBothChatRooms, "AllowBothChatRooms", false);
        Scribe_Values.Look(ref WhispersGoToChatRoom, "WhispersGoToChatRoom", true);
        Scribe_Values.Look(ref ChannelID, "ChannelID", "");
        Scribe_Values.Look(ref ChatroomUUID, "ChatroomUUID", "");
        Scribe_Values.Look(ref VoteTime, "VoteTime", 2);
        Scribe_Values.Look(ref VoteOptions, "VoteOptions", 3);
        Scribe_Values.Look(ref TimedStorytelling, "TimedStorytelling", false);
        Scribe_Values.Look(ref TimeBetweenStorytellingEvents, "TimeBetweenStorytellingEvents", 10);
        Scribe_Values.Look(ref VotingChatMsgs, "VotingChatMsgs", false);
        Scribe_Values.Look(ref VotingWindow, "VotingWindow", true);
        Scribe_Values.Look(ref VotingWindowx, "VotingWindowx", -1f);
        Scribe_Values.Look(ref VotingWindowy, "VotingWindowy", -1f);
        Scribe_Values.Look(ref LargeVotingWindow, "LargeVotingWindow", true);
        Scribe_Values.Look(ref CommandsModsEnabled, "CommandsModsEnabled", true);
        Scribe_Values.Look(ref StartingBalance, "StartingBalance", 150);
        Scribe_Values.Look(ref CoinInterval, "CoinInterval", 2);
        Scribe_Values.Look(ref CoinAmount, "CoinAmount", 30);
        Scribe_Values.Look(ref MinimumPurchasePrice, "MinimumPurchasePrice", 60);
        Scribe_Values.Look(ref UnlimitedCoins, "UnlimitedCoins", false);
        Scribe_Values.Look(ref EarningCoins, "EarningCoins", true);
        Scribe_Values.Look(ref CustomPricingSheetLink, "CustomPricingSheetLink", "bit.ly/toolkit-list");
        Scribe_Values.Look(ref WhisperCmdsAllowed, "WhisperCmdsAllowed", true);
        Scribe_Values.Look(ref WhisperCmdsOnly, "WhisperCmdsOnly", false);
        Scribe_Values.Look(ref PurchaseConfirmations, "PurchaseConfirmations", true);
        Scribe_Values.Look(ref RepeatViewerNames, "RepeatViewerNames", false);
        Scribe_Values.Look(ref MinifiableBuildings, "MinifiableBuildings", false);
        Scribe_Values.Look(ref SyncStreamElements, "SyncStreamElements", false);
        Scribe_Values.Look(ref AccountID, "AccountID", "");
        Scribe_Values.Look(ref JWTToken, "JWTToken", "");
        Scribe_Values.Look(ref SyncStreamLabs, "SyncStreamLabs", false);
        Scribe_Values.Look(ref StartingKarma, "StartingKarma", 100);
        Scribe_Values.Look(ref KarmaCap, "KarmaCap", 140);
        Scribe_Values.Look(ref BanViewersWhoPurchaseAlwaysBad, "BanViewersWhoPurchaseAlwaysBad", true);
        Scribe_Values.Look(ref KarmaReqsForGifting, "KarmaReqsForGifting", false);
        Scribe_Values.Look(ref MinimumKarmaToRecieveGifts, "MinimumKarmaToRecieveGifts", 33);
        Scribe_Values.Look(ref MinimumKarmaToSendGifts, "MinimumKarmaToSendGifts", 100);
        Scribe_Values.Look(ref KarmaMinimum, "KarmaMinimum", 10);
        Scribe_Values.Look(ref TierOneGoodBonus, "TierOneGoodBonus", 16);
        Scribe_Values.Look(ref TierOneNeutralBonus, "TierOneNeutralBonus", 36);
        Scribe_Values.Look(ref TierOneBadBonus, "TierOneBadBonus", 24);
        Scribe_Values.Look(ref TierTwoGoodBonus, "TierTwoGoodBonus", 10);
        Scribe_Values.Look(ref TierTwoNeutralBonus, "TierTwoNeutralBonus", 30);
        Scribe_Values.Look(ref TierTwoBadBonus, "TierTwoBadBonus", 20);
        Scribe_Values.Look(ref TierThreeGoodBonus, "TierThreeGoodBonus", 10);
        Scribe_Values.Look(ref TierThreeNeutralBonus, "TierThreeNeutralBonus", 24);
        Scribe_Values.Look(ref TierThreeBadBonus, "TierThreeBadBonus", 18);
        Scribe_Values.Look(ref TierFourGoodBonus, "TierFourGoodBonus", 6);
        Scribe_Values.Look(ref TierFourNeutralBonus, "TierFourNeutralBonus", 18);
        Scribe_Values.Look(ref TierFourBadBonus, "TierFourBadBonus", 12);
        Scribe_Values.Look(ref DoomBonus, "DoomBonus", 67);
        Scribe_Values.Look(ref ChatReqsForCoins, "ChatReqsForCoins", true);
        Scribe_Values.Look(ref TimeBeforeHalfCoins, "TimeBeforeHalfCoins", 30);
        Scribe_Values.Look(ref TimeBeforeNoCoins, "TimeBeforeNoCoins", 60);
        Scribe_Values.Look(ref KarmaDecay, "KarmaDecay", false);
        Scribe_Values.Look(ref KarmaDecayPeriod, "KarmaDecayPeriod", 5);
        Scribe_Values.Look(ref MaxBadEventsBeforeDecay, "MaxBadEventsBeforeDecay", 0);
        Scribe_Values.Look(ref MaxGoodEventsBeforeDecay, "MaxGoodEventsBeforeDecay", 0);
        Scribe_Values.Look(ref MaxNeutralEventsBeforeDecay, "MaxNeutralEventsBeforeDecay", 0);
        Scribe_Values.Look(ref MaxEvents, "MaxEvents", false);
        Scribe_Values.Look(ref MaxEventsPeriod, "MaxEventsPeriod", 5);
        Scribe_Values.Look(ref MaxBadEventsPerInterval, "MaxBadEventsPerInterval", 3);
        Scribe_Values.Look(ref MaxGoodEventsPerInterval, "MaxGoodEventsPerInterval", 10);
        Scribe_Values.Look(ref MaxNeutralEventsPerInterval, "MaxNeutralEventsPerInterval", 10);
        Scribe_Values.Look(ref MaxCarePackagesPerInterval, "MaxCarePackagesPerInterval", 10);
        Scribe_Values.Look(ref EventsHaveCooldowns, "EventsHaveCooldowns", true);
        Scribe_Values.Look(ref EventCooldownInterval, "EventCooldownInterval", 15);
        Scribe_Collections.Look(ref ViewerColorCodes, "ViewerColorCodes", LookMode.Value, LookMode.Value);
        Scribe_Collections.Look(ref ViewerModerators, "ViewerModerators", LookMode.Value, LookMode.Value);
        Scribe_Collections.Look(ref BannedViewers, "BannedViewers", LookMode.Value);
        Scribe_Values.Look(ref EnableViewerQueue, "EnableViewerQueue", true);
        Scribe_Values.Look(ref ViewerNamedColonistQueue, "ViewerNamedColonistQueue", true);
        Scribe_Values.Look(ref ChargeViewersForQueue, "ChargeViewersForQueue", false);
        Scribe_Values.Look(ref CostToJoinQueue, "CostToJoinQueue", 0);
        Scribe_Values.Look(ref SubscriberExtraCoins, "SubscriberExtraCoins", 10);
        Scribe_Values.Look(ref SubscriberCoinMultiplier, "SubscriberCoinMultiplier", 1.25f);
        Scribe_Values.Look(ref SubscriberExtraVotes, "SubscriberExtraVotes", 1);
        Scribe_Values.Look(ref VIPExtraCoins, "VIPExtraCoins", 5);
        Scribe_Values.Look(ref VIPCoinMultiplier, "VIPCoinMultiplier", 1.15f);
        Scribe_Values.Look(ref VIPExtraVotes, "VIPExtraVotes", 0);
        Scribe_Values.Look(ref ModExtraCoins, "ModExtraCoins", 3);
        Scribe_Values.Look(ref ModCoinMultiplier, "ModCoinMultiplier", 1.05f);
        Scribe_Values.Look(ref ModExtraVotes, "ModExtraVotes", 0);
        Scribe_Collections.Look(ref VoteWeights, "VoteWeights", LookMode.Value, LookMode.Value);
        Scribe_Values.Look(ref HodlBotEnabled, "HodlBotEnabled", true);
        Scribe_Values.Look(ref HodlBotMTBDays, "HodlBotMTBDays", 1f);
        Scribe_Collections.Look(ref VoteTypeWeights, "VoteTypeWeights", LookMode.Value, LookMode.Value);
        Scribe_Collections.Look(ref VoteCategoryWeights, "VoteCategoryWeights", LookMode.Value, LookMode.Value);
        Scribe_Values.Look(ref ToryTalkerEnabled, "ToryTalkerEnabled", false);
        Scribe_Values.Look(ref ToryTalkerMTBDays, "ToryTalkerMTBDays", 2f);
        Scribe_Values.Look(ref UristBotEnabled, "UristBotEnabled", false);
        Scribe_Values.Look(ref UristBotMTBDays, "UristBotMTBDays", 6f);
        Scribe_Values.Look(ref MilasandraEnabled, "MilasandraEnabled", false);
        Scribe_Values.Look(ref MercuriusEnabled, "MercuriusEnabled", false);
        Scribe_Collections.Look(ref CustomCommandDefs, "CustomCommandDefs", LookMode.Value);
        Scribe_Values.Look(ref NotifiedAboutUtils, "NotifiedAboutUtils", false);

        // Added: Expose the new setting
        Scribe_Values.Look(ref AlwaysShowStorytellerButton, "AlwaysShowStorytellerButton", false);
    }
}

