/*
 * TwitchToolkit Mod - Community Preservation Fork
 * 
 * Original Source: TwitchToolkit (GNU Affero GPL v3)
 * Original Copyright: 2019 hodlhodl from original repository
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

namespace TwitchToolkit.Storytellers;

public class Window_UristBotSettings : Window
{
	public Window_UristBotSettings()
	{
		base.doCloseButton = true;
	}

	public override void DoWindowContents(Rect inRect)
	{

		Listing_Standard listing = new Listing_Standard();
		((Listing)listing).Begin(inRect);
		Text.Font =((GameFont)2);
		listing.Label("<color=#CF0E0F>UristBot</color> Settings");
		Text.Font =((GameFont)1);
		((Listing)listing).Gap(12f);
		((Listing)listing).GapLine(12f);
		listing.Label("UristBot is still being developed. At the moment, it will make a small raid and let the viewers choose the raid strategy.");
		((Listing)listing).Gap(12f);
		string uristbotMTBDays = Math.Truncate((double)ToolkitSettings.UristBotMTBDays * 100.0 / 100.0).ToString();
		listing.TextFieldNumericLabeled<float>("Average Days Between Events", ref ToolkitSettings.UristBotMTBDays, ref uristbotMTBDays, 0.5f, 10f);
		((Listing)listing).End();
	}
}
