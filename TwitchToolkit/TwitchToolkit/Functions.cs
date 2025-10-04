/*
 * File: Functions.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 14, 2025
 * 
 * Summary of Changes:
 * 1. Added XML documentation comments
 * 2. Added file header for consistency
 * 3. Maintained existing functionality
 * 
 *  * This file is part of TwitchToolkit.
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


namespace TwitchToolkit;

/// <summary>
/// Provides basic functions for MoonSharp Lua scripting integration
/// This class appears to be a starting point for Lua-accessible functions
/// </summary>
public class Functions
{
    /// <summary>
    /// Gets a viewer by username for Lua script access
    /// </summary>
    /// <param name="username">The username to look up</param>
    /// <returns>The Viewer object if found</returns>
    /// <remarks>
    /// This method provides Lua scripts with access to viewer data
    /// </remarks>
    public Viewer GetViewer(string username)
    {
        return Viewers.GetViewer(username);
    }

    /// <summary>
    /// Returns a simple test string for Lua script verification
    /// </summary>
    /// <returns>"Hello World!" string</returns>
    /// <remarks>
    /// This appears to be a test method for verifying Lua integration
    /// </remarks>
    public string ReturnString()
    {
        return "Hello World!";
    }
}
