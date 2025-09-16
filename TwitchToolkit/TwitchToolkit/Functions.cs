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

/** * 
 * This file is part of TwitchToolkit.
 * TwitchToolkit is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * TwitchToolkit is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with TwitchToolkit. If not, see <http://www.gnu.org/licenses/>.

namespace TwitchToolkit;

public class Functions
{
	public Viewer GetViewer(string username)
	{
		return Viewers.GetViewer(username);
	}

	public string ReturnString()
	{
		return "Hello World!";
	}
}
 **/