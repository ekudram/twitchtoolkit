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

namespace TwitchToolkit.IncidentHelpers.Traits;

public class BuyableTrait
{
	public TraitDef def;

	public string label;

	public int degree;

	public BuyableTrait(TraitDef def, string label, int degree = 0)
	{
		this.def = def;
		this.label = label;
		this.degree = degree;
	}
}
