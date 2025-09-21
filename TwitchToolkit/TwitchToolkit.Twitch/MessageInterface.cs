/*
 * Project: Twitch Toolkit
 * File: MessageInterface.cs
 * 
 * Usage: Handles incoming Twitch chat messages and executes commands or votes accordingly.
 */

using RimWorld;
using ToolkitCore;
using TwitchLib.Client.Models;
using TwitchToolkit.Utilities;
using TwitchToolkit.Votes;
using Verse;

namespace TwitchToolkit.Twitch;

public class MessageInterface : TwitchInterfaceBase
{
    public MessageInterface(Game game)
    {
    }

    public override void ParseMessage(ChatMessage ChatMessage)
    {
        if (ChatMessage.Message.ToLower() == "!easteregg")
        {
            switch (ChatMessage.Username.ToLower())
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
        else if (ChatMessage.Message.ToLower() == "!credz")
        {
            switch (ChatMessage.Username.ToLower())
            {
                case "hodlhodl":
                case "saschahi":
                case "sirrandoo":
                case "nry_chan":
                    string text = "If you're reading this letter it means one of the TTK devs has visited your stream.\r\n Keep up the good stream and remember to have fun!";
                    Find.LetterStack.ReceiveLetter((TaggedString)("TTK Dev visited!"), (TaggedString)(text), LetterDefOf.PositiveEvent);
                    break;
            }
        }

        // Handle command processing based on forceWhispers setting
        if (ToolkitCoreSettings.forceWhispers)
        {
            // When forceWhispers is enabled, only process if it's a whisper
            if (ChatMessage is WhisperMessage)
            {
                ProcessCommand(ChatMessage);
            }
        }
        else
        {
            // When forceWhispers is disabled, process all messages
            ProcessCommand(ChatMessage);
        }
    }

    public override void ParseWhisper(WhisperMessage whisperMessage)
    {
        throw new System.NotImplementedException();
    }

    private void ProcessCommand(ChatMessage message)
    {
        if (Helper.ModActive)
        {
            CommandsHandler.CheckCommand(message);
        }

        if (VoteHandler.voteActive && int.TryParse(message.Message, out var voteId))
        {
            VoteHandler.currentVote.RecordVote(Viewers.GetViewer(message.Username).id, voteId - 1);
        }
    }
}