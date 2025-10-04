/*
 * TwitchToolkit Mod - Community Preservation Fork
 * 
 * Original Source: TwitchToolkit (GNU Affero GPL v3)
 * Original Copyright: 2019 hodlhodl from original repository
 * 
 * MAJOR MODIFICATIONS � 2025 Captolamia:
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
using Verse;

namespace TwitchToolkit.Votes;

public abstract class Vote
{
	public List<int> optionsKeys = null;

	public Dictionary<int, int> viewerVotes = new Dictionary<int, int>();

	public Dictionary<int, int> voteCounts = new Dictionary<int, int>();

	public Vote(List<int> optionKeys)
	{
		try
		{
			optionsKeys = optionKeys;
			foreach (int key in optionsKeys)
			{
				voteCounts.Add(key, 0);
			}
		}
		catch (InvalidCastException e)
		{
			Log.Error("Invalid new vote. " + e.Message);
		}
	}

	public void RecordVote(int userId, int voteKey)
	{
		if (VoteKeyExists(voteKey))
		{
			if (viewerVotes.ContainsKey(userId))
			{
				viewerVotes[userId] = voteKey;
			}
			else
			{
				viewerVotes.Add(userId, voteKey);
			}
			CountVotes();
		}
	}

	public bool VoteKeyExists(int optionId)
	{
		return optionsKeys.Contains(optionId);
	}

	public void CountVotes()
	{
		foreach (int key in optionsKeys)
		{
			voteCounts[key] = 0;
		}
		foreach (KeyValuePair<int, int> viewerVote in viewerVotes)
		{
			Viewer viewerById = Viewers.GetViewerById(viewerVote.Key);
			int voteCount = 1;
			if (viewerById.IsSub)
			{
				voteCount += ToolkitSettings.SubscriberExtraVotes;
			}
			else if (viewerById.IsVIP)
			{
				voteCount += ToolkitSettings.VIPExtraVotes;
			}
			else if (viewerById.mod)
			{
				voteCount += ToolkitSettings.ModExtraVotes;
			}
			voteCounts[viewerVote.Value] += voteCount;
		}
	}

	public int DecideWinner()
	{
		int highestCount = voteCounts.Aggregate((KeyValuePair<int, int> k, KeyValuePair<int, int> i) => (i.Value > k.Value) ? i : k).Value;
		List<KeyValuePair<int, int>> winners = new List<KeyValuePair<int, int>>();
		foreach (KeyValuePair<int, int> vote in voteCounts)
		{
			if (vote.Value == highestCount)
			{
				winners.Add(vote);
			}
		}
		winners.Shuffle();
		return winners[0].Key;
	}

	public abstract void StartVote();

	public abstract void EndVote();

	public abstract string VoteKeyLabel(int id);
}
