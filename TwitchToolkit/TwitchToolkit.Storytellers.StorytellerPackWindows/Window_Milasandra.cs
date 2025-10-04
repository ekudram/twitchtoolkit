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
using UnityEngine;
using Verse;

namespace TwitchToolkit.Storytellers.StorytellerPackWindows;

public class Window_Milasandra : Window
{
	public override Vector2 InitialSize => new Vector2(500f, 700f);

	public Window_Milasandra()
	{
		base.doCloseButton = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		Listing_Standard listing = new Listing_Standard();
		((Listing)listing).Begin(inRect);
		Text.Font =((GameFont)2);
		listing.Label("<color=#1482CB>Milasandra</color> Settings");
		Text.Font =((GameFont)1);
		((Listing)listing).Gap(12f);
		((Listing)listing).GapLine(12f);
		listing.Label("Milasandra uses an on and off cycle to bring votes in waves, similar to Cassandra.");
		((Listing)listing).Gap(12f);
		listing.Label("This pack is the closest to the base games basic forced raid cycle. You will experience lag generating these votes.");
		((Listing)listing).Gap(12f);
		listing.Label("There are no settings to change because Milasandra will generate votes on the same timeline as Cassandra.");
		((Listing)listing).End();
	}
}
