/*
 * File: ToolkitInitializer.cs
 * Project: Toolkit
 * 
 * Created: September 16, 2025
 * Updated: September 20, 2025
 * 
 * Summary of Changes:
 * 1. Initial version - mod initializer that runs after all mods are loaded
 * 2. Uses LongEventHandler for proper main thread execution
 * 3. Includes proper error handling and logging
 * 4. Maintains compatibility with ToolkitCore initialization
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
using System;
using Verse;

namespace Toolkit
{
    [StaticConstructorOnStartup]
    public static class ToolkitInitializer
    {
        static ToolkitInitializer()
        {
            // Queue initialization to run on the main thread after loading is complete
            LongEventHandler.QueueLongEvent(
                Initialize,
                null,
                false,
                ExceptionHandler, true
            );
        }

        private static void Initialize()
        {
            try
            {
                Log.Message("[Toolkit] Starting initialization after all mods have loaded...");

                // Your initialization logic here
                // Example: Register handlers, validate dependencies, etc.

                Log.Message("[Toolkit] Initialization complete");
            }
            catch (Exception ex)
            {
                Log.Error($"[Toolkit] Initialization failed: {ex}");
                throw;
            }
        }

        private static void ExceptionHandler(Exception ex)
        {
            Log.Error($"[Toolkit] Initialization encountered an error: {ex}");
        }
    }
}