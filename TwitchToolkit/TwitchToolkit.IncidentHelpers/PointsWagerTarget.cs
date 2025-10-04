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

using RimWorld;

namespace TwitchToolkit.IncidentHelpers;

public class PointsWagerTarget
{
	public float points;

	public IIncidentTarget target;

	public PointsWagerTarget(float points, IIncidentTarget target)
	{
		this.points = points;
		this.target = target;
	}
}
