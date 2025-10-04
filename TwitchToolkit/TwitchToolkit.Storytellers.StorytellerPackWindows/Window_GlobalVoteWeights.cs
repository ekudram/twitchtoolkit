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
using System.Collections.Generic;
using System.Linq;
using TwitchToolkit.Votes;
using UnityEngine;
using Verse;

namespace TwitchToolkit.Storytellers.StorytellerPackWindows;

public class Window_GlobalVoteWeights : Window
{
	// private Vector2 scrollPosition;

	private int totalWeights = 1;

	public override Vector2 InitialSize => new Vector2(450f, 560f);

	public Window_GlobalVoteWeights()
	{
		base.doCloseButton = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		Listing_Standard listing = new Listing_Standard();
		List<VotingIncident> allVotes = DefDatabase<VotingIncident>.AllDefs.ToList();
		Rect outRect = new Rect(0f, 0f, ((Rect)(inRect)).width, ((Rect)(inRect)).height - 50f);
		Rect viewRect = new Rect(0f, 0f, ((Rect)(outRect)).width - 20f, (float)allVotes.Count * 31f);
		((Listing)listing).Begin(inRect);
		listing.BeginSection(450f, 4f, 4f);
		listing.Label("Change the weights given to votes. Setting to 0% disables it.");
		((Listing)listing).Gap(12f);
		if (ToolkitSettings.VoteWeights != null)
		{
			int newWeights = 0;
			foreach (VotingIncident vote in allVotes)
			{
				int index = ((Def)vote).index;
				float percentage = (float)Math.Round((float)vote.voteWeight / (float)totalWeights * 100f, 2);
				listing.SliderLabeled(((Def)vote).defName + " - " + percentage + "%", ref vote.voteWeight, vote.voteWeight.ToString());
				ToolkitSettings.VoteWeights[((Def)vote).defName] = vote.voteWeight;
				newWeights += vote.voteWeight;
				((Listing)listing).Gap(6f);
			}
			totalWeights = newWeights;
		}
		listing.EndSection(listing);
		((Listing)listing).End();
	}
}
