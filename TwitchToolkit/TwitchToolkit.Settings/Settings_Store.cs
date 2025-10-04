/*
 * File: Settings_Store.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 15, 2025
 * 
 * Summary of Changes:
 * 1. Centralized store settings implementation
 * 2. Provides store management UI with item and event editors
 * 3. Handles store-related configuration options
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
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace TwitchToolkit.Settings;

public static class Settings_Store
{
	public static void DoWindowContents(Rect rect, Listing_Standard optionsListing)
	{
		optionsListing.CheckboxLabeled("TwitchToolkitEarningCoins".Translate(), ref ToolkitSettings.EarningCoins, null);
		optionsListing.AddLabeledTextField("TwitchToolkitCustomPricingLink".Translate(), ref ToolkitSettings.CustomPricingSheetLink);
		optionsListing.Gap(12f);
		optionsListing.GapLine(12f);
		if (optionsListing.ButtonTextLabeled("Items Edit", "Open"))
		{
			Type type2 = typeof(StoreItemsWindow);
			Find.WindowStack.TryRemove(type2, true);
			Window window2 = new StoreItemsWindow();
			Find.WindowStack.Add(window2);
		}
		optionsListing.Gap(12f);
		optionsListing.GapLine(12f);
		if (optionsListing.ButtonTextLabeled("Events Edit", "Open"))
		{
			Type type = typeof(StoreIncidentsWindow);
			Find.WindowStack.TryRemove(type, true);
			Window window = new StoreIncidentsWindow();
			Find.WindowStack.Add(window);
		}
		optionsListing.Gap(12f);
		optionsListing.GapLine(12f);
		optionsListing.CheckboxLabeled("TwitchToolkitPurchaseConfirmations".Translate(), ref ToolkitSettings.PurchaseConfirmations, null);
		optionsListing.CheckboxLabeled("TwitchToolkitRepeatViewerNames".Translate(), ref ToolkitSettings.RepeatViewerNames, null);
		optionsListing.CheckboxLabeled("TwitchToolkitMinifiableBuildings".Translate(), ref ToolkitSettings.MinifiableBuildings, null);
	}
}
