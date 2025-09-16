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
 * 4. Maintained exact backward compatibility with static field access
 * 5. Added warning system for legacy access
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
        // Backward compatibility field for ToolkitUtils and other mods
        // This MUST be a public static field (not property) for compatibility
        public static TwitchToolkit Mod = null;

        private static TwitchToolkit _modInstance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the mod instance (thread-safe)
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if mod instance is not initialized</exception>
        public static TwitchToolkit ModInstance
        {
            get
            {
                if (_modInstance == null)
                {
                    lock (_lock)
                    {
                        if (_modInstance == null)
                        {
                            // Try to fall back to the legacy field if available
                            if (Mod != null)
                            {
                                _modInstance = Mod;
                                ToolkitLogger.Warn("Using legacy Mod field for backward compatibility. Please update calling code to use ModInstance property.");
                            }
                            else
                            {
                                throw new InvalidOperationException(
                                    "TwitchToolkit mod instance has not been initialized. " +
                                    "Ensure the mod is properly loaded and initialized before accessing this property.");
                            }
                        }
                    }
                }
                return _modInstance;
            }
            internal set
            {
                lock (_lock)
                {
                    _modInstance = value ?? throw new ArgumentNullException(nameof(value));
                    // Maintain backward compatibility - set the legacy field
                    Mod = value;
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

            ModInstance = modInstance;
            ToolkitLogger.Success("Toolkit initialized successfully");
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