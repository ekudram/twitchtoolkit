/*
 * File: TwitchInterfaceBase.cs
 * Project: ToolkitCore
 * 
 * Updated: October 26, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Removed incorrect constructor that tried to pass Game parameter to base.
 * 2. Added default constructor required for GameComponent.
 * 
 * Why These Changes Were Made:
 * GameComponent in RimWorld does not have a constructor that accepts parameters.
 * The base GameComponent class only has a default constructor, so we must use that.
 */

using ToolkitCore;
using TwitchLib.Client.Models;

namespace TwitchToolkit
{
    public abstract class TwitchToolkitInterfaceBase : ToolkitCore.TwitchInterfaceBase
    {
        // Add the new method that takes TwitchMessageWrapper
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

//using TwitchLib.Client.Models.Interfaces;
//using Verse;

//namespace ToolkitCore
//{
//    public abstract class TwitchInterfaceBase : GameComponent
//    {
//        public abstract void ParseMessage(ITwitchMessage twitchMessage);
//    }
//}
