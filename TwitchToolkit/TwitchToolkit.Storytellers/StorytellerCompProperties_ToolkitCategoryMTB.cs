/*
 * File: StorytellerCompProperties_ToolkitCategoryMTB.cs
 * 
 * Project: TwitchToolkit
 * 
 * Updated: September 16, 2025
 * Changes:
 * 	public new float minDaysPassed = 5f;  from 	public float minDaysPassed = 5f;
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
using RimWorld;
using Verse;

namespace TwitchToolkit.Storytellers;

public class StorytellerCompProperties_ToolkitCategoryMTB : StorytellerCompProperties
{
	public float mtbDays = 3f;
	public SimpleCurve mtbDaysFactorByDaysPassedCurve;
	public IncidentCategoryDef category = IncidentCategoryDefOf.Misc;
	public new float minDaysPassed = 5f;

	public StorytellerCompProperties_ToolkitCategoryMTB()
	{
		base.compClass = typeof(StorytellerComp_ToolkitCategoryMTB);
	}
}
