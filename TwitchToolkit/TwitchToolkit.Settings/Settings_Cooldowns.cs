/*
 * File: Settings_Cooldowns.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 15, 2025
 * 
 * Summary of Changes:
 * 1. Comprehensive cooldown and event limit settings
 * 2. Provides full event management capabilities
 * 3. Includes max events per interval limits for balance control
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
using UnityEngine;
using Verse;

namespace TwitchToolkit.Settings;

public static class Settings_Cooldowns
{
	public static void DoWindowContents(Rect rect, Listing_Standard optionsListing)
	{
		optionsListing.SliderLabeled("Days per cooldown period",  ref ToolkitSettings.EventCooldownInterval, Math.Round((double)ToolkitSettings.EventCooldownInterval).ToString(), 1f, 15f);
		optionsListing.Gap(12f);
		optionsListing.CheckboxLabeled("TwitchToolkitMaxEventsLimit".Translate(), ref ToolkitSettings.MaxEvents, null);
		optionsListing.Gap(12f);
		optionsListing.AddLabeledNumericalTextField("TwitchToolkitMaxBadEvents".Translate(), ref ToolkitSettings.MaxBadEventsPerInterval, 0.8f);
		optionsListing.AddLabeledNumericalTextField("TwitchToolkitMaxGoodEvents".Translate(), ref ToolkitSettings.MaxGoodEventsPerInterval, 0.8f);
		optionsListing.AddLabeledNumericalTextField("TwitchToolkitMaxNeutralEvents".Translate(), ref ToolkitSettings.MaxNeutralEventsPerInterval, 0.8f);
		optionsListing.AddLabeledNumericalTextField("TwitchToolkitMaxItemEvents".Translate(), ref ToolkitSettings.MaxCarePackagesPerInterval, 0.8f);
		optionsListing.Gap(12f);
		optionsListing.CheckboxLabeled("TwitchToolkitEventsHaveCooldowns".Translate(), ref ToolkitSettings.EventsHaveCooldowns, null);
	}
}
