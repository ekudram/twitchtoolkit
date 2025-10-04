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
using System.Collections.Generic;
using RimWorld;
using TwitchToolkit.Storytellers;
using Verse;

namespace TwitchToolkit;

public class StorytellerCompProperties_Random : StorytellerCompProperties
{
	public float mtbDays;

	public List<IncidentCategoryEntry> categoryWeights = new List<IncidentCategoryEntry>();

	public float maxThreatBigIntervalDays = 99999f;

	public FloatRange randomPointsFactorRange = new FloatRange(0.5f, 1.5f);

	public bool skipThreatBigIfRaidBeacon;

	public StorytellerCompProperties_Random()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0026: Unknown result type (might be due to invalid IL or missing erences)
		base.compClass = typeof(StorytellerComp_Random);
	}
}
