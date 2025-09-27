/*
 * Program: TwitchToolkit
 * File: PawnCommands.cs
 * 
 * Usage: Handles pawn-related commands from Twitch chat, including skill and story inquiries, and name change requests.
 * 
 * Changes:
 *  Now uses TwitchMessageWrapper instead of ChatMessage for better integration.
 *  
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using ToolkitCore;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.PawnQueue;

public class PawnCommands : TwitchInterfaceBase
{
	public Dictionary<string, string> nameRequests = new Dictionary<string, string>();

	public PawnCommands(Game game)
	{
	}

	public override void ParseMessage(TwitchMessageWrapper messageWrapper)
	{
		Viewer viewer = Viewers.GetViewer(messageWrapper.Username);
		GameComponentPawns component = Current.Game.GetComponent<GameComponentPawns>();
		if (messageWrapper.Message.StartsWith("!mypawnskills"))
		{
			if (!component.HasUserBeenNamed(viewer.username))
			{
				TwitchWrapper.SendChatMessage("@" + viewer.username + " you are not in the colony.");
				return;
			}
			Pawn pawn2 = component.PawnAssignedToUser(viewer.username);
			string output = "@" + viewer.username + " " + GenText.CapitalizeFirst(pawn2.Name.ToStringShort) + "'s skill levels are ";
			List<SkillRecord> skills = pawn2.skills.skills;
			for (int j = 0; j < skills.Count; j++)
			{
				output = ((!skills[j].TotallyDisabled) ? (output + $"{((Def)skills[j].def).LabelCap}: {skills[j].levelInt}") : (output + $"{((Def)skills[j].def).LabelCap}: -"));
				if ((int)skills[j].passion == 1)
				{
					output += "+";
				}
				if ((int)skills[j].passion == 2)
				{
					output += "++";
				}
				if (j != skills.Count - 1)
				{
					output += ", ";
				}
			}
			TwitchWrapper.SendChatMessage(output);
		}
		if (messageWrapper.Message.StartsWith("!mypawnstory"))
		{
			if (!component.HasUserBeenNamed(viewer.username))
			{
				TwitchWrapper.SendChatMessage("@" + viewer.username + " you are not in the colony.");
				return;
			}
			Pawn pawn3 = component.PawnAssignedToUser(viewer.username);
			string output2 = "@" + viewer.username + " About " + GenText.CapitalizeFirst(pawn3.Name.ToStringShort) + ": ";
			List<BackstoryDef> backstories = pawn3.story.AllBackstories.ToList();
			for (int k = 0; k < backstories.Count; k++)
			{
				output2 += backstories[k].title;
				if (k != backstories.Count - 1)
				{
					output2 += ", ";
				}
			}
			output2 = output2 + " | " + pawn3.gender;
			StringBuilder stringBuilder = new StringBuilder();
			WorkTags combinedDisabledWorkTags = pawn3.story.DisabledWorkTagsBackstoryAndTraits;
			if ((int)combinedDisabledWorkTags == 0)
			{
				stringBuilder.Append((TaggedString)("(" + Translator.Translate("NoneLower") + "), "));
			}
			else
			{
				List<WorkTags> list = WorkTagsFrom(combinedDisabledWorkTags).ToList();
				bool flag2 = true;
				foreach (WorkTags tags in list)
				{
					if (flag2)
					{
						stringBuilder.Append(GenText.CapitalizeFirst(WorkTypeDefsUtility.LabelTranslated(tags)));
					}
					else
					{
						stringBuilder.Append(WorkTypeDefsUtility.LabelTranslated(tags));
					}
					stringBuilder.Append(", ");
					flag2 = false;
				}
			}
			string text = stringBuilder.ToString();
			text = text.Substring(0, text.Length - 2);
			output2 = output2 + " | Incapable of: " + text;
			output2 += " | Traits: ";
			List<Trait> traits = pawn3.story.traits.allTraits;
			for (int i = 0; i < traits.Count; i++)
			{
				output2 += traits[i].LabelCap;
				if (i != traits.Count - 1)
				{
					output2 += ", ";
				}
			}
			TwitchWrapper.SendChatMessage(output2);
		}
		if (messageWrapper.Message.StartsWith("!changepawnname"))
		{
			string[] command3 = messageWrapper.Message.Split(' ');
			if (command3.Length < 2)
			{
				return;
			}
			string newName = command3[1];
			if (newName == null || newName == "" || newName.Length > 16)
			{
				TwitchWrapper.SendChatMessage("@" + viewer.username + " your name can be up to 16 characters.");
				return;
			}
			if (!component.HasUserBeenNamed(viewer.username))
			{
				TwitchWrapper.SendChatMessage("@" + viewer.username + " you are not in the colony.");
				return;
			}
			if (!Purchase_Handler.CheckIfViewerHasEnoughCoins(viewer, 500, separateChannel: true))
			{
				return;
			}
			viewer.TakeViewerCoins(500);
			nameRequests.Add(viewer.username, newName);
			TwitchWrapper.SendChatMessage("@" + ToolkitSettings.Channel + " " + viewer.username + " has requested to be named " + newName + ", use !approvename @" + viewer.username + " or !declinename @" + viewer.username);
		}
		if (Viewer.IsModerator(viewer.username) || viewer.username == ToolkitSettings.Channel)
		{
			if (messageWrapper.Message.StartsWith("!unstickpeople"))
			{
				Purchase_Handler.viewerNamesDoingVariableCommands = new List<string>();
			}
			if (messageWrapper.Message.StartsWith("!approvename"))
			{
				string[] command2 = messageWrapper.Message.Split(' ');
				if (command2.Length < 2)
				{
					return;
				}
				string username2 = command2[1].Replace("@", "");
				if (username2 == null || username2 == "" || !nameRequests.ContainsKey(username2))
				{
					TwitchWrapper.SendChatMessage("@" + viewer.username + " invalid username");
					return;
				}
				if (!component.HasUserBeenNamed(username2))
				{
					return;
				}
				Pawn pawn = component.PawnAssignedToUser(username2);
				Name name = pawn.Name;
				NameTriple old = (NameTriple)(object)((name is NameTriple) ? name : null);
				pawn.Name = ((Name)new NameTriple(old.First, nameRequests[username2], old.Last));
				TwitchWrapper.SendChatMessage($"@{viewer.username} approved request for name change from {old} to {pawn.Name}");
			}
			if (messageWrapper.Message.StartsWith("!declinename"))
			{
				string[] command = messageWrapper.Message.Split(' ');
				if (command.Length < 2)
				{
					return;
				}
				string username = command[1].Replace("@", "");
				if (username == null || username == "" || !nameRequests.ContainsKey(username))
				{
					TwitchWrapper.SendChatMessage("@" + viewer.username + " invalid username");
					return;
				}
				if (!component.HasUserBeenNamed(username))
				{
					return;
				}
				nameRequests.Remove(username);
				TwitchWrapper.SendChatMessage("@" + viewer.username + " declined name change request from " + username);
			}
		}
		Store_Logger.LogString("Parsed pawn command");
	}

	private static IEnumerable<WorkTags> WorkTagsFrom(WorkTags tags)
	{
		foreach (WorkTags workTag in Gen.GetAllSelectedItems<WorkTags>((Enum)(object)tags))
		{
			if ((int)workTag > 0)
			{
				yield return workTag;
			}
		}
	}
}
