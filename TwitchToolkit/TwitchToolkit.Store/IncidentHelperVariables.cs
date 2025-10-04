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
using TwitchToolkit.Incidents;

namespace TwitchToolkit.Store;

public abstract class IncidentHelperVariables
{
	public StoreIncidentVariables storeIncident = null;

	public string message;

	public abstract Viewer Viewer { get; set; }

	public abstract bool IsPossible(string message, Viewer viewer, bool separateChannel = false);

	public abstract void TryExecute();
}
