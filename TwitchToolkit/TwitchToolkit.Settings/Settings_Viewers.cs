/*
 * File: Settings_Viewers.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 14, 2025
 * 
 * Summary of Changes:
 * 1. Reimplemented DoWindowContents method to use passed Listing_Standard
 * 2. Added Translate() calls for localization
 * 3. Added scroll view support for better UI organization
 * 4. Added descriptive text for each setting
 * 5. Improved layout with split regions for labels and fields
 * 6. Added conditional display for cost field
 */

using System;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace TwitchToolkit.Settings;

public static class Settings_Viewers
{
    private static Vector2 _scrollPosition = Vector2.zero;
    private static string _queueCostBuffer;
    private static string _subExtraCoinBuffer;
    private static string _subCoinMultiplierBuffer;
    private static string _subExtraVotesBuffer;
    private static string _vipExtraCoinBuffer;
    private static string _vipCoinMultiplierBuffer;
    private static string _vipExtraVotesBuffer;
    private static string _modExtraCoinBuffer;
    private static string _modCoinMultiplierBuffer;
    private static string _modExtraVotesBuffer;

    public static void DoWindowContents(Rect rect, Listing_Standard optionsListing)
    {
        // Create inner region and viewport for scrolling
        Rect innerRegion = new Rect(rect.x, rect.y, rect.width, rect.height - 30f);
        Rect viewPort = new Rect(0f, 0f, rect.width - 16f, 1200f); // Sufficient height for all content

        Widgets.BeginScrollView(innerRegion, ref _scrollPosition, viewPort);

        // Start a new listing inside the scroll view
        Listing_Standard scrollListing = new Listing_Standard();
        scrollListing.Begin(viewPort);

        // Edit Viewers button at the top
        Rect buttonRect = scrollListing.GetRect(30f);
        buttonRect.width = 150f;
        buttonRect.x = viewPort.width / 2f - buttonRect.width / 2f;

        if (Widgets.ButtonText(buttonRect, "Edit Viewers"))
        {
            Type type = typeof(Window_Viewers);
            Find.WindowStack.TryRemove(type, true);
            Window window = new Window_Viewers();
            Find.WindowStack.Add(window);
        }

        scrollListing.Gap(12f);
        scrollListing.GapLine(12f);

        // Viewer Queue Settings
        scrollListing.CheckboxLabeled("TwitchToolkitEnableViewerQueue".Translate(), ref ToolkitSettings.EnableViewerQueue, null);

        scrollListing.Label("TwitchToolkitEnableViewerQueueDesc".Translate(), -1f, "TwitchToolkitEnableViewerQueueDesc".Translate());
        scrollListing.Gap(4f);

        scrollListing.CheckboxLabeled("TwitchToolkitViewerColonistQueue".Translate(), ref ToolkitSettings.ViewerNamedColonistQueue, null);
        scrollListing.Label("TwitchToolkitViewerColonistQueueDesc".Translate(), -1f, "TwitchToolkitViewerColonistQueueDesc".Translate());
        scrollListing.Gap(4f);

        scrollListing.CheckboxLabeled("TwitchToolkitChargeViewersForQueue".Translate(), ref ToolkitSettings.ChargeViewersForQueue, null);
        scrollListing.Label("TwitchToolkitChargeViewersForQueueDesc".Translate(), -1f, "TwitchToolkitChargeViewersForQueueDesc".Translate());
        scrollListing.Gap(4f);

        if (ToolkitSettings.ChargeViewersForQueue)
        {
            (Rect costLabel, Rect costField) = scrollListing.Split(0.7f);
            Widgets.Label(costLabel, "TwitchToolkitCostToJoinQueue".Translate());
            _queueCostBuffer ??= ToolkitSettings.CostToJoinQueue.ToString();
            Widgets.TextFieldNumeric(costField, ref ToolkitSettings.CostToJoinQueue, ref _queueCostBuffer, 0f, 100000f);
            scrollListing.Gap(4f);
        }

        scrollListing.Gap(12f);
        scrollListing.GapLine(12f);

        // Special Viewers Section
        scrollListing.Label("TwitchToolkitSpecialViewers".Translate());
        scrollListing.Gap(12f);

        // Subscribers
        scrollListing.Label("<color=#D9BB25>TwitchToolkitSubscribers".Translate() + "</color>");
        scrollListing.Gap(6f);

        (Rect subCoinLabel, Rect subCoinField) = scrollListing.Split(0.7f);
        Widgets.Label(subCoinLabel, "TwitchToolkitSubscriberExtraCoins".Translate());
        _subExtraCoinBuffer ??= ToolkitSettings.SubscriberExtraCoins.ToString();
        Widgets.TextFieldNumeric(subCoinField, ref ToolkitSettings.SubscriberExtraCoins, ref _subExtraCoinBuffer, 0f, 100f);
        scrollListing.Gap(4f);

        (Rect subMultLabel, Rect subMultField) = scrollListing.Split(0.7f);
        Widgets.Label(subMultLabel, "TwitchToolkitSubscriberCoinMultiplier".Translate());
        _subCoinMultiplierBuffer ??= ToolkitSettings.SubscriberCoinMultiplier.ToString();
        Widgets.TextFieldNumeric(subMultField, ref ToolkitSettings.SubscriberCoinMultiplier, ref _subCoinMultiplierBuffer, 1f, 5f);
        scrollListing.Gap(4f);

        (Rect subVotesLabel, Rect subVotesField) = scrollListing.Split(0.7f);
        Widgets.Label(subVotesLabel, "TwitchToolkitSubscriberExtraVotes".Translate());
        _subExtraVotesBuffer ??= ToolkitSettings.SubscriberExtraVotes.ToString();
        Widgets.TextFieldNumeric(subVotesField, ref ToolkitSettings.SubscriberExtraVotes, ref _subExtraVotesBuffer, 0f, 100f);
        scrollListing.Gap(12f);

        // VIPs
        scrollListing.Label("<color=#5F49F2>TwitchToolkitVIPs".Translate() + "</color>");
        scrollListing.Gap(6f);

        (Rect vipCoinLabel, Rect vipCoinField) = scrollListing.Split(0.7f);
        Widgets.Label(vipCoinLabel, "TwitchToolkitVIPExtraCoins".Translate());
        _vipExtraCoinBuffer ??= ToolkitSettings.VIPExtraCoins.ToString();
        Widgets.TextFieldNumeric(vipCoinField, ref ToolkitSettings.VIPExtraCoins, ref _vipExtraCoinBuffer, 0f, 100f);
        scrollListing.Gap(4f);

        (Rect vipMultLabel, Rect vipMultField) = scrollListing.Split(0.7f);
        Widgets.Label(vipMultLabel, "TwitchToolkitVIPCoinMultiplier".Translate());
        _vipCoinMultiplierBuffer ??= ToolkitSettings.VIPCoinMultiplier.ToString();
        Widgets.TextFieldNumeric(vipMultField, ref ToolkitSettings.VIPCoinMultiplier, ref _vipCoinMultiplierBuffer, 1f, 5f);
        scrollListing.Gap(4f);

        (Rect vipVotesLabel, Rect vipVotesField) = scrollListing.Split(0.7f);
        Widgets.Label(vipVotesLabel, "TwitchToolkitVIPExtraVotes".Translate());
        _vipExtraVotesBuffer ??= ToolkitSettings.VIPExtraVotes.ToString();
        Widgets.TextFieldNumeric(vipVotesField, ref ToolkitSettings.VIPExtraVotes, ref _vipExtraVotesBuffer, 0f, 100f);
        scrollListing.Gap(12f);

        // Moderators
        scrollListing.Label("<color=#238C48>TwitchToolkitModerators".Translate() + "</color>");
        scrollListing.Gap(6f);

        (Rect modCoinLabel, Rect modCoinField) = scrollListing.Split(0.7f);
        Widgets.Label(modCoinLabel, "TwitchToolkitModExtraCoins".Translate());
        _modExtraCoinBuffer ??= ToolkitSettings.ModExtraCoins.ToString();
        Widgets.TextFieldNumeric(modCoinField, ref ToolkitSettings.ModExtraCoins, ref _modExtraCoinBuffer, 0f, 100f);
        scrollListing.Gap(4f);

        (Rect modMultLabel, Rect modMultField) = scrollListing.Split(0.7f);
        Widgets.Label(modMultLabel, "TwitchToolkitModCoinMultiplier".Translate());
        _modCoinMultiplierBuffer ??= ToolkitSettings.ModCoinMultiplier.ToString();
        Widgets.TextFieldNumeric(modMultField, ref ToolkitSettings.ModCoinMultiplier, ref _modCoinMultiplierBuffer, 1f, 5f);
        scrollListing.Gap(4f);

        (Rect modVotesLabel, Rect modVotesField) = scrollListing.Split(0.7f);
        Widgets.Label(modVotesLabel, "TwitchToolkitModExtraVotes".Translate());
        _modExtraVotesBuffer ??= ToolkitSettings.ModExtraVotes.ToString();
        Widgets.TextFieldNumeric(modVotesField, ref ToolkitSettings.ModExtraVotes, ref _modExtraVotesBuffer, 0f, 100f);

        scrollListing.End();
        Widgets.EndScrollView();
    }
}