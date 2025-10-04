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

public class Window_BuyItem : Window
{
	private string traitsBuffer = "";

	public Window_BuyItem()
	{
		base.doCloseButton = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
        Listing_Standard listing = new Listing_Standard();
		((Listing)listing).Begin(inRect);
		listing.Label("Buy Item Settings");
		traitsBuffer = AddTraitSettings.maxTraits.ToString();
		listing.CheckboxLabeled("Should items be researched before being buyable?", ref BuyItemSettings.mustResearchFirst, (string)null);
		((Listing)listing).End();
	}
}
