/*
 * File: CommandDriver.cs
 * Project: TwitchToolkit
 * 
 * Updated: [Current Date]
 * 
 * Summary of Changes:
 * 1. Added comprehensive XML documentation
 * 2. Replaced Helper.Log with ToolkitLogger
 * 3. Added null checking and error handling
 * 4. Improved MoonSharp script execution safety
 * 5. Maintained backward compatibility
 */

using System;
using System.Text;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using ToolkitCore;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace TwitchToolkit;

/// <summary>
/// Base class for handling Twitch command execution with MoonSharp Lua scripting support
/// </summary>
public class CommandDriver
{
    public Command command = null;

    /// <summary>
    /// Executes the command with MoonSharp Lua scripting support
    /// </summary>
    /// <param name="twitchMessage">The Twitch message that triggered the command</param>
    /// <exception cref="ArgumentNullException">Thrown if twitchMessage or command is null</exception>
    public virtual void RunCommand(ITwitchMessage twitchMessage)
    {
        if (twitchMessage == null)
        {
            ToolkitLogger.Error("Twitch message cannot be null");
            throw new ArgumentNullException(nameof(twitchMessage));
        }

        if (command == null)
        {
            ToolkitLogger.Error("Command reference is null in CommandDriver");
            throw new InvalidOperationException("Command cannot be null");
        }

        Log.Warning("Reached That"); // Debug line to see if this is reached leave this in for now

        ToolkitLogger.Debug("Filtering command");
        string output = FilterTags(twitchMessage, command.outputMessage);
        ToolkitLogger.Debug("Command filtered");

        try
        {
            // Register MoonSharp types if not already registered
            if (!UserData.IsTypeRegistered<Functions>())
            {
                UserData.RegisterType<Functions>();
                UserData.RegisterType<Viewer>();
                ToolkitLogger.Debug("Registered MoonSharp types");
            }

            ToolkitLogger.Debug("Creating script");
            Script script = new Script();
            script.DebuggerEnabled = true;

            DynValue functions = UserData.Create(new Functions());
            script.Globals.Set("functions", functions);

            ToolkitLogger.Debug($"Parsing Script: {output}");
            DynValue res = script.DoString(output);

            string resultMessage = res.CastToString();
            TwitchWrapper.SendChatMessage(resultMessage);
            Log.Message(resultMessage);
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error executing MoonSharp script: {ex}");
            TwitchWrapper.SendChatMessage("Error processing command. Please try again.");
        }
    }

    /// <summary>
    /// Replaces template tags in the input string with viewer-specific data
    /// </summary>
    /// <param name="twitchMessage">The source Twitch message</param>
    /// <param name="input">The input string with template tags</param>
    /// <returns>The processed string with tags replaced</returns>
    public string FilterTags(ITwitchMessage twitchMessage, string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            ToolkitLogger.Warn("Input string is null or empty in FilterTags");
            return string.Empty;
        }

        ToolkitLogger.Debug("Starting filter");
        Viewer viewer = Viewers.GetViewer(twitchMessage.Username);
        StringBuilder output = new StringBuilder(input);

        // Replace basic template tags
        output.Replace("{username}", viewer?.username ?? "Unknown");
        output.Replace("{balance}", (viewer?.GetViewerCoins() ?? 0).ToString());
        output.Replace("{karma}", (viewer?.GetViewerKarma() ?? 0).ToString());
        output.Replace("{purchaselist}", ToolkitSettings.CustomPricingSheetLink ?? "");
        output.Replace("{coin-reward}", ToolkitSettings.CoinAmount.ToString());
        output.Replace("\n", "");

        ToolkitLogger.Debug("Starting regex pattern matching");
        Regex regex = new Regex("\\[(.*?)\\]");
        MatchCollection matches = regex.Matches(output.ToString());

        foreach (Match match in matches)
        {
            ToolkitLogger.Debug($"Found match: {match.Value}");
            string code = match.Value;
            code = code.Replace("[", "").Replace("]", "");

            try
            {
                string moonSharpResult = MoonSharpString(code);
                output.Replace(match.Value, moonSharpResult);
            }
            catch (Exception ex)
            {
                ToolkitLogger.Error($"Error processing MoonSharp code '{code}': {ex}");
                output.Replace(match.Value, "[Error]");
            }
        }

        return output.ToString();
    }

    /// <summary>
    /// Executes MoonSharp Lua code and returns the string result
    /// Used for processing dynamic content in command responses
    /// Example: [functions.GetRandomNumber()] would execute Lua code and replace with result
    /// </summary>
    /// <param name="function">Lua code to execute (without brackets)</param>
    /// <returns>The string result of the Lua execution</returns>
    public string MoonSharpString(string function)
    {
        if (string.IsNullOrEmpty(function))
        {
            ToolkitLogger.Warn("MoonSharp function string is null or empty");
            return string.Empty;
        }

        try
        {
            DynValue res = Script.RunString(function);
            return res.String;
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"MoonSharp execution failed for '{function}': {ex}");
            return "[Script Error]";
        }
    }

    /// <summary>
    /// Executes MoonSharp Lua code and returns the numeric result
    /// Used for mathematical operations or numeric calculations in commands
    /// Example: [functions.CalculatePrice(100)] would return a numeric value
    /// </summary>
    /// <param name="function">Lua code to execute (without brackets)</param>
    /// <returns>The numeric result of the Lua execution</returns>
    public double MoonSharpDouble(string function)
    {
        if (string.IsNullOrEmpty(function))
        {
            ToolkitLogger.Warn("MoonSharp function string is null or empty");
            return 0.0;
        }

        try
        {
            DynValue res = Script.RunString(function);
            return res.Number;
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"MoonSharp execution failed for '{function}': {ex}");
            return 0.0;
        }
    }

    // Note: TokenizeObjects method appears unused but preserved for potential future use
    private static string TokenizeObjects()
    {
        return "";
    }
}
/**
using System.Text;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using ToolkitCore;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace TwitchToolkit;

public class CommandDriver
{
	public Command command = null;

	public virtual void RunCommand(ITwitchMessage twitchMessage)
	{
		Log.Warning("Reached That"); // Debug line to see if this is reached leave this in for now
        Helper.Log("filtering command");
		string output = FilterTags(twitchMessage, command.outputMessage);
		Helper.Log("command filtered");
		if (!UserData.IsTypeRegistered<Functions>())
		{
			UserData.RegisterType<Functions>();
			UserData.RegisterType<Viewer>();
		}
		Helper.Log("creating script");
		Script script = new Script();
		script.DebuggerEnabled = true;
		DynValue functions = UserData.Create(new Functions());
		script.Globals.Set("functions", functions);
		Helper.Log("Parsing Script " + output);
		DynValue res = script.DoString(output);
		TwitchWrapper.SendChatMessage(res.CastToString());
		Log.Message(res.CastToString());
	}

	public string FilterTags(ITwitchMessage twitchMessage, string input)
	{
		Helper.Log("starting filter");
		Viewer viewer = Viewers.GetViewer(twitchMessage.Username);
		StringBuilder output = new StringBuilder(input);
		output.Replace("{username}", viewer.username);
		output.Replace("{balance}", viewer.GetViewerCoins().ToString());
		output.Replace("{karma}", viewer.GetViewerKarma().ToString());
		output.Replace("{purchaselist}", ToolkitSettings.CustomPricingSheetLink);
		output.Replace("{coin-reward}", ToolkitSettings.CoinAmount.ToString());
		output.Replace("\n", "");
		Helper.Log("starting regex");
		Regex regex = new Regex("\\[(.*?)\\]");
		MatchCollection matches = regex.Matches(output.ToString());
		foreach (Match match in matches)
		{
			Helper.Log("found match " + match.Value);
			string code = match.Value;
			code = code.Replace("[", "");
			code = code.Replace("]", "");
			output.Replace(match.Value, MoonSharpString(code));
		}
		return output.ToString();
	}

	public string MoonSharpString(string function)
	{
		DynValue res = Script.RunString(function);
		return res.String;
	}

	public double MoonSharpDouble(string function)
	{
		DynValue res = Script.RunString(function);
		return res.Number;
	}

	private static string TokenizeObjects()
	{
		return "";
	}
}
**/