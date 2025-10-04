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

using RimWorld;
using Verse;

namespace TwitchToolkit;

public class StorytellerCompProperties_CustomCategoryMTB : StorytellerCompProperties
{
	public float mtbDays = -1f;

	public SimpleCurve mtbDaysFactorByDaysPassedCurve;

	public IncidentCategoryDef category;

	public StorytellerCompProperties_CustomCategoryMTB()
	{
		base.compClass = typeof(StorytellerComp_CustomCategoryMTB);
	}
}
