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
using TwitchToolkit.Votes;

namespace TwitchToolkit.Storytellers;

public class VotingIncidentEntry
{
	public VotingIncident incident;

	public float weight;

	public VotingIncidentEntry(VotingIncident incident, float weight)
	{
		this.incident = incident;
		this.weight = weight;
	}
}
