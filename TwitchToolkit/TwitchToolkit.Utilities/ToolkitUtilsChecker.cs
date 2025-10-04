/*
 * TwitchToolkit Mod - Community Preservation Fork
 * 
 * Original Source: TwitchToolkit (GNU Affero GPL v3)
 * Original Copyright: 2019 hodlhodl from original repository
 * 
 * MAJOR MODIFICATIONS � 2025 Captolamia:
 * Modifications listed above if any.
 * 
 * This work is licensed under GNU Affero GPL v3
 * This is a community preservation effort to maintain and improve
 * abandoned mod code for the benefit of all users.
 * 
 * See LICENSE file for full terms.
 */
using Verse;

namespace TwitchToolkit.Utilities;

[StaticConstructorOnStartup]
public static class ToolkitUtilsChecker
{
	public static bool ToolkitUtilsInstalled;

	public static bool ToolkitUtilsActive;

	static ToolkitUtilsChecker()
	{
		ToolkitUtilsInstalled = false;
		ToolkitUtilsActive = false;
		foreach (ModMetaData modMetaData in ModLister.AllInstalledMods)
		{
			if (modMetaData.Name == "ToolkitUtils")
			{
				ToolkitUtilsInstalled = true;
				Log.Message("ToolkitUtils Installed");
				if (modMetaData.Active)
				{
					ToolkitUtilsActive = true;
					Log.Message("ToolkitUtils Active");
				}
				break;
			}
		}
	}
}
