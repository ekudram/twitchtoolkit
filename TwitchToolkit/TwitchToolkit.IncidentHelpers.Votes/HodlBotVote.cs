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
using System.Collections.Generic;
using RimWorld;
using TwitchToolkit.Store;
using TwitchToolkit.Storytellers;
using Verse;

namespace TwitchToolkit.IncidentHelpers.Votes;

public class HodlBotVote : IncidentHelperVariables
{
	private StorytellerPack hodlBot;

	public override Viewer Viewer { get; set; }

	public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
	{
		Viewer = viewer;
		hodlBot = DefDatabase<StorytellerPack>.GetNamed("HodlBot", true);
		if (hodlBot != null && hodlBot.enabled)
		{
			return true;
		}
		return false;
	}

	public override void TryExecute()
	{
		Map map = Helper.AnyPlayerMap;
		StorytellerComp_HodlBot storytellerComp = new StorytellerComp_HodlBot();
		storytellerComp.forced = true;
		using (IEnumerator<FiringIncident> enumerator = ((StorytellerComp)storytellerComp).MakeIntervalIncidents((IIncidentTarget)(object)map).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				FiringIncident incident = enumerator.Current;
				Ticker.FiringIncidents.Enqueue(incident);
			}
		}
		Viewer.TakeViewerCoins(storeIncident.cost);
		Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);
		VariablesHelpers.SendPurchaseMessage("@" + Viewer.username + " purchased a HodlBot Vote.");
	}
}
