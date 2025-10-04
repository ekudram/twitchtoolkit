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
using Verse;

namespace TwitchToolkit.Settings;

[StaticConstructorOnStartup]
public static class Settings_ToolkitExtensions
{
	public static List<ToolkitExtension> GetExtensions { get; private set; }

	static Settings_ToolkitExtensions()
	{
		GetExtensions = new List<ToolkitExtension>();
	}

	public static void RegisterExtension(ToolkitExtension extension)
	{
		GetExtensions.Add(extension);
	}
}
