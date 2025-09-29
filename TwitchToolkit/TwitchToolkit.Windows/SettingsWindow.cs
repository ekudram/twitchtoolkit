using UnityEngine;
using Verse;

namespace TwitchToolkit.Windows;

public class SettingsWindow : Window
{
	public override Vector2 InitialSize => new Vector2(900f, 700f);

	public Mod Mod { get; set; }

	public SettingsWindow(Mod mod)
	{
		Mod = mod;
		base.doCloseButton = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		Mod.DoSettingsWindowContents(inRect);
	}

	public override void PostClose()
	{
		Mod.WriteSettings();
	}
}
