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
using TwitchToolkit.Store;

namespace TwitchToolkit.IncidentHelpers.Misc;

public class VomitRain : IncidentHelper
{
	public override bool IsPossible()
	{
		return true;
	}

	public override void TryExecute()
	{
		int count = 50;
		for (int i = 0; i < count; i++)
		{
			Helper.Vomit(Helper.AnyPlayerMap);
		}
	}
}
