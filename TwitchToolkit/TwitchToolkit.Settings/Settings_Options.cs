/*
 * File: Settings_Options.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 15, 2025
 */
using UnityEngine;
using Verse;

namespace TwitchToolkit.Settings;

public static class Settings_Options
{
	public static void DoWindowContents(Rect rect, Listing_Standard optionsListing)
	{
        Listing_Standard listing = new Listing_Standard();
        listing.Begin(rect);
        listing.Label("Options settings will be implemented here");
        listing.End();
    }
}
