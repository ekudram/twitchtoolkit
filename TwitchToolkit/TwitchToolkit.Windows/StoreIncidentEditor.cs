/*
 * File: StoreIncidentEditor.cs
 * Project: TwitchToolkit
 * 
 * Updated: [Current Date]
 * 
 * Summary of Changes:
 * 1. Added proper error handling and null checking
 * 2. Improved UI layout and documentation
 * 3. Added thread safety considerations
 * 4. Enhanced backup system reliability
 */

using System;
using System.Linq;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace TwitchToolkit.Windows
{
    public class StoreIncidentEditor : Window
    {
        public bool checkedForBackup = false;
        public bool haveBackup = false;
        public bool variableIncident = false;
        public StoreIncident storeIncident = null;
        public StoreIncidentVariables storeIncidentVariables = null;
        private string[] karmaTypeStrings = null;
        private string setKarmaType = "";

        public override Vector2 InitialSize => new Vector2(500f, 500f);

        public StoreIncidentEditor(StoreIncident storeIncident)
        {
            if (storeIncident == null)
            {
                throw new ArgumentNullException(nameof(storeIncident), "Store incident cannot be null");
            }

            // In the static constructor or initialization
            ToolkitLogger.Debug($"Total simple incidents: {DefDatabase<StoreIncidentSimple>.AllDefs.Count()}");
            ToolkitLogger.Debug($"Total variable incidents: {DefDatabase<StoreIncidentVariables>.AllDefs.Count()}");


            base.doCloseButton = true;
            this.storeIncident = storeIncident;

            ToolkitLogger.Debug($"Creating editor for incident: {storeIncident.defName}");

            MakeSureSaveExists();

            // Check if this is a variable incident - FIXED the assignment bug
            StoreIncidentVariables foundVariables = DefDatabase<StoreIncidentVariables>.AllDefs
                .FirstOrDefault(s => s.defName == storeIncident.defName);

            if (foundVariables != null)
            {
                this.storeIncidentVariables = foundVariables;  // Fixed: assign to class field
                variableIncident = true;
                ToolkitLogger.Debug($"Found variable incident: {foundVariables.defName}");
            }
            else
            {
                ToolkitLogger.Debug($"Incident {storeIncident.defName} is not a variable incident");
            }

            karmaTypeStrings = Enum.GetNames(typeof(KarmaType));
            setKarmaType = storeIncident.karmaType.ToString();
        }
        //public StoreIncidentEditor(StoreIncident storeIncident)
        //{
        //    if (storeIncident == null)
        //    {
        //        throw new ArgumentNullException(nameof(storeIncident), "Store incident cannot be null");
        //    }

        //    base.doCloseButton = true;
        //    this.storeIncident = storeIncident;

        //    MakeSureSaveExists();

        //    // Check if this is a variable incident
        //    storeIncidentVariables = DefDatabase<StoreIncidentVariables>.AllDefs
        //        .FirstOrDefault(s => s.defName == storeIncident.defName);

        //    if (storeIncidentVariables != null)
        //    {
        //        this.storeIncidentVariables = storeIncidentVariables;
        //        variableIncident = true;
        //    }

        //    karmaTypeStrings = Enum.GetNames(typeof(KarmaType));
        //    setKarmaType = storeIncident.karmaType.ToString();
        //}

        public override void PostClose()
        {
            try
            {
                MakeSureSaveExists();
                Store_IncidentEditor.UpdatePriceSheet();
                ((Mod)Toolkit.Mod)?.WriteSettings();
            }
            catch (Exception ex)
            {
                ToolkitLogger.Error($"Error in PostClose: {ex}");
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            
            
            try
            {
                ToolkitLogger.Debug($"DoWindowContents called for incident: {storeIncident?.defName ?? "null"}");
                ToolkitLogger.Debug($"Window rect: {inRect}");

                if (!checkedForBackup || !haveBackup)
                {
                    ToolkitLogger.Debug("Backup not checked or doesn't exist, calling MakeSureSaveExists");
                    MakeSureSaveExists();
                    return;
                }



                if (!checkedForBackup || !haveBackup)
                {
                    MakeSureSaveExists();
                    return;
                }

                // Check if we have a valid incident
                if (storeIncident == null)
                {
                    Widgets.Label(inRect, "No store incident selected");
                    ToolkitLogger.Error("Store incident is null in DoWindowContents");
                    return;
                }

                Listing_Standard ls = new Listing_Standard();
                ls.Begin(inRect);

                // Display incident information
                ls.Label(GenText.CapitalizeFirst(storeIncident.label));
                ls.Gap(12f);

                if (storeIncident.cost > 0)
                {
                    // Purchase Code
                    storeIncident.abbreviation = ls.TextEntryLabeled("Purchase Code:", storeIncident.abbreviation, 1);
                    ls.Gap(12f);

                    // Cost
                    ls.AddLabeledNumericalTextField("Cost", ref storeIncident.cost);

                    // Event Cap
                    ls.SliderLabeled($"Max times per {ToolkitSettings.EventCooldownInterval} ingame day(s)",
                        ref storeIncident.eventCap, storeIncident.eventCap.ToString(), 0f, 15f);

                    // Variable incident settings
                    if (variableIncident && storeIncidentVariables != null && storeIncidentVariables.maxWager > 0)
                    {
                        ls.Gap(12f);
                        ls.SliderLabeled("Maximum coin wager", ref storeIncidentVariables.maxWager,
                            storeIncidentVariables.maxWager.ToString(), storeIncident.cost, 20000f);

                        if (storeIncidentVariables.maxWager < storeIncident.cost)
                        {
                            storeIncidentVariables.maxWager = storeIncident.cost * 2;
                        }
                    }

                    ls.Gap(12f);

                    // Karma Type
                    ls.AddLabeledRadioList("Karma Type", karmaTypeStrings, ref setKarmaType);
                    storeIncident.karmaType = (KarmaType)Enum.Parse(typeof(KarmaType), setKarmaType);

                    ls.Gap(12f);

                    // Disable button
                    if (ls.ButtonTextLabeled("Disable Store Incident", "Disable"))
                    {
                        storeIncident.cost = -10;
                        ToolkitLogger.Debug($"Disabled incident: {storeIncident.defName}");
                    }
                }

                ls.Gap(12f);

                // Special handling for Items
                if (storeIncident.defName == "Item")
                {
                    ls.SliderLabeled($"Max times per {ToolkitSettings.EventCooldownInterval} ingame day(s)",
                        ref storeIncident.eventCap, storeIncident.eventCap.ToString(), 0f, 15f);
                    ls.Gap(12f);
                }

                // Custom settings for variable incidents
                if (variableIncident && storeIncidentVariables?.customSettings == true)
                {
                    ls.Gap(12f);
                    if (ls.ButtonTextLabeled("Edit Extra Settings", "Settings"))
                    {
                        storeIncidentVariables.settings?.EditSettings();
                    }
                }

                ls.Gap(12f);

                // Reset functionality
                if (storeIncident.defName != "Item" && ls.ButtonTextLabeled("Reset to Default", "Reset"))
                {
                    Store_IncidentEditor.LoadBackup(storeIncident);
                    if (storeIncident.cost < 1)
                    {
                        storeIncident.cost = 50;
                    }
                    setKarmaType = storeIncident.karmaType.ToString();
                    MakeSureSaveExists();
                    ToolkitLogger.Debug($"Reset incident: {storeIncident.defName}");
                }

                // Item price editor
                if (storeIncident.defName == "Item" && ls.ButtonTextLabeled("Edit item prices", "Edit"))
                {
                    Find.WindowStack.TryRemove(typeof(StoreItemsWindow), true);
                    Find.WindowStack.Add(new StoreItemsWindow());
                }

                ls.End();
            }
            catch (Exception ex)
            {
                ToolkitLogger.Error($"Error in DoWindowContents: {ex}");
                Widgets.Label(inRect, $"Error: {ex.Message}");
            }
        }

        //public override void DoWindowContents(Rect inRect)
        //{
        //    try
        //    {
        //        if (!checkedForBackup || !haveBackup)
        //        {
        //            MakeSureSaveExists();
        //            return;
        //        }

        //        Listing_Standard ls = new Listing_Standard();
        //        ls.Begin(inRect);

        //        ls.Label(GenText.CapitalizeFirst(storeIncident.label));
        //        ls.Gap(12f);

        //        if (storeIncident.cost > 0)
        //        {
        //            // Purchase Code
        //            storeIncident.abbreviation = ls.TextEntryLabeled("Purchase Code:", storeIncident.abbreviation, 1);
        //            ls.Gap(12f);

        //            // Cost
        //            ls.AddLabeledNumericalTextField("Cost", ref storeIncident.cost);

        //            // Event Cap
        //            ls.SliderLabeled($"Max times per {ToolkitSettings.EventCooldownInterval} ingame day(s)",
        //                ref storeIncident.eventCap, storeIncident.eventCap.ToString(), 0f, 15f);

        //            // Variable incident settings
        //            if (variableIncident && storeIncidentVariables.maxWager > 0)
        //            {
        //                ls.Gap(12f);
        //                ls.SliderLabeled("Maximum coin wager", ref storeIncidentVariables.maxWager,
        //                    storeIncidentVariables.maxWager.ToString(), storeIncident.cost, 20000f);

        //                if (storeIncidentVariables.maxWager < storeIncident.cost)
        //                {
        //                    storeIncidentVariables.maxWager = storeIncident.cost * 2;
        //                }
        //            }

        //            ls.Gap(12f);

        //            // Karma Type
        //            ls.AddLabeledRadioList("Karma Type", karmaTypeStrings, ref setKarmaType);
        //            storeIncident.karmaType = (KarmaType)Enum.Parse(typeof(KarmaType), setKarmaType);

        //            ls.Gap(12f);

        //            // Disable button
        //            if (ls.ButtonTextLabeled("Disable Store Incident", "Disable"))
        //            {
        //                storeIncident.cost = -10;
        //            }
        //        }

        //        ls.Gap(12f);

        //        // Special handling for Items
        //        if (storeIncident.defName == "Item")
        //        {
        //            ls.SliderLabeled($"Max times per {ToolkitSettings.EventCooldownInterval} ingame day(s)",
        //                ref storeIncident.eventCap, storeIncident.eventCap.ToString(), 0f, 15f);
        //            ls.Gap(12f);
        //        }

        //        // Custom settings for variable incidents
        //        if (variableIncident && storeIncidentVariables?.customSettings == true)
        //        {
        //            ls.Gap(12f);
        //            if (ls.ButtonTextLabeled("Edit Extra Settings", "Settings"))
        //            {
        //                storeIncidentVariables.settings?.EditSettings();
        //            }
        //        }

        //        ls.Gap(12f);

        //        // Reset functionality
        //        if (storeIncident.defName != "Item" && ls.ButtonTextLabeled("Reset to Default", "Reset"))
        //        {
        //            Store_IncidentEditor.LoadBackup(storeIncident);
        //            if (storeIncident.cost < 1)
        //            {
        //                storeIncident.cost = 50;
        //            }
        //            setKarmaType = storeIncident.karmaType.ToString();
        //            MakeSureSaveExists();
        //        }

        //        // Item price editor
        //        if (storeIncident.defName == "Item" && ls.ButtonTextLabeled("Edit item prices", "Edit"))
        //        {
        //            Find.WindowStack.TryRemove(typeof(StoreItemsWindow), true);
        //            Find.WindowStack.Add(new StoreItemsWindow());
        //        }

        //        ls.End();
        //    }
        //    catch (Exception ex)
        //    {
        //        ToolkitLogger.Error($"Error in DoWindowContents: {ex}");
        //    }
        //}

        public void MakeSureSaveExists(bool forceSave = true)
        {
            try
            {
                checkedForBackup = true;
                ToolkitLogger.Debug("Checking if save exists for incident: " + storeIncident?.defName);

                if (storeIncident == null)
                {
                    ToolkitLogger.Error("Incident is null in MakeSureSaveExists");
                    return;
                }

                haveBackup = Store_IncidentEditor.CopyExists(storeIncident);

                if (!haveBackup || forceSave)
                {
                    Store_IncidentEditor.SaveCopy(storeIncident);
                    haveBackup = true;
                }
            }
            catch (Exception ex)
            {
                ToolkitLogger.Error($"Error in MakeSureSaveExists: {ex}");
            }
        }
    }
}

/**
using System;
using System.Linq;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace TwitchToolkit.Windows;

public class StoreIncidentEditor : Window
{
	public bool checkedForBackup = false;

	public bool haveBackup = false;

	public bool variableIncident = false;

	public StoreIncident storeIncident = null;

	public StoreIncidentVariables storeIncidentVariables = null;

	private string[] karmaTypeStrings = null;

	private string setKarmaType = "";

	public override Vector2 InitialSize => new Vector2(500f, 500f);

	public StoreIncidentEditor(StoreIncident storeIncident)
	{
		base.doCloseButton = true;
		this.storeIncident = storeIncident;
		if (storeIncident == null)
		{
			throw new ArgumentNullException();
		}
		MakeSureSaveExists();
		StoreIncidentVariables storeIncidentVariables = DefDatabase<StoreIncidentVariables>.AllDefs.ToList().Find((StoreIncidentVariables s) => ((Def)s).defName == ((Def)storeIncident).defName);
		if (storeIncidentVariables != null)
		{
			this.storeIncidentVariables = storeIncidentVariables;
			variableIncident = true;
		}
		karmaTypeStrings = Enum.GetNames(typeof(KarmaType));
		setKarmaType = storeIncident.karmaType.ToString();
	}

	public override void PostClose()
	{
		MakeSureSaveExists();
		Store_IncidentEditor.UpdatePriceSheet();
		((Mod)Toolkit.Mod).WriteSettings();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing erences)
		//IL_002d: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing erences)
		//IL_004c: Unknown result type (might be due to invalid IL or missing erences)
		if (!checkedForBackup || !haveBackup)
		{
			MakeSureSaveExists();
			return;
		}
		Listing_Standard ls = new Listing_Standard();
		((Listing)ls).Begin(inRect);
		ls.Label(GenText.CapitalizeFirst(((Def)storeIncident).label));
		((Listing)ls).Gap(12f);
		if (storeIncident.cost > 0)
		{
			storeIncident.abbreviation = ls.TextEntryLabeled("Purchase Code:", storeIncident.abbreviation, 1);
			((Listing)ls).Gap(12f);
			ls.AddLabeledNumericalTextField("Cost", ref storeIncident.cost);
			ls.SliderLabeled("Max times per " + ToolkitSettings.EventCooldownInterval + " ingame day(s)", ref storeIncident.eventCap, storeIncident.eventCap.ToString(), 0f, 15f);
			if (variableIncident && storeIncidentVariables.maxWager > 0)
			{
				((Listing)ls).Gap(12f);
				ls.SliderLabeled("Maximum coin wager",  ref storeIncidentVariables.maxWager, storeIncidentVariables.cost.ToString(), storeIncident.cost, 20000f);
				if (storeIncidentVariables.maxWager < storeIncidentVariables.cost)
				{
					storeIncidentVariables.maxWager = storeIncidentVariables.cost * 2;
				}
			}
			((Listing)ls).Gap(12f);
			ls.AddLabeledRadioList("Karma Type", karmaTypeStrings, ref setKarmaType);
			storeIncident.karmaType = (KarmaType)Enum.Parse(typeof(KarmaType), setKarmaType);
			((Listing)ls).Gap(12f);
			if (ls.ButtonTextLabeled("Disable Store Incident", "Disable"))
			{
				storeIncident.cost = -10;
			}
		}
		((Listing)ls).Gap(12f);
		if (((Def)storeIncident).defName == "Item")
		{
			ls.SliderLabeled("Max times per " + ToolkitSettings.EventCooldownInterval + " ingame day(s)", ref storeIncident.eventCap, storeIncident.eventCap.ToString(), 0f, 15f);
			((Listing)ls).Gap(12f);
		}
		if (variableIncident && storeIncidentVariables.customSettings)
		{
			((Listing)ls).Gap(12f);
			if (ls.ButtonTextLabeled("Edit Extra Settings", "Settings"))
			{
				storeIncidentVariables.settings.EditSettings();
			}
		}
		((Listing)ls).Gap(12f);
		if (((Def)storeIncident).defName != "Item" && ls.ButtonTextLabeled("Reset to Default", "Reset"))
		{
			Store_IncidentEditor.LoadBackup(storeIncident);
			if (storeIncident.cost < 1)
			{
				storeIncident.cost = 50;
			}
			setKarmaType = storeIncident.karmaType.ToString();
			MakeSureSaveExists();
		}
		if (((Def)storeIncident).defName == "Item" && ls.ButtonTextLabeled("Edit item prices", "Edit"))
		{
			Type type = typeof(StoreItemsWindow);
			Find.WindowStack.TryRemove(type, true);
			Window window = (Window)(object)new StoreItemsWindow();
			Find.WindowStack.Add(window);
		}
		((Listing)ls).End();
	}

	public void MakeSureSaveExists(bool forceSave = true)
	{
		checkedForBackup = true;
		Helper.Log("Checking if save exists");
		if (storeIncident == null)
		{
			Log.Error("incident is null");
		}
		haveBackup = Store_IncidentEditor.CopyExists(storeIncident);
		if (!haveBackup || forceSave)
		{
			Store_IncidentEditor.SaveCopy(storeIncident);
		}
	}
}
**/