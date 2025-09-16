/*
 * File: Settings_Viewers.cs
 * Project: TwitchToolkit
 * 
 * Created: September 14, 2025
 * 
 */
using System;
using UnityEngine;
using Verse;

namespace TwitchToolkit.Settings
{
    public static class UIExtensions
    {
        /// <summary>
        /// Splits a Rect into two parts with the specified percentage for the left side
        /// </summary>
        public static (Rect left, Rect right) Split(this Rect region, float leftPercent = 0.7f)
        {
            float leftWidth = Mathf.Floor(region.width * leftPercent);
            Rect left = new Rect(region.x, region.y, leftWidth, region.height);
            Rect right = new Rect(region.x + leftWidth + 2f, region.y, region.width - leftWidth - 2f, region.height);

            return (left, right);
        }

        /// <summary>
        /// Gets a rect from the listing and splits it into two parts
        /// </summary>
        public static (Rect left, Rect right) Split(this Listing_Standard listing, float leftPercent = 0.7f)
        {
            Rect rect = listing.GetRect(Text.LineHeight);
            return rect.Split(leftPercent);
        }
    }
}