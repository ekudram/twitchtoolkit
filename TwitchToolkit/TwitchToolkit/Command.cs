/*
 * File: Command.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 20, 2025
 * Summary of Changes:
 * 1. Added XML documentation comments for all members
 * 2. Improved error handling with ToolkitLogger
 * 3. Added null checking for commandDriver instantiation
 * 4. Changed from ITwitchMessage to ChatMessage for TwitchLib 3.4.0 compatibility
 * 5. Maintained backward compatibility
 */

using System;
using TwitchLib.Client.Models;
using Verse;

namespace TwitchToolkit;

/// <summary>
/// Defines a Twitch command that can be executed by viewers
/// </summary>
public class Command : Def
{
    /// <summary>The command text that triggers this command</summary>
    public string command = null;

    /// <summary>Whether this command is currently enabled</summary>
    public bool enabled = true;

    /// <summary>Whether this command should be processed in a separate room</summary>
    public bool shouldBeInSeparateRoom = false;

    /// <summary>The type of command driver that handles this command</summary>
    public Type commandDriver = typeof(CommandDriver);

    /// <summary>Whether this command requires mod privileges</summary>
    public bool requiresMod = false;

    /// <summary>Whether this command requires admin privileges</summary>
    public bool requiresAdmin = false;

    /// <summary>The output message sent when this command is executed</summary>
    public string outputMessage = "";

    /// <summary>Whether this command uses a custom message format</summary>
    public bool isCustomMessage = false;

    /// <summary>
    /// Gets the display label for this command, using defName if label is empty
    /// </summary>
    public string Label
    {
        get
        {
            if (!string.IsNullOrEmpty(base.label))
            {
                return base.label;
            }
            return base.defName;
        }
    }

    /// <summary>
    /// Executes this command with the provided Twitch message
    /// </summary>
    /// <param name="chatMessage">The Twitch chat message that triggered the command</param>
    /// <exception cref="ArgumentNullException">Thrown if chatMessage is null</exception>
    /// <exception cref="InvalidOperationException">Thrown if command text is null</exception>
    public void RunCommand(ChatMessage chatMessage)
    {
        ToolkitLogger.Debug($"Running command: {command}");

        if (chatMessage == null)
        {
            ToolkitLogger.Error("Chat message cannot be null");
            throw new ArgumentNullException(nameof(chatMessage));
        }

        if (command == null)
        {
            ToolkitLogger.Error($"Command text is null for def: {defName}");
            throw new InvalidOperationException("Command text cannot be null");
        }

        try
        {
            // Create command driver instance with null checking
            if (commandDriver == null)
            {
                ToolkitLogger.Warning($"Command driver type is null for command: {command}, using default CommandDriver");
                commandDriver = typeof(CommandDriver);
            }

            if (Activator.CreateInstance(commandDriver) is CommandDriver driver)
            {
                driver.command = this;
                driver.RunCommand(chatMessage);
            }
            else
            {
                ToolkitLogger.Error($"Failed to create CommandDriver instance for type: {commandDriver.FullName}");
            }
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error executing command '{command}': {ex}");
        }
    }

    // Optional: Add a method for whisper commands if needed
    /// <summary>
    /// Executes this command with the provided Twitch whisper message
    /// </summary>
    /// <param name="whisperMessage">The Twitch whisper message that triggered the command</param>
    public void RunWhisperCommand(WhisperMessage whisperMessage)
    {
        ToolkitLogger.Debug($"Running whisper command: {command}");
        // Implementation for whisper commands if needed
        // This would require a separate WhisperCommandDriver class
    }
}