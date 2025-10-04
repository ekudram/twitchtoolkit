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

namespace TwitchToolkit.Utilities;

public class TimeHelper
{
	public static int SecondsElapsed(DateTime startTime)
	{
		TimeSpan span = DateTime.Now - startTime;
		return span.Seconds + (span.Hours * 60 + span.Minutes) * 60;
	}

	public static int MinutesElapsed(DateTime startTime)
	{
		TimeSpan span = DateTime.Now - startTime;
		return span.Hours * 60 + span.Minutes;
	}
}
