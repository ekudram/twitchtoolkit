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

public class StoreIncidentVariables : StoreIncident
{
	public int minPointsToFire = 0;

	public int maxWager = 0;

	public string syntax = null;

	public new Type incidentHelper = typeof(IncidentHelperVariables);

	public bool customSettings = false;

	public Type customSettingsHelper = typeof(IncidentHelperVariablesSettings);

	public IncidentHelperVariablesSettings settings = null;

	public void RegisterCustomSettings()
	{
		if (settings == null)
		{
			settings = StoreIncidentMaker.MakeIncidentVariablesSettings(this);
		}
	}
}
