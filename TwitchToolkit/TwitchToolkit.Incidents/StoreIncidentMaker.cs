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
using TwitchToolkit.Store;

namespace TwitchToolkit.Incidents;

public static class StoreIncidentMaker
{
	public static IncidentHelper MakeIncident(StoreIncidentSimple def)
	{
		IncidentHelper helper = (IncidentHelper)Activator.CreateInstance(def.incidentHelper);
		helper.storeIncident = def;
		return helper;
	}

	public static IncidentHelperVariables MakeIncidentVariables(StoreIncidentVariables def)
	{
		IncidentHelperVariables helper = (IncidentHelperVariables)Activator.CreateInstance(def.incidentHelper);
		helper.storeIncident = def;
		return helper;
	}

	public static IncidentHelperVariablesSettings MakeIncidentVariablesSettings(StoreIncidentVariables def)
	{
		if (!def.customSettings)
		{
			return null;
		}
		return (IncidentHelperVariablesSettings)Activator.CreateInstance(def.customSettingsHelper);
	}
}
