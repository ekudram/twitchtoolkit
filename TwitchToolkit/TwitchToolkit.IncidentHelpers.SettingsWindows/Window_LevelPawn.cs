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
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using UnityEngine;
using Verse;

namespace TwitchToolkit.IncidentHelpers.SettingsWindows;

public class Window_LevelPawn : Window
{
	private string xpBuffer = "";

	public Window_LevelPawn()
	{
		base.doCloseButton = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
        Listing_Standard listing = new Listing_Standard();
		((Listing)listing).Begin(inRect);
		listing.Label("Level Pawn Settings");
		xpBuffer = LevelPawnSettings.xpMultiplier.ToString();
		listing.TextFieldNumericLabeled<float>("XP Multiplier", ref LevelPawnSettings.xpMultiplier, ref xpBuffer, 0.5f, 5f);
		((Listing)listing).End();
	}
}
