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
