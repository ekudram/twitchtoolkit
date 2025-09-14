/*
 * File: Toolkit.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 13, 2025
 * 
 * Summary of Changes:
 * 1. Added XML documentation comments
 * 2. Added null checking for Mod property
 * 3. Added thread-safe initialization
 * 4. Maintained backward compatibility with static access
 */

using System;
using TwitchToolkit.Utilities;
using Verse;

namespace TwitchToolkit
{
    /// <summary>
    /// Main entry point for the Twitch Toolkit mod
    /// Provides static access to mod instance and job management
    /// </summary>
    public static class Toolkit
    {
        private static TwitchToolkit _mod;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the mod instance (thread-safe)
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if mod instance is not initialized</exception>
        public static TwitchToolkit Mod
        {
            get
            {
                if (_mod == null)
                {
                    lock (_lock)
                    {
                        if (_mod == null)
                        {
                            throw new InvalidOperationException(
                                "TwitchToolkit mod instance has not been initialized. " +
                                "Ensure the mod is properly loaded and initialized before accessing this property.");
                        }
                    }
                }
                return _mod;
            }
            internal set
            {
                lock (_lock)
                {
                    _mod = value ?? throw new ArgumentNullException(nameof(value));
                }
            }
        }

        /// <summary>
        /// Gets the job manager for scheduling tasks
        /// </summary>
        public static Scheduled JobManager { get; } = new Scheduled();

        /// <summary>
        /// Initializes the mod instance (should be called from main thread)
        /// </summary>
        /// <param name="modInstance">The mod instance to initialize with</param>
        public static void Initialize(TwitchToolkit modInstance)
        {
            if (modInstance == null)
                throw new ArgumentNullException(nameof(modInstance));

            Mod = modInstance;
            Log.Message("[TwitchToolkit] Toolkit initialized successfully");
        }
    }
}

/**

using TwitchToolkit.Utilities;

namespace TwitchToolkit;

public static class Toolkit
{
	public static TwitchToolkit Mod = null;

	public static Scheduled JobManager = new Scheduled();
}
**/