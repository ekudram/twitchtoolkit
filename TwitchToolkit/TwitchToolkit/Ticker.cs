/*
 * File: 
 * Project: TwitchToolkit
 * 
 * Updated: September 16, 2025
 * 
 * Class Use:  (if known)
 * Summary of Changes:
 * 1. Maintains backward compatibility with the timer field that ToolkitUtils expects
 * 2. Removes threading by eliminating the separate registration thread
 * 3. Uses RimWorld's tick system for periodic registration checks
 * 4. Includes proper error handling with ToolkitLogger
 * 5. Adds cleanup with the Destroy method
 * 6. Preserves all original functionality while making it thread-safe
 * Reason for Change:

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
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TwitchToolkit.Store;
using TwitchToolkit.Votes;
using UnityEngine;
using Verse;

namespace TwitchToolkit;

public class Ticker : Thing
{
    // Add back the timer field for backward compatibility with ToolkitUtils
    // Mark it as obsolete to warn other modders not to use it
    [Obsolete("This field is maintained for backward compatibility only. Do not use in new code.")]
    public System.Threading.Timer timer = null;

    public static long LastIRCPong = 0L;
    public static DateTime lastEvent;

    public static Queue<FiringIncident> FiringIncidents = new Queue<FiringIncident>();
    public static Queue<VoteEvent> VoteEvents = new Queue<VoteEvent>();
    public static Queue<IncidentWorker> Incidents = new Queue<IncidentWorker>();
    public static Queue<IncidentHelper> IncidentHelpers = new Queue<IncidentHelper>();
    public static Queue<IncidentHelperVariables> IncidentHelperVariables = new Queue<IncidentHelperVariables>();

    private static Game _game;
    private static TwitchToolkit _mod = Toolkit.Mod;
    private static Ticker _instance;

    private int[] _baseTimes = new int[5] { 20, 60, 120, 180, 999999 };
    private int _lastMinute = -1;
    private int _lastCoinReward = -1;

    // Add tick counter for registration checks
    private int _registrationCheckTickCounter = 0;
    private const int RegistrationCheckInterval = 60; // Check every 60 ticks (~1 second)

    public bool CreatedByController { get; internal set; }


    public static Ticker Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Ticker();
            }
            return _instance;
        }
    }

    public Ticker()
    {
        base.def = new ThingDef
        {
            tickerType = TickerType.Normal,
            isSaveable = false
        };

        // Initialize the timer field for backward compatibility
        // Create a dummy timer that doesn't actually do anything
#pragma warning disable CS0618 // Type or member is obsolete
        timer = new System.Threading.Timer(_ => {
            // Empty callback - this timer doesn't actually do anything
            ToolkitLogger.Warning("Legacy timer callback called - this timer is deprecated and does nothing");
        }, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
#pragma warning restore CS0618 // Type or member is obsolete

        // Initial registration attempt
        TryRegisterWithCurrentGame();

        lastEvent = DateTime.Now;
        LastIRCPong = DateTime.Now.ToFileTime();
    }

    // Replace thread-based registration with tick-based registration
    private void TryRegisterWithCurrentGame()
    {
        try
        {
            if (_game != Current.Game)
            {
                if (_game != null)
                {
                    _game.tickManager.DeRegisterAllTickabilityFor(this);
                    _game = null;
                }

                _game = Current.Game;
                if (_game != null)
                {
                    _game.tickManager.RegisterAllTickabilityFor(this);
                    Toolkit.Mod.RegisterTicker();
                    ToolkitLogger.Log("Ticker successfully registered with current game");
                }
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Exception in TryRegisterWithCurrentGame: {ex.Message}\n{ex.StackTrace}");
        }
    }

    protected override void Tick()
    {
        try
        {
            // Check for game registration periodically
            _registrationCheckTickCounter++;
            if (_registrationCheckTickCounter >= RegistrationCheckInterval)
            {
                _registrationCheckTickCounter = 0;
                TryRegisterWithCurrentGame();
            }

            if (_game == null || _mod == null)
            {
                return;
            }

            _mod.Tick();
            int minutes = (int)(_game.Info.RealPlayTimeInteracting / 60f);
            double getTime = (double)Time.time / 60.0;
            int time = Convert.ToInt32(Math.Truncate(getTime));

            if (IncidentHelpers.Count > 0)
            {
                while (IncidentHelpers.Count > 0)
                {
                    IncidentHelper incidentHelper2 = IncidentHelpers.Dequeue();
                    if (!(incidentHelper2 is VotingHelper))
                    {
                        Purchase_Handler.QueuePlayerMessage(incidentHelper2.Viewer, incidentHelper2.message);
                    }
                    incidentHelper2.TryExecute();
                }
                Helper.playerMessages = new List<string>();
            }

            if (IncidentHelperVariables.Count > 0)
            {
                while (IncidentHelperVariables.Count > 0)
                {
                    IncidentHelperVariables incidentHelper = IncidentHelperVariables.Dequeue();
                    Purchase_Handler.QueuePlayerMessage(incidentHelper.Viewer, incidentHelper.message, incidentHelper.storeIncident.variables);
                    incidentHelper.TryExecute();
                    if (Purchase_Handler.viewerNamesDoingVariableCommands.Contains(incidentHelper.Viewer.username))
                    {
                        Purchase_Handler.viewerNamesDoingVariableCommands.Remove(incidentHelper.Viewer.username);
                    }
                }
                Helper.playerMessages = new List<string>();
            }

            if (Incidents.Count > 0)
            {
                IncidentWorker incident2 = Incidents.Dequeue();
                IncidentParms incidentParms = new IncidentParms();
                incidentParms.target = (IIncidentTarget)(object)Helper.AnyPlayerMap;
                incident2.TryExecute(incidentParms);
            }

            if (FiringIncidents.Count > 0)
            {
                ToolkitLogger.Log("Firing " + ((Def)FiringIncidents.First().def).defName);
                FiringIncident incident = FiringIncidents.Dequeue();
                incident.def.Worker.TryExecute(incident.parms);
            }

            VoteHandler.CheckForQueuedVotes();

            if (_lastCoinReward < 0)
            {
                _lastCoinReward = time;
            }
            else if (ToolkitSettings.EarningCoins && time - _lastCoinReward >= ToolkitSettings.CoinInterval && Viewers.jsonallviewers != null)
            {
                _lastCoinReward = time;
                Viewers.AwardViewersCoins();
            }

            if (_lastMinute < 0)
            {
                _lastMinute = time;
            }
            else if (_lastMinute < time)
            {
                _lastMinute = time;
                Toolkit.JobManager.CheckAllJobs();
                Viewers.RefreshViewers();
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Exception in Ticker.Tick: {ex.Message}{ex.StackTrace}");
        }
    }

    // Add proper cleanup when the ticker is destroyed
    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        if (_game != null)
        {
            _game.tickManager.DeRegisterAllTickabilityFor(this);
            _game = null;
        }
        base.Destroy(mode);
    }
}