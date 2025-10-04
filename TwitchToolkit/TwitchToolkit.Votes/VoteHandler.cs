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
using TwitchToolkit.Utilities;

namespace TwitchToolkit.Votes;

public static class VoteHandler
{
	public static bool voteActive = false;

	public static DateTime voteStartedAt;

	private static List<Vote> voteQueue = new List<Vote>();

	public static Vote currentVote = null;

	private static bool forceEnd = false;

	public static void QueueVote(Vote vote)
	{
		voteQueue.Add(vote);
	}

	public static void ForceEnd()
	{
		forceEnd = true;
	}

	public static void CheckForQueuedVotes()
	{
		if (!voteActive && voteQueue.Count > 0 && currentVote == null)
		{
			voteActive = true;
			voteStartedAt = DateTime.Now;
			currentVote = voteQueue[0];
			voteQueue.Remove(currentVote);
			currentVote.StartVote();
		}
		if (voteActive && (TimeHelper.MinutesElapsed(voteStartedAt) >= ToolkitSettings.VoteTime || forceEnd))
		{
			forceEnd = false;
			currentVote.EndVote();
			currentVote = null;
			voteActive = false;
		}
	}
}
