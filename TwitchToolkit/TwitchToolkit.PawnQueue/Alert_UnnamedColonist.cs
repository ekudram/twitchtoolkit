using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace TwitchToolkit.PawnQueue;

[StaticConstructorOnStartup]
public class Alert_UnnamedColonist : Alert
{
	private static readonly Texture2D AlertBGTex = SolidColorMaterials.NewSolidColorTexture(Color.white);

	private static readonly Texture2D AlertBGTexHighlight = TexUI.HighlightTex;

	private static List<GlobalTargetInfo> tmpTargets = new List<GlobalTargetInfo>();

	public Alert_UnnamedColonist()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing erences)
		base.defaultPriority = (AlertPriority)1;
		base.defaultLabel = "Colonists need names";
	}

	public override AlertReport GetReport()
	{
		if (!ToolkitSettings.ViewerNamedColonistQueue)
		{
			return (AlertReport)(false);
		}
		Dictionary<string, Pawn> pawnHistory = Current.Game.GetComponent<GameComponentPawns>().pawnHistory;
		IEnumerable<Pawn> freeColonists = Helper.AnyPlayerMap.mapPawns.FreeColonistsSpawned;
		if (freeColonists.Count() != pawnHistory.Count)
		{
			IEnumerable<Pawn> newPawns = freeColonists.Where((Pawn k) => !pawnHistory.Values.Contains(k));
			if (newPawns.Count() > 0)
			{
				return AlertReport.CulpritsAre(newPawns.Cast<Thing>().ToList());
			}
		}
		return (AlertReport)(false);
	}

	public override TaggedString GetExplanation()
	{
		return (TaggedString)("These colonists need names");
	}

	public override Rect DrawAt(float topY, bool minimized)
	{
        Text.Font =((GameFont)1);
		string label = ((Alert)this).GetLabel();
		float height = Text.CalcHeight(label, 148f);
		Rect rect = new Rect((float)UI.screenWidth - 154f, topY, 154f, height);
		GUI.color =(this.BGColor);
		GUI.DrawTexture(rect, (Texture)(object)AlertBGTex);
		GUI.color =(Color.white);
		GUI.BeginGroup(rect);
		Text.Anchor =((TextAnchor)5);
		Widgets.Label(new Rect(0f, 0f, 148f, height), label);
		GUI.EndGroup();
		if (Mouse.IsOver(rect))
		{
			GUI.DrawTexture(rect, (Texture)(object)AlertBGTexHighlight);
		}
		if (Widgets.ButtonInvisible(rect, false))
		{
			Find.WindowStack.TryRemove(typeof(QueueWindow), true);
			Find.WindowStack.Add((Window)(object)new QueueWindow());
		}
		Text.Anchor =((TextAnchor)0);
		return rect;
	}
}
