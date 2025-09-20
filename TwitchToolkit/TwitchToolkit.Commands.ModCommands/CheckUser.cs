/*
 * File: CheckUser.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 14, 2025
 * 
 * Summary of Changes:

 */
using System;
using ToolkitCore;
using TwitchLib.Client.Models;
using Verse;

namespace TwitchToolkit.Commands.ModCommands;

public class CheckUser : CommandDriver
{
	public override void RunCommand(ITwitchMessage twitchMessage)
	{
		try
		{
			string[] command = twitchMessage.Message.Split(' ');
			if (command.Length >= 2)
			{
				string target = command[1].Replace("@", "");
				Viewer targeted = Viewers.GetViewer(target);
				TwitchWrapper.SendChatMessage("@" + twitchMessage.Username + " " + Helper.ReplacePlaceholder((TaggedString)(Translator.Translate("TwitchToolkitCheckUser")), null, null, null, null, null, null, null, null, null, null, viewer: targeted.username, amount: targeted.coins.ToString(), mod: null, newbalance: null, karma: targeted.GetViewerKarma().ToString()));
			}
		}
		catch (InvalidCastException e)
		{
			ToolkitLogger.Log("Invalid Check User Command " + e.Message);
		}
	}
}
