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
using System;
using RimWorld;
using Verse;

namespace TwitchToolkit.Storytellers;

public class StorytellerPack : Def
{
	public bool enabled = false;

	public int minDaysBetweenEvents = 1;

	public Type storytellerCompPropertiesType = typeof(StorytellerCompProperties);

	public StorytellerComp storytellerComp = null;

	public StorytellerCompProperties storytellerCompProps = null;

	public StorytellerComp StorytellerComp
	{
		get
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing erences)
			//IL_0026: Expected O, but got Unknown
			//IL_0052: Unknown result type (might be due to invalid IL or missing erences)
			//IL_005c: Expected O, but got Unknown
			try
			{
				if (storytellerComp == null)
				{
					storytellerCompProps = (StorytellerCompProperties)Activator.CreateInstance(storytellerCompPropertiesType);
					storytellerCompProps.ResolveReferences(Current.Game.storyteller.def);
					storytellerComp = (StorytellerComp)Activator.CreateInstance(storytellerCompProps.compClass);
					storytellerComp.props = storytellerCompProps;
				}
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
			}
			return storytellerComp;
		}
	}
}
