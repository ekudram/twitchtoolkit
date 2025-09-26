/*
 * File: TwitchInterfaceBase.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 25, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Removed incorrect constructor that tried to pass Game parameter to base.
 * 2. Added default constructor required for GameComponent.
 * 3. Updated to use TwitchMessageWrapper instead of separate ChatMessage/WhisperMessage methods
 */

using ToolkitCore;
using TwitchLib.Client.Models;
using Verse;

namespace TwitchToolkit
{
    public abstract class TwitchInterfaceBase : ToolkitCore.TwitchInterfaceBase
    {
        // Default constructor required for GameComponent
        public TwitchInterfaceBase() { }

        public TwitchInterfaceBase(Game game) { }

        // NEW: Main method that takes TwitchMessageWrapper
        public abstract void ParseMessage(TwitchMessageWrapper messageWrapper);

        // Override the existing methods to route to the new method
        public override void ParseMessage(ChatMessage chatMessage)
        {
            ParseMessage(new TwitchMessageWrapper(chatMessage));
        }

        public override void ParseWhisper(WhisperMessage whisperMessage)
        {
            ParseMessage(new TwitchMessageWrapper(whisperMessage));
        }
    }
}