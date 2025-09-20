/*
 * File: AvailableCommands.cs 
 * Project: TwitchToolkit
 * 
 * Updated: September 20, 2025
 * 
 * Summary of Changes:
 * 1. Changed from ITwitchMessage to ChatMessage for TwitchLib 3.4.0 compatibility
 * 2. Added error handling with try-catch
 * 3. Added logging for debugging and error tracking
 * 4. Added a check for empty command lists
 * 5. Added message splitting functionality to handle long command lists
 * 6. Used StringBuilder for more efficient string manipulation
 * 7. Added user feedback for errors
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolkitCore;
using TwitchLib.Client.Models;
using Verse;

namespace TwitchToolkit.Commands.ViewerCommands;

public class AvailableCommands : CommandDriver
{
    public override void RunCommand(ChatMessage chatMessage)
    {
        try
        {
            ToolkitLogger.Debug($"AvailableCommands requested by {chatMessage.Username}");

            List<Command> commands = (from s in DefDatabase<Command>.AllDefs
                                      where !s.requiresAdmin && !s.requiresMod && s.enabled
                                      select s).ToList();

            if (commands.Count == 0)
            {
                ToolkitLogger.Warning("No available commands found for viewer listing");
                TwitchWrapper.SendChatMessage($"@{chatMessage.Username} No commands are currently available.");
                return;
            }

            // Build the list of commands
            StringBuilder commandList = new StringBuilder();
            for (int i = 0; i < commands.Count; i++)
            {
                commandList.Append("!" + commands[i].command);
                if (i < commands.Count - 1)
                {
                    commandList.Append(", ");
                }
            }

            // Create the full message
            string fullMessage = $"@{chatMessage.Username} viewer commands: {commandList}";

            // Handle message length limitations
            if (fullMessage.Length > 500)
            {
                // Split into multiple messages if needed
                SendSplitMessages(chatMessage.Username, commandList.ToString());
            }
            else
            {
                TwitchWrapper.SendChatMessage(fullMessage);
            }

            ToolkitLogger.Debug($"Sent available commands list to {chatMessage.Username}");
        }
        catch (Exception ex)
        {
            ToolkitLogger.Error($"Error in AvailableCommands: {ex.Message}");
            TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Error retrieving available commands.");
        }
    }

    private void SendSplitMessages(string username, string commandList)
    {
        // Split the command list into chunks that fit within Twitch's limits
        int maxLength = 450; // Leave room for username and prefix
        string[] commands = commandList.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

        StringBuilder currentMessage = new StringBuilder($"@{username} viewer commands: ");
        List<string> messagesToSend = new List<string>();

        foreach (string command in commands)
        {
            // Check if adding this command would exceed the limit
            if (currentMessage.Length + command.Length + 2 > maxLength) // +2 for ", "
            {
                messagesToSend.Add(currentMessage.ToString());
                currentMessage = new StringBuilder($"@{username} commands continued: ");
            }

            if (currentMessage.Length > $"@{username} commands continued: ".Length)
            {
                currentMessage.Append(", ");
            }

            currentMessage.Append(command);
        }

        // Add the last message if it has content
        if (currentMessage.Length > $"@{username} commands continued: ".Length)
        {
            messagesToSend.Add(currentMessage.ToString());
        }

        // Send all messages
        foreach (string message in messagesToSend)
        {
            TwitchWrapper.SendChatMessage(message);
        }

        ToolkitLogger.Debug($"Split command list into {messagesToSend.Count} messages for {username}");
    }
}