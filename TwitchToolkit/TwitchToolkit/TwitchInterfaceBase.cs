/*
 * File: TwitchInterfaceBase.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 25, 2023
 *  * 
 * Summary of Changes:
 * 1. Removed incorrect constructor that tried to pass Game parameter to base.
 * 2. Added default constructor required for GameComponent.
 * 3. Updated to use TwitchMessageWrapper instead of separate ChatMessage/WhisperMessage methods
 * 4. Added abstract ParseMessage method for TwitchMessageWrapper
 * 5. Overrode existing ParseMessage and ParseWhisper to route to new method
 * 6. Removed unused usings TooltkitCore and System
 * 
 */
/*
 * TwitchToolkit Mod - Community Preservation Fork
 * 
 * Original Source: TwitchToolkit (GNU Affero GPL v3)
 * Original Copyright: 2019 from original repository
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