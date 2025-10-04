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

public class ScheduledJob
{
	public int MinutesTillExpire;

	public Func<object, bool> Job;

	public object Product;

	public ScheduledJob(int length, Func<object, bool> job, object product)
	{
		MinutesTillExpire = length;
		Job = job;
		Product = product;
	}

	public void RunJob()
	{
		Job(Product);
	}

	public void Decrement()
	{
		if (MinutesTillExpire > 0)
		{
			MinutesTillExpire--;
		}
	}
}
