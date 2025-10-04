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
using System.Linq;
using RimWorld;
using Verse;

namespace TwitchToolkit.IncidentHelpers.Traits;

[StaticConstructorOnStartup]
public static class AllTraits
{
	public static List<BuyableTrait> buyableTraits;

	static AllTraits()
	{
		buyableTraits = new List<BuyableTrait>();
		List<TraitDef> traitDefs = DefDatabase<TraitDef>.AllDefs.ToList();
		foreach (TraitDef def in traitDefs)
		{
			if (def.degreeDatas != null)
			{
				foreach (TraitDegreeData degree in def.degreeDatas)
				{
					buyableTraits.Add(new BuyableTrait(def, string.Join("", degree.label.Split(' ')).ToLower(), degree.degree));
				}
			}
			else
			{
				buyableTraits.Add(new BuyableTrait(def, string.Join("", ((Def)def).label.Split(' ')).ToLower()));
			}
		}
	}
}
