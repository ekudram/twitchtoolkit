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

using TwitchToolkit.IncidentHelpers.SettingsWindows;
using TwitchToolkit.Incidents;
using Verse;

namespace TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;

public class LevelPawnSettings : IncidentHelperVariablesSettings
{
	public static float xpMultiplier = 1f;

	public override void ExposeData()
	{
		ToolkitLogger.Debug($"=== ExposeData LevelPawnSettings called! Mode: {Scribe.mode}");
        Scribe_Values.Look<float>(ref xpMultiplier, "LevelPawnSettings.xpMultiplier", 1f, false);
	}

	public override void EditSettings()
	{
		Window_LevelPawn window = new Window_LevelPawn();
		Find.WindowStack.TryRemove(typeof(Window_LevelPawn), true);
		Find.WindowStack.Add((Window)(object)window);
	}
}
