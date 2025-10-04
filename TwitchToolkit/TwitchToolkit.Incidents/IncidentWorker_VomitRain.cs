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
using TwitchToolkit.GameConditions;
using UnityEngine;
using Verse;

namespace TwitchToolkit.Incidents
{
	public class IncidentWorker_VomitRain : IncidentWorker
	{
		protected override bool CanFireNowSub(IncidentParms parms) => !((Map) parms.target).gameConditionManager.ConditionIsActive(GameConditionDef.Named("VomitRain"));

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map target = (Map) parms.target;
			target.weatherManager.TransitionTo(WeatherDef.Named("VomitRain"));
			int duration = Mathf.RoundToInt(this.def.durationDays.RandomInRange * 60000f);
			GameCondition_VomitRain cond = (GameCondition_VomitRain) GameConditionMaker.MakeCondition(GameConditionDef.Named("VomitRain"), duration);
			target.gameConditionManager.RegisterCondition((GameCondition) cond);
			this.SendStandardLetter(parms, (LookTargets) new TargetInfo(cond.centerLocation.ToIntVec3, target));
			if ((double) target.weatherManager.curWeather.rainRate > 0.10000000149011612)
				target.weatherDecider.StartNextWeather();
			return true;
		}
	}
}

