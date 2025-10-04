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
using ToolkitCore;
using TwitchToolkit.Storytellers;
using Verse;

namespace TwitchToolkit.Votes;

public class Vote_HodlBot : Vote_VotingIncident
{
	private static readonly List<EventCategory> categoriesThatCanBeMystery = new List<EventCategory>
	{
		EventCategory.Animal,
		EventCategory.Colonist,
		EventCategory.Drop,
		EventCategory.Enviroment,
		EventCategory.Foreigner
	};

	private readonly StorytellerPack pack = null;

	public Vote_HodlBot(Dictionary<int, VotingIncident> incidents, StorytellerPack pack, VoteLabelType labelType, string title = null)
		: base(incidents, title)
	{
		PickVoteLabelType(labelType);
		this.pack = pack;
		base.title = title;
	}

	public override void StartVote()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0071: Unknown result type (might be due to invalid IL or missing erences)
		//IL_007b: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0080: Unknown result type (might be due to invalid IL or missing erences)
		if (ToolkitSettings.VotingWindow || (!ToolkitSettings.VotingWindow && !ToolkitSettings.VotingChatMsgs))
		{
			VoteWindow window = new VoteWindow(this, "<color=#4BB543>" + title + "</color>");
			Find.WindowStack.Add((Window)(object)window);
		}
		if (!ToolkitSettings.VotingChatMsgs)
		{
			return;
		}
		TwitchWrapper.SendChatMessage(title ?? (TaggedString)(Translator.Translate("TwitchStoriesChatMessageNewVote") + ": " + Translator.Translate("TwitchToolKitVoteInstructions")));
		foreach (KeyValuePair<int, VotingIncident> pair in incidents)
		{
			TwitchWrapper.SendChatMessage($"[{pair.Key + 1}]  {VoteKeyLabel(pair.Key)}");
		}
	}

	public override void EndVote()
	{
		Current.Game.GetComponent<StoryTellerVoteTracker>().LogStorytellerCompVote(pack);
		base.EndVote();
	}

	private void PickVoteLabelType(VoteLabelType labelType)
	{
		if (labelType == VoteLabelType.Category)
		{
			if (GenCollection.Any<EventCategory>(categoriesThatCanBeMystery, (Predicate<EventCategory>)((EventCategory s) => s == incidents.ElementAt(0).Value.eventCategory)) && Rand.Bool)
			{
				base.labelType = VoteLabelType.Type;
			}
			else
			{
				base.labelType = VoteLabelType.Label;
			}
		}
	}
}
