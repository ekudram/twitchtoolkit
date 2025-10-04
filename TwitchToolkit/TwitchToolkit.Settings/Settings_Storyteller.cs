/*
 * File: Toolkitsettings.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 14, 2025
 * 
 * Summary of Changes:
 * 1. Added storytelleer button option
 * 2. Implemented DoWindowContents method to use passed Listing_Standard
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
using System.Collections.Generic;
using TwitchToolkit.Storytellers.StorytellerPackWindows;
using TwitchToolkit.Votes;
using UnityEngine;
using Verse;
using EventType = TwitchToolkit.Votes.EventType;

namespace TwitchToolkit.Settings;

[StaticConstructorOnStartup]
public static class Settings_Storyteller
{
	static Settings_Storyteller()
	{
		if (ToolkitSettings.VoteTypeWeights.Count < 1)
		{
			NewVoteTypeWeightsHodlBot();
		}
		if (ToolkitSettings.VoteCategoryWeights.Count < 1)
		{
			NewVoteCategoryWeightsHodlBot();
		}
	}

	public static void NewVoteCategoryWeightsHodlBot()
	{
		ToolkitSettings.VoteCategoryWeights = new Dictionary<string, float>
		{
			{
				EventCategory.Animal.ToString(),
				100f
			},
			{
				EventCategory.Colonist.ToString(),
				100f
			},
			{
				EventCategory.Disease.ToString(),
				100f
			},
			{
				EventCategory.Drop.ToString(),
				100f
			},
			{
				EventCategory.Enviroment.ToString(),
				100f
			},
			{
				EventCategory.Foreigner.ToString(),
				100f
			},
			{
				EventCategory.Hazard.ToString(),
				100f
			},
			{
				EventCategory.Invasion.ToString(),
				150f
			},
			{
				EventCategory.Mind.ToString(),
				100f
			},
			{
				EventCategory.Weather.ToString(),
				100f
			}
		};
	}

	public static void NewVoteTypeWeightsHodlBot()
	{
		ToolkitSettings.VoteTypeWeights = new Dictionary<string, float>
		{
			{
				EventType.Bad.ToString(),
				125f
			},
			{
				EventType.Good.ToString(),
				100f
			},
			{
				EventType.Neutral.ToString(),
				100f
			}
		};
	}

    public static void DoWindowContents(Rect rect, Listing_Standard optionsListing)
    {

        optionsListing.Label("Storyteller");
        optionsListing.Gap(12f);
        optionsListing.Label("All");
        optionsListing.GapLine(12f);
        optionsListing.SliderLabeled("TwitchToolkitVoteTime".Translate(), ref ToolkitSettings.VoteTime, Math.Round((double)ToolkitSettings.VoteTime).ToString(), 1f, 15f);
        optionsListing.SliderLabeled("TwitchToolkitVoteOptions".Translate(), ref ToolkitSettings.VoteOptions, Math.Round((double)ToolkitSettings.VoteOptions).ToString(), 2f, 5f);
        optionsListing.CheckboxLabeled("TwitchToolkitVotingChatMsgs".Translate(), ref ToolkitSettings.VotingChatMsgs, null);
        optionsListing.CheckboxLabeled("TwitchToolkitVotingWindow".Translate(), ref ToolkitSettings.VotingWindow, null);
        optionsListing.CheckboxLabeled("TwitchToolkitLargeVotingWindow".Translate(), ref ToolkitSettings.LargeVotingWindow, null);
        optionsListing.Gap(12f);

        // Added: Always show storyteller button option
        optionsListing.CheckboxLabeled("Always Show Storyteller Button", ref ToolkitSettings.AlwaysShowStorytellerButton);

        if (optionsListing.ButtonTextLabeled("Edit Storyteller Packs", "TTK Storyteller Packs", TextAnchor.UpperLeft, null, "TTK Polls"))
        {
            Window_StorytellerPacks window = new Window_StorytellerPacks();
            Find.WindowStack.TryRemove(window.GetType(), true);
            Find.WindowStack.Add(window);
        }
    }
}
