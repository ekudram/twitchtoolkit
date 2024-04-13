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
		//IL_000d: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0025: Unknown result type (might be due to invalid IL or missing erences)
		//IL_005e: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0097: Unknown result type (might be due to invalid IL or missing erences)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing erences)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing erences)
		optionsListing.Label("All", -1f, null);
		optionsListing.GapLine(12f);
		optionsListing.SliderLabeled("TwitchToolkitVoteTime".Translate(), ref ToolkitSettings.VoteTime, Math.Round((double)ToolkitSettings.VoteTime).ToString(), 1f, 15f);
		optionsListing.SliderLabeled("TwitchToolkitVoteOptions".Translate(), ref ToolkitSettings.VoteOptions, Math.Round((double)ToolkitSettings.VoteOptions).ToString(), 2f, 5f);
		optionsListing.CheckboxLabeled("TwitchToolkitVotingChatMsgs".Translate(), ref ToolkitSettings.VotingChatMsgs, null);
		optionsListing.CheckboxLabeled("TwitchToolkitVotingWindow".Translate(), ref ToolkitSettings.VotingWindow, null);
		optionsListing.CheckboxLabeled("TwitchToolkitLargeVotingWindow".Translate(), ref ToolkitSettings.LargeVotingWindow, null);
		optionsListing.Gap(12f);
		if (optionsListing.ButtonTextLabeled("Edit Storyteller Packs", "Storyteller Packs"))
		{
			Window_StorytellerPacks window = new Window_StorytellerPacks();
			Find.WindowStack.TryRemove(window.GetType(), true);
			Find.WindowStack.Add(window);
		}
	}
}
