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
using System.IO;
using TwitchToolkit.Utilities;

namespace TwitchToolkit.Store;

public static class Store_Logger
{
	public static string DataPath = Path.Combine(SaveHelper.dataPath, "Logs");

	public static string LogFile = Path.Combine(DataPath, DateTime.Now.Month + "_" + DateTime.Now.Day + "_log.txt");

	public static void LogString(string line)
	{
		if (!Directory.Exists(DataPath))
		{
			Directory.CreateDirectory(DataPath);
		}
		if (!File.Exists(LogFile))
		{
			try
			{
				using StreamWriter streamWriter = File.CreateText(LogFile);
				streamWriter.WriteLine("TwitchToolkit - Log - " + DateTime.Now.ToLongDateString());
			}
			catch (Exception e2)
			{
                ToolkitLogger.Error(e2.Message);
			}
		}
		try
		{
			using StreamWriter writer = File.AppendText(LogFile);
			writer.WriteLine(line);
		}
		catch (Exception e)
		{
            ToolkitLogger.Error(e.Message);
		}
	}

	public static void LogPurchase(string username, string command)
	{
		LogString("Purchase " + username + ": " + command + " @ " + DateTime.Now.ToShortTimeString());
	}

	public static void LogKarmaChange(string username, int oldKarma, int newKarma)
	{
		LogString($"{username}'s karma went from {oldKarma} to {newKarma}");
	}

	public static void LogGiveCoins(string username, string giftee, int amount)
	{
		LogString($"{username} gave viewer {giftee} {amount} coins @ {DateTime.Now.ToShortTimeString()}");
	}

	public static void LogGiftCoins(string username, string giftee, int amount)
	{
		LogString($"{username} gifted viewer {giftee} {amount} coins @ {DateTime.Now.ToShortTimeString()}");
	}
}
