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
using RimWorld;
using ToolkitCore;
using Verse;

namespace TwitchToolkit.Votes;

public class VoteIncidentDef : Vote
{
	public Dictionary<int, IncidentDef> incidents = null;

	public StorytellerComp source = null;

	public IncidentParms parms = null;

	public string title = null;

	public VoteIncidentDef(Dictionary<int, IncidentDef> incidents, StorytellerComp source, IncidentParms parms = null, string title = null)
		: base(new List<int>(incidents.Keys))
	{
		this.parms = parms;
		this.title = title;
		try
		{
			this.incidents = incidents;
			this.source = source;
		}
		catch (InvalidCastException e)
		{
			Log.Error("Invalid VoteIncidentDef. " + e.Message);
		}
	}

	public override void StartVote()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing erences)
		//IL_005d: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0067: Unknown result type (might be due to invalid IL or missing erences)
		//IL_006c: Unknown result type (might be due to invalid IL or missing erences)
		if (ToolkitSettings.VotingWindow || (!ToolkitSettings.VotingWindow && !ToolkitSettings.VotingChatMsgs))
		{
			VoteWindow window = new VoteWindow(this);
			Find.WindowStack.Add((Window)(object)window);
		}
		if (!ToolkitSettings.VotingChatMsgs)
		{
			return;
		}
		TwitchWrapper.SendChatMessage(title ?? (TaggedString)(Translator.Translate("TwitchStoriesChatMessageNewVote") + ": " + Translator.Translate("TwitchToolKitVoteInstructions")));
		foreach (KeyValuePair<int, IncidentDef> pair in incidents)
		{
			TwitchWrapper.SendChatMessage($"[{pair.Key + 1}]  {VoteKeyLabel(pair.Key)}");
		}
	}

	public override void EndVote()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing erences)
		//IL_002d: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0069: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0078: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0083: Expected O, but got Unknown
		Ticker.FiringIncidents.Enqueue(new FiringIncident(incidents[DecideWinner()], source, parms));
		Ticker.lastEvent = DateTime.Now;
		Find.WindowStack.TryRemove(typeof(VoteWindow), true);
		Messages.Message(new Message((TaggedString)("Chat voted for: " + ((Def)incidents[DecideWinner()]).LabelCap), MessageTypeDefOf.NeutralEvent), true);
	}

	public override string VoteKeyLabel(int id)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing erences)
		return (TaggedString)(((Def)incidents[id]).LabelCap);
	}
}
