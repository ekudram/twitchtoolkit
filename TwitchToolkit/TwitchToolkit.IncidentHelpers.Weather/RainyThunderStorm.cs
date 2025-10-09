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
	public class RainyThunderStorm : IncidentHelper
	{
		private Map target = (Map) null;
		private WeatherDef weather = (WeatherDef) null;

		public override bool IsPossible()
		{
			this.weather = DefDatabase<WeatherDef>.GetNamed("RainyThunderstorm");
            List<Map> playerHomeMaps = Current.Game.Maps.Where(map => map.IsPlayerHome).ToList();

            // Shuffle the list of player maps to randomize which one is checked first
            playerHomeMaps.Shuffle();
            foreach (Map map in playerHomeMaps)
			{
				if (map.weatherManager.curWeather != this.weather)
				{
					this.target = map;
					return true;
				}
			}
			return false;
		}

		public override void TryExecute() => Helper.Weather((string) "TwitchStoriesDescription31".Translate(), this.weather, LetterDefOf.PositiveEvent);
	}
}

