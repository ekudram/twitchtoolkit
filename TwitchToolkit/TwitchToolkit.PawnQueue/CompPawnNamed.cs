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
using Verse;

namespace TwitchToolkit.PawnQueue;

public class CompPawnNamed : ThingComp
{
	public CompProperties_PawnNamed PropsName => (CompProperties_PawnNamed)(object)base.props;

	public override void PostExposeData()
	{
        ToolkitLogger.Debug($"=== ExposeData CompPawnNamed called! Mode: {Scribe.mode}");
        Scribe_Values.Look<bool>(ref PropsName.isNamed, "isNamed", false, false);
	}
}
