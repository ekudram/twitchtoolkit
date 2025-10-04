/*
 * Project: Twitch Toolkit
 * File: MessageInterface.cs
 * 
 * Usage: Handles incoming Twitch chat messages and executes commands or votes accordingly.
 * Updated to use TwitchMessageWrapper
 */
/*
 * TwitchToolkit Mod - Community Preservation Fork
 * 
 * Original Source: TwitchToolkit (GNU Affero GPL v3)
 * Original Copyright: 2019 hodlhodl from original repository
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

using RimWorld;
using ToolkitCore;
using TwitchToolkit.Utilities;
using TwitchToolkit.Votes;
using Verse;

namespace TwitchToolkit.Twitch;

public class MessageInterface : TwitchInterfaceBase
{
    public MessageInterface(Game game)
    {
        ToolkitLogger.Debug("MessageInterface initialized");  // Added debug
    }

    public override void ParseMessage(TwitchMessageWrapper messageWrapper)
    {
        ToolkitLogger.Debug($"Received chat message from {messageWrapper.Username}: {messageWrapper.Message}");

        ProcessSpecialMessages(messageWrapper);

        if (ToolkitCoreSettings.forceWhispers)
        {
            TwitchWrapper.SendChatMessage($"@{messageWrapper.Username} Please use whispers for commands");
            ToolkitLogger.Debug("Force whispers enabled - command not processed");
        }
        else
        {
            ToolkitLogger.Debug("Processing command from public chat");
            ProcessCommand(messageWrapper);
        }
    }
    private void ProcessSpecialMessages(TwitchMessageWrapper messageWrapper)
    {
        string lowerMessage = messageWrapper.Message.ToLower();
        string lowerUsername = messageWrapper.Username.ToLower();

        if (lowerMessage == "!easteregg")
        {
            switch (lowerUsername)
            {
                case "hodlhodl":
                    EasterEgg.ExecuteHodlEasterEgg();
                    break;
                case "saschahi":
                    EasterEgg.ExecuteSaschahiEasterEgg();
                    break;
                case "sirrandoo":
                    EasterEgg.ExecuteSirRandooEasterEgg();
                    break;
                case "nry_chan":
                    EasterEgg.ExecuteNryEasterEgg();
                    break;
                case "kogayis":
                    EasterEgg.ExecuteYiskahEasterEgg();
                    break;
                case "labrat616":
                    EasterEgg.ExecuteLabratEasterEgg();
                    break;
            }
        }
        else if (lowerMessage == "!credz")
        {
            switch (lowerUsername)
            {
                case "hodlhodl":
                case "saschahi":
                case "sirrandoo":
                case "nry_chan":
                    string text = "If you're reading this letter it means one of the TTK devs has visited your stream.\r\n Keep up the good stream and remember to have fun!";
                    Find.LetterStack.ReceiveLetter("TTK Dev visited!", text, LetterDefOf.PositiveEvent);
                    break;
            }
        }
    }
    private void ProcessCommand(TwitchMessageWrapper messageWrapper)
    {
        ToolkitLogger.Debug($"Processing command/message: {messageWrapper.Message} from user: {messageWrapper.Username}");
        Viewer viewer = Viewers.GetViewer(messageWrapper.Username);
        if (viewer == null) return;

        if (Helper.ModActive)
        {
            CommandsHandler.CheckCommand(messageWrapper);
        }

        if (VoteHandler.voteActive && int.TryParse(messageWrapper.Message, out int voteId))
        {
            VoteHandler.currentVote.RecordVote(viewer.id, voteId - 1);
        }
    }
}