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

namespace TwitchToolkit.Storytellers.StorytellerPackWindows;

public class Window_ToryTalkerSettings : Window
{
	public Window_ToryTalkerSettings()
	{
		base.doCloseButton = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		Listing_Standard listing = new Listing_Standard();
		((Listing)listing).Begin(inRect);
		Text.Font = ((GameFont)2);
		listing.Label("<color=#6441A4>ToryTalker</color> Settings");
		Text.Font =((GameFont)1);
		((Listing)listing).Gap(12f);
		((Listing)listing).GapLine(12f);
		listing.Label("Tory Talker uses the global weights and it's own weighting system based on events that have happened recently.");
		((Listing)listing).Gap(12f);
		if (listing.ButtonTextLabeled("Edit Global Vote Weights", "Edit Weights"))
		{
			Window_GlobalVoteWeights window = new Window_GlobalVoteWeights();
			Find.WindowStack.TryRemove(((object)window).GetType(), true);
			Find.WindowStack.Add((Window)(object)window);
		}
		((Listing)listing).Gap(12f);
		string toryTalkerMTBDays = Math.Truncate((double)ToolkitSettings.ToryTalkerMTBDays * 100.0 / 100.0).ToString();
		listing.TextFieldNumericLabeled<float>("Average Days Between Events", ref ToolkitSettings.ToryTalkerMTBDays, ref toryTalkerMTBDays, 0.5f, 10f);
		((Listing)listing).End();
	}
}
