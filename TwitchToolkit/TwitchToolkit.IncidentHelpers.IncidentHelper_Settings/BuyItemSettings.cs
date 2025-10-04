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

public class BuyItemSettings : IncidentHelperVariablesSettings
{
	public static bool mustResearchFirst = true;

	public override void ExposeData()
	{
        ToolkitLogger.Debug($"=== ExposeData AddTraitSettings called! Mode: {Scribe.mode}");
        Scribe_Values.Look<bool>(ref mustResearchFirst, "BuyItemSettings.mustResearchFirst", true, false);
	}

	public override void EditSettings()
	{
		Window_BuyItem window = new Window_BuyItem();
		Find.WindowStack.TryRemove(typeof(Window_BuyItem), true);
		Find.WindowStack.Add((Window)(object)window);
	}
}
