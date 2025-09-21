/*
 * File: Viewers.cs
 * Project: TwitchToolkit
 * 
 * Date Created: September 14, 2025
 * 
 * Key Changes Made:
 * 1. Added comprehensive error handling with try-catch blocks in all methods
 * 2. Enhanced null checking throughout the codebase
 * 3. Improved logging using ToolkitLogger with appropriate log levels (Debug, Warn, Error, Log)
 * 4. Maintained backward compatibility - all public static members remain unchanged
 * 5. Added descriptive error messages to help with debugging
 * 6. Fixed potential null reference exceptions with safe navigation operators (?.)
 * 7. Improved JSON parsing safety with null checks on JSON nodes
 * 8. Added input validation for empty usernames and other parameters
 * 9. Preserved all existing functionality while making error handling more robust
 * 10. Added calls to RemoveDuplicateViewers in key methods to ensure no duplicate viewers exist
 * 11. Add thread safety with a lock object:
 * 
 * Future Recommendations:
 * 1. Add Subscriber teirs for more granular rewards
 * */
using RimWorld;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using ToolkitCore;
using TwitchToolkit.Store;
using TwitchToolkit.Utilities;
using TwitchToolkitDev;
using Verse;

namespace TwitchToolkit;

public static class Viewers
{
    public static string jsonallviewers;
    public static List<Viewer> All = new List<Viewer>();
    private static readonly object viewerLock = new object();

    public static void AwardViewersCoins(int setamount = 0)
    {
        List<string> usernames = ParseViewersFromJsonAndFindActiveViewers();
        if (usernames == null)
        {
            ToolkitLogger.Warning("AwardViewersCoins: No active viewers found to award coins");
            return;
        }

        foreach (string username in usernames)
        {
            try
            {
                Viewer viewer = GetViewer(username);
                if (viewer == null)
                {
                    ToolkitLogger.Warning($"AwardViewersCoins: Could not find viewer {username}");
                    continue;
                }

                if (viewer.IsBanned)
                {
                    ToolkitLogger.Debug($"AwardViewersCoins: Skipping banned viewer {username}");
                    continue;
                }

                if (setamount > 0)
                {
                    viewer.GiveViewerCoins(setamount);
                    continue;
                }

                int baseCoins = ToolkitSettings.CoinAmount;
                float baseMultiplier = (float)viewer.GetViewerKarma() / 100f;

                if (viewer.IsSub)
                {
                    baseCoins += ToolkitSettings.SubscriberExtraCoins;
                    baseMultiplier *= ToolkitSettings.SubscriberCoinMultiplier;
                }
                else if (viewer.IsVIP)
                {
                    baseCoins += ToolkitSettings.VIPExtraCoins;
                    baseMultiplier *= ToolkitSettings.VIPCoinMultiplier;
                }
                else if (viewer.mod)
                {
                    baseCoins += ToolkitSettings.ModExtraCoins;
                    baseMultiplier *= ToolkitSettings.ModCoinMultiplier;
                }

                int minutesSinceViewerWasActive = TimeHelper.MinutesElapsed(viewer.last_seen);
                if (ToolkitSettings.ChatReqsForCoins)
                {
                    if (minutesSinceViewerWasActive > ToolkitSettings.TimeBeforeHalfCoins)
                    {
                        baseMultiplier *= 0.5f;
                    }
                    if (minutesSinceViewerWasActive > ToolkitSettings.TimeBeforeNoCoins)
                    {
                        baseMultiplier *= 0f;
                    }
                }

                double coinsToReward = (double)baseCoins * (double)baseMultiplier;
                Store_Logger.LogString($"{viewer.username} gets {baseCoins} * {baseMultiplier} coins, total {(int)Math.Ceiling(coinsToReward)}");
                viewer.GiveViewerCoins((int)Math.Ceiling(coinsToReward));
            }
            catch (Exception ex)
            {
                ToolkitLogger.Error($"AwardViewersCoins: Error processing viewer {username}: {ex.Message}");
            }
        }
    }

    public static void GiveAllViewersCoins(int amount, List<Viewer> viewers = null)
    {
        try
        {
            lock (viewerLock)
            {
                if (All == null)
                {
                    All = new List<Viewer>();
                }

                if (viewers != null)
                {
                    foreach (Viewer viewer2 in viewers)
                    {
                        viewer2?.GiveViewerCoins(amount);
                    }
                    return;
                }

                List<string> usernames = ParseViewersFromJsonAndFindActiveViewers();
                if (usernames == null)
                {
                    ToolkitLogger.Warning("GiveAllViewersCoins: No active viewers found");
                    return;
                }

                foreach (string username in usernames)
                {
                    Viewer viewer = GetViewer(username);
                    if (viewer != null && viewer.GetViewerKarma() > 1)
                    {
                        viewer.GiveViewerCoins(amount);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"GiveAllViewersCoins: Error: {ex.Message}");
        }
    }

    public static void SetAllViewersCoins(int amount, List<Viewer> viewers = null)
    {
        try
        {
            lock (viewerLock)
            {
                if (All == null)
                {
                    All = new List<Viewer>();
                }

                if (viewers != null)
                {
                    foreach (Viewer viewer in viewers)
                    {
                        viewer?.SetViewerCoins(amount);
                    }
                    return;
                }

                if (All == null)
                {
                    ToolkitLogger.Warning("SetAllViewersCoins: All viewers list is null");
                    return;
                }

                foreach (Viewer item in All)
                {
                    item?.SetViewerCoins(amount);
                }
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"SetAllViewersCoins: Error: {ex.Message}");
        }
    }

    public static void GiveAllViewersKarma(int amount, List<Viewer> viewers = null)
    {
        try
        {
            lock (viewerLock)
            {
                if (viewers != null)
                {
                    foreach (Viewer viewer2 in viewers)
                    {
                        if (viewer2 != null)
                        {
                            viewer2.SetViewerKarma(Math.Min(ToolkitSettings.KarmaCap, viewer2.GetViewerKarma() + amount));
                        }
                    }
                    return;
                }

                List<string> usernames = ParseViewersFromJsonAndFindActiveViewers();
                if (usernames == null)
                {
                    ToolkitLogger.Warning("GiveAllViewersKarma: No active viewers found");
                    return;
                }

                foreach (string username in usernames)
                {
                    Viewer viewer = GetViewer(username);
                    if (viewer != null && viewer.GetViewerKarma() > 1)
                    {
                        viewer.SetViewerKarma(Math.Min(ToolkitSettings.KarmaCap, viewer.GetViewerKarma() + amount));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"GiveAllViewersKarma: Error: {ex.Message}");
        }
    }

    public static void TakeAllViewersKarma(int amount, List<Viewer> viewers = null)
    {
        try
        {   
            lock (viewerLock)
            {
                if (All == null)
                {
                    All = new List<Viewer>();
                }

                if (viewers != null)
                {
                    foreach (Viewer viewer2 in viewers)
                    {
                        if (viewer2 != null)
                        {
                            viewer2.SetViewerKarma(Math.Max(0, viewer2.GetViewerKarma() - amount));
                        }
                    }
                    return;
                }

                if (All == null)
                {
                    ToolkitLogger.Warning("TakeAllViewersKarma: All viewers list is null");
                    return;
                }

                foreach (Viewer viewer in All)
                {
                    viewer?.SetViewerKarma(Math.Max(0, viewer.GetViewerKarma() - amount));
                }
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"TakeAllViewersKarma: Error: {ex.Message}");
        }
    }

    public static void SetAllViewersKarma(int amount, List<Viewer> viewers = null)
    {
        try
        {
            lock (viewerLock)
            {
                if (All == null)
                {
                    All = new List<Viewer>();
                }
            
                if (viewers != null)
                {
                    foreach (Viewer viewer in viewers)
                    {
                        viewer?.SetViewerKarma(amount);
                    }
                    return;
                }

                if (All == null)
                {
                    ToolkitLogger.Warning("SetAllViewersKarma: All viewers list is null");
                    return;
                }

                foreach (Viewer item in All)
                {
                    item?.SetViewerKarma(amount);
                }
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"SetAllViewersKarma: Error: {ex.Message}");
        }
    }

    public static List<string> ParseViewersFromJsonAndFindActiveViewers()
    {
        try
        {
            List<string> usernames = new List<string>();
            string json = jsonallviewers;

            if (GenText.NullOrEmpty(json))
            {
                ToolkitLogger.Warning("ParseViewersFromJson: JSON data is null or empty");
                return null;
            }

            JSONNode parsed = JSON.Parse(json);
            if (parsed == null)
            {
                ToolkitLogger.Error("ParseViewersFromJson: Failed to parse JSON");
                return null;
            }

            List<JSONArray> groups = new List<JSONArray>();
            groups.Add(parsed["chatters"]?["moderators"]?.AsArray);
            groups.Add(parsed["chatters"]?["staff"]?.AsArray);
            groups.Add(parsed["chatters"]?["admins"]?.AsArray);
            groups.Add(parsed["chatters"]?["global_mods"]?.AsArray);
            groups.Add(parsed["chatters"]?["viewers"]?.AsArray);
            groups.Add(parsed["chatters"]?["vips"]?.AsArray);

            foreach (JSONArray group in groups)
            {
                if (group == null) continue;

                JSONNode.Enumerator enumerator2 = group.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    JSONNode username = enumerator2.Current;
                    if (username == null) continue;

                    string usernameconvert = username.ToString();
                    usernameconvert = usernameconvert.Remove(0, 1);
                    usernameconvert = usernameconvert.Remove(usernameconvert.Length - 1, 1);
                    usernames.Add(usernameconvert);
                }
            }

            if (All != null)
            {
                foreach (Viewer viewer in All.Where(delegate (Viewer s)
                {
                    _ = s.last_seen;
                    return TimeHelper.MinutesElapsed(s.last_seen) <= ToolkitSettings.TimeBeforeHalfCoins;
                }))
                {
                    if (!usernames.Contains(viewer.username))
                    {
                        ToolkitLogger.Log("Viewer " + viewer.username + " added to active viewers through chat participation but not in chatter list.");
                        usernames.Add(viewer.username);
                    }
                }
            }

            return usernames;
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"ParseViewersFromJsonAndFindActiveViewers: Error parsing JSON: {ex.Message}");
            return null;
        }
    }

    public static bool SaveUsernamesFromJsonResponse(RequestState request)
    {
        try
        {
            ToolkitLogger.Log("Saving Usernames From Json Response");
            if (request == null || string.IsNullOrEmpty(request.jsonString))
            {
                ToolkitLogger.Warning("SaveUsernamesFromJsonResponse: Invalid request or empty JSON");
                return false;
            }

            jsonallviewers = request.jsonString;

            // Remove duplicates after updating from API
            RemoveDuplicateViewers();

            return true;
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"SaveUsernamesFromJsonResponse: Error: {ex.Message}");
            return false;
        }
    }

    public static void ResetViewers()
    {
        lock (viewerLock)
        {
            All = new List<Viewer>();
        }
        ToolkitLogger.Log("Viewers list has been reset");
    }

    public static Viewer GetViewer(string user)
    {
        try
        {
            if (string.IsNullOrEmpty(user))
            {
                ToolkitLogger.Warning("GetViewer: Username is null or empty");
                return null;
            }

            string usernameLower = user.ToLower();
            Viewer viewer = All?.Find((Viewer x) => x.username == usernameLower);

            if (viewer == null)
            {
                ToolkitLogger.Debug($"Creating new viewer: {usernameLower}");
                viewer = new Viewer(user);
                viewer.SetViewerCoins(ToolkitSettings.StartingBalance);
                viewer.karma = ToolkitSettings.StartingKarma;

                lock (viewerLock)
                {
                    if (All == null)
                    {
                        All = new List<Viewer>();
                    }
                    // Add the new viewer
                    All.Add(viewer);
                }
                // Occasionally check for duplicates (every 10th new viewer)
                if (All.Count % 10 == 0)
                {
                    RemoveDuplicateViewers();
                }
            }

            return viewer;
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"GetViewer: Error getting viewer {user}: {ex.Message}");
            return null;
        }
    }

    public static Viewer GetViewerById(int id)
    {
        try
        {
            if (All == null)
            {
                ToolkitLogger.Warning("GetViewerById: All viewers list is null");
                return null;
            }
            return All?.Find((Viewer s) => s.id == id);
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"GetViewerById: Error finding viewer with ID {id}: {ex.Message}");
            return null;
        }
    }

    public static void RefreshViewers()
    {
        try
        {   
            lock (viewerLock)
            {
                if (All == null)
                {
                    All = new List<Viewer>();
                }
            
                ToolkitLogger.Log("Refreshing Viewers");
                string channel = ToolkitCoreSettings.channel_username?.ToLower();
                if (string.IsNullOrEmpty(channel))
                {
                    ToolkitLogger.Error("RefreshViewers: Channel username is not set");
                    return;
                }
                WebRequest_BeginGetResponse.Main($"https://tmi.twitch.tv/group/user/{channel}/chatters", SaveUsernamesFromJsonResponse);
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"RefreshViewers: Error: {ex.Message}");
        }
    }

    public static void ResetViewersCoins()
    {
        try
        {   
            lock (viewerLock)
            {
                if (All == null)
                {
                    All = new List<Viewer>();
                }
            
                if (All == null)
                {
                    ToolkitLogger.Warning("ResetViewersCoins: All viewers list is null");
                    return;
                }

                foreach (Viewer viewer in All)
                {
                    viewer?.SetViewerCoins(ToolkitSettings.StartingBalance);
                }
                ToolkitLogger.Log("All viewers coins have been reset");
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"ResetViewersCoins: Error: {ex.Message}");
        }
    }

    public static void ResetViewersKarma()
    {
        try
        {
            lock (viewerLock)
            {
                if (All == null)
                {
                    ToolkitLogger.Warning("ResetViewersKarma: All viewers list is null");
                    return;
                }

                foreach (Viewer viewer in All)
                {
                    viewer?.SetViewerKarma(ToolkitSettings.StartingKarma);
                }
                ToolkitLogger.Log("All viewers karma has been reset");
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"ResetViewersKarma: Error: {ex.Message}");
        } 
    }
    
    public static void RemoveDuplicateViewers()
    {
        try
        {
            if (All == null || All.Count == 0) return;

            int duplicatesRemoved = 0;
            int coinsMerged = 0;

            lock (viewerLock)
            {
                var duplicates = All
                    .GroupBy(v => v.username.ToLower())
                    .Where(g => g.Count() > 1)
                    .ToList();

                if (duplicates.Count == 0)
                {
                    ToolkitLogger.Debug("No duplicate viewers found");
                    return;
                }

                foreach (var group in duplicates)
                {
                    // Keep the first viewer and merge coins from duplicates
                    Viewer mainViewer = group.First();
                    var duplicateViewers = group.Skip(1).ToList();

                    foreach (Viewer duplicate in duplicateViewers)
                    {
                        // Merge coins from duplicate viewer
                        int duplicateCoins = duplicate.GetViewerCoins();
                        if (duplicateCoins > 0)
                        {
                            mainViewer.GiveViewerCoins(duplicateCoins);
                            coinsMerged += duplicateCoins;
                        }

                        // Remove the duplicate
                        All.Remove(duplicate);
                        duplicatesRemoved++;
                    }
                }
            }

            if (duplicatesRemoved > 0)
            {
                ToolkitLogger.Log($"Removed {duplicatesRemoved} duplicate viewers and merged {coinsMerged} coins");
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error removing duplicate viewers: {ex.Message}");
        }
    }
}