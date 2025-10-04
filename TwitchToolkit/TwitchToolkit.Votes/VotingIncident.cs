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
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.Votes;

public class VotingIncident : Def
{
	public int weight;

	public int voteWeight = 100;

	public Storyteller storyteller;

	public EventType eventType;

	public EventCategory eventCategory;

	public Type votingHelper = typeof(IncidentHelper);

	public VotingHelper helper = null;

	public VotingHelper Helper
	{
		get
		{
			if (helper == null)
			{
				Log.Warning("Casting " + base.label);
				helper = VotingIncidentMaker.makeVotingHelper(this);
			}
			return helper;
		}
	}
}
