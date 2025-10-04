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

namespace TwitchToolkit.IncidentHelpers.Special;

public static class ItemHelper
{
	public static void setItemQualityRandom(Thing thing)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0006: Unknown result type (might be due to invalid IL or missing erences)
		//IL_000d: Unknown result type (might be due to invalid IL or missing erences)
		QualityCategory qual = QualityUtility.GenerateQualityTraderItem();
		ThingCompUtility.TryGetComp<CompQuality>(thing).SetQuality(qual, (ArtGenerationContext)0);
	}

	public static LetterDef GetLetterFromValue(int value)
	{
		if (value <= 250)
		{
			return LetterDefOf.NeutralEvent;
		}
		if (value > 250 && value <= 750)
		{
			return DefDatabase<LetterDef>.GetNamed("BlueLetter", true);
		}
		if (value > 750 && value <= 1500)
		{
			return DefDatabase<LetterDef>.GetNamed("GreenLetter", true);
		}
		return DefDatabase<LetterDef>.GetNamed("GoldLetter", true);
	}
}
