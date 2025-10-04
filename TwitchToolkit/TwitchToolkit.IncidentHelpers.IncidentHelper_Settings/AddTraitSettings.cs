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

public class AddTraitSettings : IncidentHelperVariablesSettings
{
	public static int maxTraits = 4;

	public override void ExposeData()
	{
        ToolkitLogger.Debug($"=== ExposeData AddTraitSettings called! Mode: {Scribe.mode}");
        Scribe_Values.Look<int>(ref maxTraits, "AddTraitSettings.maxTraits", 4, false);
	}

	public override void EditSettings()
	{
		Window_AddTrait window = new Window_AddTrait();
		Find.WindowStack.TryRemove(typeof(Window_AddTrait), true);
		Find.WindowStack.Add((Window)(object)window);
	}
}
