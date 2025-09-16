/* This file is part of TwitchToolkit.
	TwitchToolkit is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.
	TwitchToolkit is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.
	You should have received a copy of the GNU General Public License
	along with TwitchToolkit.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * File: Settings_Patches.cs
 * 

 */

using System;
using System.Reflection;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace TwitchToolkit.Settings;

internal class Settings_Patches
{
    public static void DoWindowContents(Rect rect, Listing_Standard optionsListing)
    {
        optionsListing.Label("Patches");
        optionsListing.Gap(12f);

        foreach (ToolkitExtension extension in Settings_ToolkitExtensions.GetExtensions)
        {
            if (optionsListing.ButtonTextLabeled(extension.mod.SettingsCategory(), "Settings"))
            {
                ConstructorInfo constructor = extension.windowType.GetConstructor(new Type[1] { typeof(Mod) });
                SettingsWindow window = constructor.Invoke(new object[1] { extension.mod }) as SettingsWindow;
                Type type = typeof(SettingsWindow);
                Find.WindowStack.TryRemove(type, true);
                Find.WindowStack.Add(window);
            }
        }
    }
}
