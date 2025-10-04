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
using Verse;

namespace TwitchToolkit.Incidents;

public class IncidentWorker_Quote : IncidentWorker
{
	private readonly string Quote;

	public IncidentWorker_Quote(string quote)
	{
		Quote = quote;
	}

	protected void SendStandardLetter()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0082: Unknown result type (might be due to invalid IL or missing erences)
		if (GenText.NullOrEmpty(base.def.letterLabel) || GenText.NullOrEmpty(base.def.letterText))
		{
			Log.Error("Sending standard incident letter with no label or text.");
		}
		string text = base.def.letterText;
		if (Quote != null)
		{
			text += "\n\n";
			text += Quote;
		}
		Find.LetterStack.ReceiveLetter((TaggedString)(base.def.letterLabel), (TaggedString)(text), base.def.letterDef, (string)null);
	}

	protected void SendStandardLetter(LookTargets lookTargets, Faction relatedFaction = null, params string[] textArgs)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing erences)
		//IL_008f: Unknown result type (might be due to invalid IL or missing erences)
		if (GenText.NullOrEmpty(base.def.letterLabel) || GenText.NullOrEmpty(base.def.letterText))
		{
			Log.Error("Sending standard incident letter with no label or text.");
		}
		string text = GenText.CapitalizeFirst(string.Format(base.def.letterText, textArgs));
		if (Quote != null)
		{
			text += "\n\n";
			text += Quote;
		}
		Find.LetterStack.ReceiveLetter((TaggedString)(base.def.letterLabel), (TaggedString)(text), base.def.letterDef, lookTargets, relatedFaction, (Quest)null, (List<ThingDef>)null, (string)null);
	}
}
