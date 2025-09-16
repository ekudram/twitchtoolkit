/*
 * File: Settings_Karma.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 15, 2025
 * 
 * Summary of Changes:
 * 1. Fixed minimum karma setting layout to display label on left and input field on right
 * 2. Replaced TextFieldNumericLabeled with manual Widgets layout for better UI alignment
 */

using System;
using UnityEngine;
using Verse;

namespace TwitchToolkit.Settings;

public static class Settings_Karma
{
	public static void DoWindowContents(Rect rect, Listing_Standard optionsListing)
	{
		optionsListing.SliderLabeled("TwitchToolkitStartingKarma".Translate(),  ref ToolkitSettings.StartingKarma, Math.Round((double)ToolkitSettings.StartingKarma).ToString(), 50f, 250f);
		optionsListing.SliderLabeled("TwitchToolkitKarmaCap".Translate(),  ref ToolkitSettings.KarmaCap, Math.Round((double)ToolkitSettings.KarmaCap).ToString(), 150f, 600f);
		optionsListing.CheckboxLabeled("TwitchToolkitBanViewersWhoAreBad".Translate(), ref ToolkitSettings.BanViewersWhoPurchaseAlwaysBad, null);

        var lineRect = optionsListing.GetRect(Text.LineHeight);
        Widgets.Label(lineRect.LeftPart(0.7f), "What is the minimum amount of karma viewers can reach?");
        string minKarmaBuffer = ToolkitSettings.KarmaMinimum.ToString();
        Widgets.TextFieldNumeric<int>(lineRect.RightPart(0.3f), ref ToolkitSettings.KarmaMinimum, ref minKarmaBuffer, -100f, 100f);

        optionsListing.Gap(12f); optionsListing.Gap(12f);
		optionsListing.CheckboxLabeled("TwitchToolkitKarmaReqsForGifting".Translate(), ref ToolkitSettings.KarmaReqsForGifting, null);
		optionsListing.Gap(12f);
		optionsListing.SliderLabeled("TwitchToolkitMinKarmaForGifts".Translate(), ref  ToolkitSettings.MinimumKarmaToRecieveGifts, Math.Round((double)ToolkitSettings.MinimumKarmaToRecieveGifts).ToString(), 10f);
		optionsListing.SliderLabeled("TwitchToolkitMinKarmaSendGifts".Translate(), ref  ToolkitSettings.MinimumKarmaToSendGifts, Math.Round((double)ToolkitSettings.MinimumKarmaToSendGifts).ToString(), 20f, 150f);
		optionsListing.Gap(12f);
		optionsListing.GapLine(12f);
		optionsListing.Label("TwitchToolkitGoodViewers".Translate(), -1f, null);
		optionsListing.SliderLabeled("TwitchToolkitGoodKarma".Translate(),  ref ToolkitSettings.TierOneGoodBonus, Math.Round((double)ToolkitSettings.TierOneGoodBonus).ToString(), 1f);
		optionsListing.SliderLabeled("TwitchToolkitNeutralKarma".Translate(), ref  ToolkitSettings.TierOneNeutralBonus, Math.Round((double)ToolkitSettings.TierOneNeutralBonus).ToString(), 1f);
		optionsListing.SliderLabeled("TwitchToolkitBadKarma".Translate(),  ref ToolkitSettings.TierOneBadBonus, Math.Round((double)ToolkitSettings.TierOneBadBonus).ToString(), 1f);
		optionsListing.Gap(12f);
		optionsListing.GapLine(12f);
		optionsListing.Label("TwitchToolkitNeutralViewers".Translate(), -1f, null);
		optionsListing.SliderLabeled("TwitchToolkitGoodKarma".Translate(),  ref ToolkitSettings.TierTwoGoodBonus, Math.Round((double)ToolkitSettings.TierTwoGoodBonus).ToString(), 1f);
		optionsListing.SliderLabeled("TwitchToolkitNeutralKarma".Translate(),  ref ToolkitSettings.TierTwoNeutralBonus, Math.Round((double)ToolkitSettings.TierTwoNeutralBonus).ToString(), 1f);
		optionsListing.SliderLabeled("TwitchToolkitBadKarma".Translate(),  ref ToolkitSettings.TierTwoBadBonus, Math.Round((double)ToolkitSettings.TierTwoBadBonus).ToString(), 1f);
		optionsListing.Gap(12f);
		optionsListing.GapLine(12f);
		optionsListing.Label("TwitchToolkitBadViewers".Translate(), -1f, null);
		optionsListing.SliderLabeled("TwitchToolkitGoodKarma".Translate(),  ref ToolkitSettings.TierThreeGoodBonus, Math.Round((double)ToolkitSettings.TierThreeGoodBonus).ToString(), 1f);
		optionsListing.SliderLabeled("TwitchToolkitNeutralKarma".Translate(),  ref ToolkitSettings.TierThreeNeutralBonus, Math.Round((double)ToolkitSettings.TierThreeNeutralBonus).ToString(), 1f);
		optionsListing.SliderLabeled("TwitchToolkitBadKarma".Translate(),  ref ToolkitSettings.TierThreeBadBonus, Math.Round((double)ToolkitSettings.TierThreeBadBonus).ToString(), 1f);
		optionsListing.Gap(12f);
		optionsListing.GapLine(12f);
		optionsListing.Label("TwitchToolkitDoomViewers".Translate(), -1f, null);
		optionsListing.SliderLabeled("TwitchToolkitGoodKarma".Translate(),  ref ToolkitSettings.TierFourGoodBonus, Math.Round((double)ToolkitSettings.TierFourGoodBonus).ToString(), 1f);
		optionsListing.SliderLabeled("TwitchToolkitNeutralKarma".Translate(),  ref ToolkitSettings.TierFourNeutralBonus, Math.Round((double)ToolkitSettings.TierFourNeutralBonus).ToString(), 1f);
		optionsListing.SliderLabeled("TwitchToolkitBadKarma".Translate(),  ref ToolkitSettings.TierFourBadBonus, Math.Round((double)ToolkitSettings.TierFourBadBonus).ToString(), 1f);
	}
}
