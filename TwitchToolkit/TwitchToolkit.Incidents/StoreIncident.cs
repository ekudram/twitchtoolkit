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
using Verse;

namespace TwitchToolkit.Incidents;

public class StoreIncident : Def
{
	public string abbreviation;

	public int cost;

	public int eventCap;

	public Type incidentHelper = typeof(IncidentHelper);

	public KarmaType karmaType;

	public int variables = 0;
}
