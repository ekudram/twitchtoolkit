// TwitchMessageWrapper.cs
using TwitchLib.Client.Models;

namespace TwitchToolkit
{
    public class TwitchMessageWrapper
    {
        public string Username { get; }
        public string DisplayName { get; }
        public string UserId { get; }
        public string BotUsername { get; }
        public string Message { get; }
        public bool IsWhisper { get; }
        public bool IsModerator { get; }
        public bool IsSubscriber { get; }
        public bool IsVip { get; }
        public bool IsBroadcaster { get; }

        // Constructor for ChatMessage
        public TwitchMessageWrapper(ChatMessage chatMessage)
        {
            Username = chatMessage.Username;
            DisplayName = chatMessage.DisplayName;
            UserId = chatMessage.UserId;
            BotUsername = chatMessage.BotUsername;
            Message = chatMessage.Message;
            IsWhisper = false;
            IsModerator = chatMessage.IsModerator;
            IsSubscriber = chatMessage.IsSubscriber;
            IsVip = chatMessage.IsVip;
            IsBroadcaster = chatMessage.IsBroadcaster;
        }

        // Constructor for WhisperMessage
        public TwitchMessageWrapper(WhisperMessage whisperMessage)
        {
            Username = whisperMessage.Username;
            DisplayName = whisperMessage.DisplayName;
            UserId = whisperMessage.UserId;
            BotUsername = whisperMessage.BotUsername;
            Message = whisperMessage.Message;
            IsWhisper = true;
            // Whisper messages don't have these properties, set to false
            IsModerator = false;
            IsSubscriber = false;
            IsVip = false;
            IsBroadcaster = false;
        }

        // Implicit conversion operators for convenience
        public static implicit operator TwitchMessageWrapper(ChatMessage chatMessage) => new TwitchMessageWrapper(chatMessage);
        public static implicit operator TwitchMessageWrapper(WhisperMessage whisperMessage) => new TwitchMessageWrapper(whisperMessage);
    }
}