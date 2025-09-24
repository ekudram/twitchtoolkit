/*
 * Project: Twitch Toolkit
 * File: MessageInterface.cs
 * 
 * Usage: Handles incoming Twitch chat messages and executes commands or votes accordingly.
 */

// MessageInterface.cs
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
        ToolkitLogger.Debug("MessageInterface initialized");  // Added debug
    }

    public override void ParseMessage(ChatMessage chatMessage)
    {
        ToolkitLogger.Debug($"Received chat message from {chatMessage.Username}: {chatMessage.Message}");

        TwitchMessageWrapper wrappedMessage = new TwitchMessageWrapper(chatMessage);
        ProcessSpecialMessages(wrappedMessage);

        if (ToolkitCoreSettings.forceWhispers)
        {
            TwitchWrapper.SendChatMessage($"@{chatMessage.Username} Please use whispers for commands");
            ToolkitLogger.Debug("Force whispers enabled - command not processed");
        }
        else
        {
            ToolkitLogger.Debug("Processing command from public chat");
            ProcessCommand(wrappedMessage);
        }
    }

    public override void ParseWhisper(WhisperMessage whisperMessage)
    {
        if (whisperMessage == null)
        {
            ToolkitLogger.Warning("Received null whisper message");
            return;
        }

        ToolkitLogger.Debug($"Received whisper from {whisperMessage.Username}: {whisperMessage.Message}");

        TwitchMessageWrapper wrappedMessage = new TwitchMessageWrapper(whisperMessage);
        ProcessSpecialMessages(wrappedMessage);

        ToolkitLogger.Debug("Processing command from whisper");
        ProcessCommand(wrappedMessage);
    }

    private void ProcessSpecialMessages(TwitchMessageWrapper message)
    {
        string lowerMessage = message.Message.ToLower();
        string lowerUsername = message.Username.ToLower();

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

    private void ProcessCommand(TwitchMessageWrapper message)
    {
        ToolkitLogger.Debug($"Processing command/message: {message.Message} from user: {message.Username}");
        Viewer viewer = Viewers.GetViewer(message.Username);
        if (viewer == null) return;

        if (Helper.ModActive)
        {
            CommandsHandler.CheckCommand(message);
        }

        if (VoteHandler.voteActive && int.TryParse(message.Message, out int voteId))
        {
            VoteHandler.currentVote.RecordVote(viewer.id, voteId - 1);
        }
    }
}