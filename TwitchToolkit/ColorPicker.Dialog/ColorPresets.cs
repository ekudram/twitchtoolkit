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
using UnityEngine;

namespace ColorPicker.Dialog;

public class ColorPresets
{
	public int Count => Colors.Length;

	private Color[] Colors { get; set; }

	private int SelectedIndex { get; set; }

	public bool IsModified { get; set; }

	public Color this[int i]
	{
		get
		{
			return Colors[i];
		}
		set
		{
			SetColor(i, value);
		}
	}

	public ColorPresets()
	{
		Colors = (Color[])(object)new Color[6];
		Deselect();
		IsModified = false;
	}

	public void Deselect()
	{
		SelectedIndex = -1;
	}

	public Color GetSelectedColor()
	{
		return Colors[SelectedIndex];
	}

	public bool HasSelected()
	{
		return SelectedIndex != -1;
	}

	public bool IsSelected(int i)
	{
		return SelectedIndex == i;
	}

	public void SetColor(int i, Color c)
	{
		if (!((Color)(Colors[i])).Equals(c))
		{
			Colors[i] = c;
			IsModified = true;
		}
	}

	public void SetSelected(int i)
	{
		SelectedIndex = i;
	}

	internal void SetSelectedColor(Color c)
	{
		Colors[SelectedIndex] = c;
		IsModified = true;
	}
}
