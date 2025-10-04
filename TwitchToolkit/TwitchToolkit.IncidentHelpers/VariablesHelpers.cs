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
using ToolkitCore;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.IncidentHelpers;

public static class VariablesHelpers
{
	public static void ViewerDidWrongSyntax(string username, string syntax, bool separateChannel = false)
	{
		TwitchWrapper.SendChatMessage("@" + username + " syntax is " + syntax);
	}

	public static bool PointsWagerIsValid(string wager, Viewer viewer, ref int pointsWager, ref StoreIncidentVariables incident, bool separateChannel = false, int quantity = 1, int maxPrice = 25000)
	{
		checked
		{
			try
			{
				if (!int.TryParse(wager, out pointsWager))
				{
					ViewerDidWrongSyntax(viewer.username, incident.syntax);
					return false;
				}
				pointsWager *= quantity;
			}
			catch (OverflowException e)
			{
                ToolkitLogger.Error(e.Message);
				TwitchWrapper.SendChatMessage("@" + viewer.username + " points wager is invalid.");
				return false;
			}
			if (incident.maxWager > 0 && incident.maxWager > incident.cost && pointsWager > incident.maxWager)
			{
				TwitchWrapper.SendChatMessage($"@{viewer.username} you cannot spend more than {incident.maxWager} coins on {GenText.CapitalizeFirst(incident.abbreviation)}");
				return false;
			}
			if (pointsWager < incident.cost || pointsWager < incident.minPointsToFire)
			{
				TwitchWrapper.SendChatMessage(Helper.ReplacePlaceholder((TaggedString)(Translator.Translate("TwitchToolkitMinPurchaseNotMet")), null, null, null, null, null, null, null, null, null, null, viewer: viewer.username, amount: pointsWager.ToString(), mod: null, newbalance: null, karma: null, first: incident.cost.ToString()));
				return false;
			}
			if (!Purchase_Handler.CheckIfViewerHasEnoughCoins(viewer, pointsWager))
			{
				return false;
			}
			return true;
		}
	}

	public static void SendPurchaseMessage(string message, bool separateChannel = false)
	{
		if (ToolkitSettings.PurchaseConfirmations)
		{
			TwitchWrapper.SendChatMessage(message);
		}
	}
}
