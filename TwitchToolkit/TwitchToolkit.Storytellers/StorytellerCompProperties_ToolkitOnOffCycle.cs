/*
 * File: StorytellerCompProperties_ToolkitOnOffCycle.cs
 * 
 * Project: TwitchToolkit
 * 
 * Updated: September 16, 2025
 */
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
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TwitchToolkit.Storytellers;

public class StorytellerCompProperties_ToolkitOnOffCycle : StorytellerCompProperties
{
	public float onDays = 4.6f;
	public float offDays = 6f;
	public float minSpacingDays = 1.9f;
	public new float minDaysPassed = 11f;
	public FloatRange numIncidentsRange = new FloatRange(1f, 2f);
	public SimpleCurve acceptFractionByDaysPassedCurve;
	public SimpleCurve acceptPercentFactorPerThreatPointsCurve;
	public IncidentDef incident;
	private IncidentCategoryDef category = IncidentCategoryDefOf.ThreatBig;
	public bool applyRaidBeaconThreatMtbFactor;
	public float forceRaidEnemyBeforeDaysPassed = 20f;

	public IncidentCategoryDef IncidentCategory
	{
		get
		{
			if (incident != null)
			{
				return incident.category;
			}
			return category;
		}
	}

	public StorytellerCompProperties_ToolkitOnOffCycle()
	{
		base.compClass = typeof(StorytellerComp_ToolkitOnOffCycle);
	}

	public override IEnumerable<string> ConfigErrors(StorytellerDef parentDef)
	{
		if (incident != null && category != null)
		{
			yield return "incident and category should not both be defined";
		}
		if (onDays <= 0f)
		{
			yield return "onDays must be above zero";
		}
		if (((FloatRange)( numIncidentsRange)).TrueMax <= 0f)
		{
			yield return "numIncidentRange not configured";
		}
		if (minSpacingDays * ((FloatRange)( numIncidentsRange)).TrueMax > onDays * 0.9f)
		{
			yield return "minSpacingDays too high compared to max number of incidents.";
		}
	}
}
