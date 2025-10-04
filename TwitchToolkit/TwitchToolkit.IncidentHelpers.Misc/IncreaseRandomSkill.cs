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
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.IncidentHelpers.Misc;

public class IncreaseRandomSkill : IncidentHelper
{
	private Pawn pawn = null;

	private SkillRecord skill = null;

	public override bool IsPossible()
	{
		List<Pawn> allPawns = Helper.AnyPlayerMap.mapPawns.FreeColonistsSpawned.ToList();
		allPawns.Shuffle();
		foreach (Pawn pawn in allPawns)
		{
			List<SkillRecord> allSkills = pawn.skills.skills;
			allSkills.Shuffle();
			foreach (SkillRecord skill in allSkills)
			{
				if (skill.TotallyDisabled)
				{
					continue;
				}
				this.skill = skill;
				this.pawn = pawn;
				break;
			}
		}
		return this.skill != null;
	}

	public override void TryExecute()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0082: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0088: Unknown result type (might be due to invalid IL or missing erences)
		float xpBoost = SkillRecord.XpRequiredToLevelUpFrom(skill.Level);
		skill.Learn(xpBoost, true);
		string text = Helper.ReplacePlaceholder((TaggedString)(Translator.Translate("TwitchStoriesDescription55")), ((object)pawn.Name).ToString(), null, null, null, ((Def)skill.def).defName, null, null, null, null, null, null, null, null, null, null, Math.Round(xpBoost).ToString());
		Current.Game.letterStack.ReceiveLetter(Translator.Translate("TwitchToolkitIncreaseSkill"), (TaggedString)(text), LetterDefOf.PositiveEvent, (LookTargets)((Thing)(object)pawn), (Faction)null, (Quest)null, (List<ThingDef>)null, (string)null);
	}
}
