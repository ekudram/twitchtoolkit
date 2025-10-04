/*
 * File: HarmonyPatches.cs
 * Project: TwitchToolkit
 * 
 * Updated: [Current Date]
 * 
 * Summary of Changes:
 * 1. Added comprehensive XML documentation
 * 2. Replaced IL-compiler generated code with clean C#
 * 3. Added null checking and error handling
 * 4. Improved UI positioning for storyteller button
 * 5. Added ToolkitLogger integration
 * 6. Maintained backward compatibility
 */
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
using HarmonyLib;
using RimWorld;
using TwitchToolkit.Storytellers.StorytellerPackWindows;
using TwitchToolkit.Utilities;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace TwitchToolkit;

[StaticConstructorOnStartup]
internal static class HarmonyPatches
{
    public static TwitchToolkit _mod;
    private static readonly Type patchType = typeof(HarmonyPatches);

    static HarmonyPatches()
    {
        try
        {
            _mod = LoadedModManager.GetMod<TwitchToolkit>();
            if (_mod == null)
            {
                ToolkitLogger.Error("Failed to get TwitchToolkit mod instance");
                return;
            }

            SaveHelper.LoadListOfViewers();

            Harmony harmony = new Harmony("com.github.harmony.rimworld.mod.twitchtoolkit");

            // Patch game save to also save mod data
            harmony.Patch(
                original: AccessTools.Method(typeof(GameDataSaveLoader), "SaveGame"),
                postfix: new HarmonyMethod(patchType, nameof(SaveGame_Postfix))
            );

            // Patch letter maker to add player messages
            harmony.Patch(
                original: AccessTools.Method(typeof(LetterMaker), "MakeLetter",
                    new Type[] { typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(Faction), typeof(Quest) }),
                prefix: new HarmonyMethod(patchType, nameof(AddLastPlayerMessagePix))
            );

            // Patch storyteller UI to add custom button
            harmony.Patch(
                original: AccessTools.Method(typeof(StorytellerUI), "DrawStorytellerSelectionInterface"),
                postfix: new HarmonyMethod(patchType, nameof(DrawCustomStorytellerInterface))
            );

            // Patch game load to handle Twitch connection
            harmony.Patch(
                original: AccessTools.Method(typeof(GameDataSaveLoader), "LoadGame", new Type[] { typeof(string) }),
                postfix: new HarmonyMethod(patchType, nameof(NewTwitchConnection))
            );

            // Patch main menu to show ToolkitUtils notification
            harmony.Patch(
                original: AccessTools.Method(typeof(MainMenuDrawer), "Init"),
                postfix: new HarmonyMethod(patchType, nameof(ToolkitUtilsNotify))
            );

            ToolkitLogger.Log("Harmony patches applied successfully");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Failed to apply harmony patches: {ex}");
        }
    }

    /// <summary>
    /// Postfix for game save to ensure mod data is also saved
    /// </summary>
    public static void SaveGame_Postfix()
    {
        try
        {
            SaveHelper.SaveAllModData();
            ToolkitLogger.Debug("Mod data saved successfully");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error saving mod data: {ex}");
        }
    }

    /// <summary>
    /// Prefix for letter creation to add player messages to letter text
    /// </summary>
    public static void AddLastPlayerMessagePix(TaggedString label, ref TaggedString text, LetterDef def)
    {
        try
        {
            if (Helper.playerMessages != null && Helper.playerMessages.Count > 0)
            {
                string msg = Helper.playerMessages[0];
                if (!text.NullOrEmpty())
                {
                    text += msg;
                }
                Helper.playerMessages.RemoveAt(0);
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in AddLastPlayerMessagePix: {ex}");
        }
    }

    /// <summary>
    /// Postfix for storyteller UI to add custom storyteller packs button
    /// </summary>
    /// <remarks>
    /// Only shows the button when a Twitch Toolkit storyteller is selected
    /// to avoid UI clutter with vanilla storytellers
    /// </remarks>
    public static void DrawCustomStorytellerInterface(Rect rect, StorytellerDef chosenStoryteller, DifficultyDef difficulty, Listing_Standard infoListing)
    {
        try
        {
            // Only show button for Twitch Toolkit storytellers or specific conditions
            bool shouldShowButton = ShouldShowStorytellerButton(chosenStoryteller);

            if (shouldShowButton)
            {
                // Position button at bottom right of the storyteller info panel
                Rect storytellerPacksButton = new Rect(rect.width - 200f, rect.height - 45f, 190f, 38f);

                if (Widgets.ButtonText(storytellerPacksButton, "Storyteller Packs", true, true, true))
                {
                    OpenStorytellerPacksWindow();
                }
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in DrawCustomStorytellerInterface: {ex}");
        }
    }

    /// <summary>
    /// Determines whether to show the storyteller packs button
    /// </summary>
    /// <param name="chosenStoryteller">The currently selected storyteller</param>
    /// <returns>True if the button should be shown</returns>
    private static bool ShouldShowStorytellerButton(StorytellerDef chosenStoryteller)
    {
        if (chosenStoryteller == null)
            return false;

        // Show button for Twitch Toolkit storytellers or always show if setting enabled
        return chosenStoryteller.modContentPack?.Name.Contains("TwitchToolkit") == true ||
               chosenStoryteller.defName.Contains("Twitch") ||
               ToolkitSettings.AlwaysShowStorytellerButton;
    }

    /// <summary>
    /// Opens the storyteller packs window
    /// </summary>
    private static void OpenStorytellerPacksWindow()
    {
        try
        {
            Window_StorytellerPacks window = new Window_StorytellerPacks();
            Find.WindowStack.TryRemove(typeof(Window_StorytellerPacks), true);
            Find.WindowStack.Add(window);
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error opening storyteller packs window: {ex}");
        }
    }

    /// <summary>
    /// Postfix for game load to handle Twitch connection re-establishment
    /// </summary>
    private static void NewTwitchConnection()
    {
        try
        {
            // Placeholder for future Twitch connection logic
            // This could re-establish Twitch connection after game load
            ToolkitLogger.Debug("Game loaded, Twitch connection handling placeholder");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in NewTwitchConnection: {ex}");
        }
    }

    /// <summary>
    /// Postfix for main menu initialization to show ToolkitUtils notification
    /// </summary>
    private static void ToolkitUtilsNotify()
    {
        try
        {
            if (!ToolkitSettings.NotifiedAboutUtils && !ToolkitUtilsChecker.ToolkitUtilsActive)
            {
                ToolkitSettings.NotifiedAboutUtils = true;

                Window_ToolkitUtils window = new Window_ToolkitUtils();
                Find.WindowStack.TryRemove(typeof(Window_ToolkitUtils), true);
                Find.WindowStack.Add(window);

                ToolkitLogger.Debug("Showed ToolkitUtils notification");
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in ToolkitUtilsNotify: {ex}");
        }
    }
}

/** * HarmonyPatches.cs
using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using TwitchToolkit.Storytellers.StorytellerPackWindows;
using TwitchToolkit.Utilities;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace TwitchToolkit;

[StaticConstructorOnStartup]
internal static class HarmonyPatches
{
	public static TwitchToolkit _mod;

	private static readonly Type patchType;

	static HarmonyPatches()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing erences)
		//IL_002b: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0063: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing erences)
		//IL_00d9: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0109: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing erences)
		//IL_014b: Expected O, but got Unknown
		//IL_016f: Unknown result type (might be due to invalid IL or missing erences)
		//IL_017b: Expected O, but got Unknown
		_mod = LoadedModManager.GetMod<TwitchToolkit>();
		patchType = typeof(HarmonyPatches);
		SaveHelper.LoadListOfViewers();
		Harmony harmony = new Harmony("com.github.harmony.rimworld.mod.twitchtoolkit");
		harmony.Patch((MethodBase)AccessTools.Method(typeof(GameDataSaveLoader), "SaveGame", (Type[])null, (Type[])null), (HarmonyMethod)null, new HarmonyMethod(typeof(HarmonyPatches).GetMethod("SaveGame_Postfix")), (HarmonyMethod)null, (HarmonyMethod)null);
		harmony.Patch((MethodBase)AccessTools.Method(typeof(LetterMaker), "MakeLetter", new Type[5]
		{
			typeof(TaggedString),
			typeof(TaggedString),
			typeof(LetterDef),
			typeof(Faction),
			typeof(Quest)
		}, (Type[])null), new HarmonyMethod(patchType, "AddLastPlayerMessagePix", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
		harmony.Patch((MethodBase)AccessTools.Method(typeof(StorytellerUI), "DrawStorytellerSelectionInterface", (Type[])null, (Type[])null), (HarmonyMethod)null, new HarmonyMethod(patchType, "DrawCustomStorytellerInterface", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null);
		harmony.Patch((MethodBase)AccessTools.Method(typeof(GameDataSaveLoader), "LoadGame", new Type[1] { typeof(string) }, (Type[])null), (HarmonyMethod)null, new HarmonyMethod(patchType, "NewTwitchConnection", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null);
		harmony.Patch((MethodBase)AccessTools.Method(typeof(MainMenuDrawer), "Init", (Type[])null, (Type[])null), (HarmonyMethod)null, new HarmonyMethod(patchType, "ToolkitUtilsNotify", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null);
	}

	public static void SaveGame_Postfix()
	{
		TwitchToolkit mod = LoadedModManager.GetMod<TwitchToolkit>();
		SaveHelper.SaveAllModData();
	}

	public static void AddLastPlayerMessagePix(TaggedString label,  TaggedString text, LetterDef def)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing erences)
		//IL_003b: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0041: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0046: Unknown result type (might be due to invalid IL or missing erences)
		if (Helper.playerMessages.Count > 0)
		{
			string msg = Helper.playerMessages[0];
			if ((TaggedString)(text) != "")
			{
				text += msg;
			}
			Helper.playerMessages.RemoveAt(0);
		}
	}

	public static void DrawCustomStorytellerInterface(Rect rect,  StorytellerDef chosenStoryteller,  DifficultyDef difficulty, Listing_Standard infoListing)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing erences)
		Rect storytellerPacksButton = new Rect(140f, ((Rect)(rect)).height - 10f, 190f, 38f);
		if (Widgets.ButtonText(storytellerPacksButton, "Storyteller Packs", true, true, true))
		{
			Window_StorytellerPacks window = new Window_StorytellerPacks();
			Find.WindowStack.TryRemove(((object)window).GetType(), true);
			Find.WindowStack.Add((Window)(object)window);
		}
	}

	private static void NewTwitchConnection()
	{
	}

	private static void ToolkitUtilsNotify()
	{
		if (!ToolkitSettings.NotifiedAboutUtils && !ToolkitUtilsChecker.ToolkitUtilsActive)
		{
			ToolkitSettings.NotifiedAboutUtils = true;
			Window_ToolkitUtils window = new Window_ToolkitUtils();
			Find.WindowStack.TryRemove(((object)window).GetType(), true);
			Find.WindowStack.Add((Window)(object)window);
		}
	}
}
**/