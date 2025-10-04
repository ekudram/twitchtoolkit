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

namespace TwitchToolkit.Storytellers;

public class StorytellerCompProperties_ToryTalker : StorytellerCompProperties
{
	public StorytellerCompProperties_ToryTalker()
	{
		base.compClass = typeof(StorytellerComp_ToryTalker);
	}
}
