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

using System.Collections.Generic;
using RimWorld;

namespace TwitchToolkit;

public class VoteEvent
{
	public List<IncidentDef> options;

	public StorytellerComp_CustomCategoryMTB storytellerComp_CustomCategoryMTB;

	public StorytellerComp_CustomRandomStoryTeller storytellerComp_CustomStoryTeller;

	public StorytellerComp_CustomOnOffCycle storytellerComp_CustomOnOffCycle;

	public IncidentParms parms;

	public VoteEvent(List<IncidentDef> options, StorytellerComp_CustomRandomStoryTeller storytellerComp_CustomRandomStoryTeller, IncidentParms parms)
	{
		this.options = options;
		storytellerComp_CustomStoryTeller = storytellerComp_CustomRandomStoryTeller;
		this.parms = parms;
		Helper.Log("VoteEvent Created, Random");
	}

	public VoteEvent(List<IncidentDef> options, StorytellerComp_CustomCategoryMTB storytellerComp_CustomCategoryMTB, IncidentParms parms)
	{
		this.options = options;
		this.storytellerComp_CustomCategoryMTB = storytellerComp_CustomCategoryMTB;
		this.parms = parms;
		Helper.Log("VoteEvent Created, MTB");
	}

	public VoteEvent(List<IncidentDef> options, StorytellerComp_CustomOnOffCycle storytellerComp_CustomOnOffCycle, IncidentParms parms)
	{
		this.options = options;
		this.storytellerComp_CustomOnOffCycle = storytellerComp_CustomOnOffCycle;
		this.parms = parms;
		Helper.Log("VoteEvent Created, OFC");
	}
}
