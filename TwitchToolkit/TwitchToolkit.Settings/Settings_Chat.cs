/*
 * File: Settings_Chat.cs  
 * Project: TwitchToolkit
 * 
 * Updated: September 15, 2025
 * 
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
using UnityEngine;
using Verse;

namespace TwitchToolkit.Settings;

internal static class Settings_Chat
{
	public static void DoWindowContents(Rect rect, Listing_Standard optionsListing)
	{
		optionsListing.Label("Toolkit Chat Settings have moved to ToolkitCore Settings");
	}
}
