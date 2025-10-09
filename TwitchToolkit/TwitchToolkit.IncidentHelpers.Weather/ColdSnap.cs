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
using System.Collections.Generic;
using System.Linq;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.IncidentHelpers.Weather
{
	public class ColdSnap : IncidentHelper
	{
		private IncidentParms parms = (IncidentParms) null;
		private IncidentWorker worker = (IncidentWorker) null;

		public override bool IsPossible()
		{
			this.worker = (IncidentWorker) new IncidentWorker_MakeGameCondition();
			this.worker.def = IncidentDef.Named(nameof (ColdSnap));
			this.parms = new IncidentParms();
            // Get all maps and filter for player home maps only
            List<Map> playerHomeMaps = Current.Game.Maps.Where(map => map.IsPlayerHome).ToList();

            // Shuffle the list of player maps to randomize which one is checked first
            playerHomeMaps.Shuffle();
            foreach (IIncidentTarget incidentTarget in playerHomeMaps)
			{
				this.parms.target = incidentTarget;
				if (this.worker.CanFireNow(this.parms))
					return true;
			}
			return false;
		}

		public override void TryExecute() => this.worker.TryExecute(this.parms);
	}
}