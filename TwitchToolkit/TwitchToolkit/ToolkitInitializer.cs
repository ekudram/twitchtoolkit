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

using RimWorld;
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