/*
 * File: Extensions.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 13, 2025
 * 
 * Summary of Changes:
 * 1. Added comprehensive XML documentation
 * 2. Replaced Log/Helper calls with ToolkitLogger
 * 3. Added null checking and validation
 * 4. Improved thread safety and performance
 * 5. Added proper error handling
 * 6. Maintained backward compatibility
 */
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Verse;

namespace TwitchToolkit;

/// <summary>
/// Provides extension methods for various common operations
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Provides thread-safe random number generation
    /// </summary>
    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        /// <summary>
        /// Gets a Random instance specific to the current thread
        /// </summary>
        public static Random ThisThreadsRandom => Local ?? (Local = new Random(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId));
    }

    /// <summary>
    /// Interface for weighted items used in random selection
    /// </summary>
    public interface IWeighted
    {
        /// <summary>Gets or sets the weight value for random selection</summary>
        int Weight { get; set; }
    }

    /// <summary>
    /// Shuffles the elements of a list using Fisher-Yates algorithm
    /// </summary>
    /// <typeparam name="T">The type of elements in the list</typeparam>
    /// <param name="list">The list to shuffle</param>
    /// <exception cref="ArgumentNullException">Thrown if list is null</exception>
    public static void Shuffle<T>(this IList<T> list)
    {
        if (list == null)
        {
            ToolkitLogger.Error("Cannot shuffle null list");
            throw new ArgumentNullException(nameof(list));
        }

        int j = list.Count;
        while (j > 1)
        {
            j--;
            int i = ThreadSafeRandom.ThisThreadsRandom.Next(j + 1);
            T value = list[i];
            list[i] = list[j];
            list[j] = value;
        }
    }

    /// <summary>
    /// Creates a deep clone of a list of cloneable items
    /// </summary>
    /// <typeparam name="T">The type of elements, must implement ICloneable</typeparam>
    /// <param name="listToClone">The list to clone</param>
    /// <returns>A new list with cloned elements</returns>
    /// <exception cref="ArgumentNullException">Thrown if listToClone is null</exception>
    public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
    {
        if (listToClone == null)
        {
            ToolkitLogger.Error("Cannot clone null list");
            throw new ArgumentNullException(nameof(listToClone));
        }

        return listToClone.Select(item => (T)item.Clone()).ToList();
    }

    /// <summary>
    /// Replaces an element at a specific index in an enumerable
    /// </summary>
    /// <typeparam name="T">The type of elements</typeparam>
    /// <param name="enumerable">The enumerable to modify</param>
    /// <param name="index">The index of the element to replace</param>
    /// <param name="value">The new value</param>
    /// <returns>A new enumerable with the replaced element</returns>
    public static IEnumerable<T> Replace<T>(this IEnumerable<T> enumerable, int index, T value)
    {
        if (enumerable == null)
        {
            ToolkitLogger.Error("Cannot replace element in null enumerable");
            throw new ArgumentNullException(nameof(enumerable));
        }

        return enumerable.Select((x, i) => (index == i) ? value : x);
    }

    /// <summary>
    /// Selects a random element from an enumerable using the provided Random instance
    /// </summary>
    /// <typeparam name="T">The type of elements</typeparam>
    /// <param name="enumerable">The enumerable to select from</param>
    /// <param name="rand">The Random instance to use</param>
    /// <returns>A random element from the enumerable</returns>
    /// <exception cref="ArgumentNullException">Thrown if enumerable or rand is null</exception>
    /// <exception cref="InvalidOperationException">Thrown if enumerable is empty</exception>
    public static T RandomElement<T>(this IEnumerable<T> enumerable, Random rand)
    {
        if (enumerable == null)
        {
            ToolkitLogger.Error("Cannot get random element from null enumerable");
            throw new ArgumentNullException(nameof(enumerable));
        }

        if (rand == null)
        {
            ToolkitLogger.Error("Random instance cannot be null");
            throw new ArgumentNullException(nameof(rand));
        }

        var list = enumerable as IList<T> ?? enumerable.ToList();
        if (list.Count == 0)
        {
            ToolkitLogger.Error("Cannot get random element from empty enumerable");
            throw new InvalidOperationException("Enumerable cannot be empty");
        }

        int index = rand.Next(0, list.Count);
        return list[index];
    }

    /// <summary>
    /// Converts seconds to a human-readable time string
    /// </summary>
    /// <param name="seconds">The number of seconds</param>
    /// <returns>A formatted time string (e.g., "2 hours, 30 minutes")</returns>
    public static string ToReadableTimeString(this float seconds)
    {
        return ((int)seconds).ToReadableTimeString();
    }

    /// <summary>
    /// Converts seconds to a human-readable time string
    /// </summary>
    /// <param name="seconds">The number of seconds</param>
    /// <returns>A formatted time string (e.g., "2 hours, 30 minutes")</returns>
    public static string ToReadableTimeString(this int seconds)
    {
        int days = seconds / 86400;
        seconds %= 86400;
        int hours = seconds / 3600;
        seconds %= 3600;
        int minutes = seconds / 60;
        seconds %= 60;

        string formatted = string.Format("{0}{1}{2}{3}",
            (days > 0) ? $"{days:0} day{(days > 1 ? "s" : "")}, " : "",
            (hours > 0) ? $"{hours:0} hour{(hours > 1 ? "s" : "")}, " : "",
            (minutes > 0) ? $"{minutes:0} minute{(minutes > 1 ? "s" : "")}, " : "",
            (seconds > 0) ? $"{seconds:0} second{(seconds > 1 ? "s" : "")}" : "");

        if (formatted.EndsWith(", ", StringComparison.InvariantCultureIgnoreCase))
        {
            formatted = formatted.Substring(0, formatted.Length - 2);
        }

        if (string.IsNullOrEmpty(formatted))
        {
            formatted = "0 seconds";
        }

        return formatted;
    }

    /// <summary>
    /// Converts RimWorld ticks to a human-readable time string
    /// </summary>
    /// <param name="ticks">The number of RimWorld ticks</param>
    /// <returns>A formatted time string using RimWorld time units</returns>
    public static string ToReadableRimworldTimeString(this float ticks)
    {
        return ((int)ticks).ToReadableRimworldTimeString();
    }

    /// <summary>
    /// Converts RimWorld ticks to a human-readable time string
    /// </summary>
    /// <param name="ticks">The number of RimWorld ticks</param>
    /// <returns>A formatted time string using RimWorld time units</returns>
    public static string ToReadableRimworldTimeString(this int ticks)
    {
        int years = ticks / 3600000;
        ticks %= 3600000;
        int quadrums = ticks / 900000;
        ticks %= 900000;
        int days = ticks / 60000;
        ticks %= 60000;
        int hours = ticks / 2500;
        ticks %= 2500;
        int minutes = ticks / 90;
        ticks %= 90;

        string formatted = string.Format("{0}{1}{2}{3}{4}",
            (years > 0) ? $"{years:0} year{(years > 1 ? "s" : "")}, " : "",
            (quadrums > 0) ? $"{quadrums:0} quadrum{(quadrums > 1 ? "s" : "")}, " : "",
            (days > 0) ? $"{days:0} day{(days > 1 ? "s" : "")}, " : "",
            (hours > 0) ? $"{hours:0} hour{(hours > 1 ? "s" : "")}, " : "",
            (minutes > 0) ? $"{minutes:0} minute{(minutes > 1 ? "s" : "")}, " : "");

        if (formatted.EndsWith(", ", StringComparison.InvariantCultureIgnoreCase))
        {
            formatted = formatted.Substring(0, formatted.Length - 2);
        }

        if (string.IsNullOrEmpty(formatted))
        {
            formatted = "0 minutes";
        }

        return formatted;
    }

    /// <summary>
    /// Truncates a string to the specified maximum length
    /// </summary>
    /// <param name="value">The string to truncate</param>
    /// <param name="maxLength">The maximum length of the resulting string</param>
    /// <param name="dots">Whether to append "..." when truncating</param>
    /// <returns>The truncated string</returns>
    public static string Truncate(this string value, int maxLength, bool dots = false)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (value.Length <= maxLength)
        {
            return value;
        }

        return value.Substring(0, maxLength) + (dots ? "..." : "");
    }

    /// <summary>
    /// Selects a random element from a collection based on weighted probabilities
    /// </summary>
    /// <typeparam name="T">The type of elements</typeparam>
    /// <param name="source">The source collection</param>
    /// <param name="weightSelector">Function to get the weight of each element</param>
    /// <param name="result">The selected element if successful</param>
    /// <returns>True if an element was selected, false otherwise</returns>
    public static bool TryChooseRandomElementByWeight<T>(this IEnumerable<T> source, Func<T, float> weightSelector, out T result)
    {
        result = default(T);

        if (source == null)
        {
            ToolkitLogger.Error("Cannot choose random element from null source");
            return false;
        }

        if (weightSelector == null)
        {
            ToolkitLogger.Error("Weight selector cannot be null");
            return false;
        }

        IList<T> list = source.ToList();
        if (list.Count == 0)
        {
            ToolkitLogger.Debug("List is empty in TryChooseRandomElementByWeight");
            return false;
        }

        float totalWeight = 0f;
        for (int j = 0; j < list.Count; j++)
        {
            float weight = weightSelector(list[j]);
            if (weight < 0f)
            {
                ToolkitLogger.Error($"Negative weight in selector: {weight} from {list[j]}");
                weight = 0f;
            }
            totalWeight += weight;
        }

        if (totalWeight <= 0f)
        {
            ToolkitLogger.Debug("Total weight is zero or negative, selecting first element");
            result = list[0];
            return true;
        }

        float choice = Rand.Range(0f, totalWeight);
        float sum = 0f;
        int iterator = 0;

        foreach (T obj in list)
        {
            float weight2 = weightSelector(list[iterator]);
            for (int i = (int)sum; (float)i < weight2 + sum; i++)
            {
                if ((float)i >= choice)
                {
                    result = obj;
                    return true;
                }
            }
            iterator++;
            sum += weight2;
        }

        // Fallback: select first element
        result = list[0];
        return true;
    }
}

/*** Extensions.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Verse;

namespace TwitchToolkit;

public static class Extensions
{
	public static class ThreadSafeRandom
	{
		[ThreadStatic]
		private static Random Local;

		public static Random ThisThreadsRandom => Local ?? (Local = new Random(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId));
	}

	public interface IWeighted
	{
		int Weight { get; set; }
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int j = list.Count;
		while (j > 1)
		{
			j--;
			int i = ThreadSafeRandom.ThisThreadsRandom.Next(j + 1);
			T value = list[i];
			list[i] = list[j];
			list[j] = value;
		}
	}

	public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
	{
		return listToClone.Select((T item) => (T)item.Clone()).ToList();
	}

	public static IEnumerable<T> Replace<T>(this IEnumerable<T> enumerable, int index, T value)
	{
		return enumerable.Select((T x, int i) => (index == i) ? value : x);
	}

	public static T RandomElement<T>(this IEnumerable<T> enumerable, Random rand)
	{
		int index = rand.Next(0, enumerable.Count());
		return enumerable.ElementAt(index);
	}

	public static string ToReadableTimeString(this float seconds)
	{
		return ((int)seconds).ToReadableTimeString();
	}

	public static string ToReadableTimeString(this int seconds)
	{
		int days = seconds / 86400;
		seconds %= 86400;
		int hours = seconds / 3600;
		seconds %= 3600;
		int minutes = seconds / 60;
		seconds %= 60;
		string formatted = string.Format("{0}{1}{2}{3}", (days > 0) ? string.Format("{0:0} day{1}, ", days, (days > 1) ? "s" : string.Empty) : string.Empty, (hours > 0) ? string.Format("{0:0} hour{1}, ", hours, (hours > 1) ? "s" : string.Empty) : string.Empty, (minutes > 0) ? string.Format("{0:0} minute{1}, ", minutes, (minutes > 1) ? "s" : string.Empty) : string.Empty, (seconds > 0) ? string.Format("{0:0} second{1}", seconds, (seconds > 1) ? "s" : string.Empty) : string.Empty);
		if (formatted.EndsWith(", ", StringComparison.InvariantCultureIgnoreCase))
		{
			formatted = formatted.Substring(0, formatted.Length - 2);
		}
		if (string.IsNullOrEmpty(formatted))
		{
			formatted = "0 seconds";
		}
		return formatted;
	}

	public static string ToReadableRimworldTimeString(this float ticks)
	{
		return ((int)ticks).ToReadableRimworldTimeString();
	}

	public static string ToReadableRimworldTimeString(this int ticks)
	{
		int years = ticks / 3600000;
		ticks %= 3600000;
		int quadrums = ticks / 900000;
		ticks %= 900000;
		int days = ticks / 60000;
		ticks %= 60000;
		int hours = ticks / 2500;
		ticks %= 2500;
		int minutes = ticks / 90;
		ticks %= 90;
		string formatted = string.Format("{0}{1}{2}{3}{4}", (years > 0) ? string.Format("{0:0} year{1}, ", years, (years > 1) ? "s" : string.Empty) : string.Empty, (quadrums > 0) ? string.Format("{0:0} quadrum{1}, ", quadrums, (quadrums > 1) ? "s" : string.Empty) : string.Empty, (days > 0) ? string.Format("{0:0} day{1}, ", days, (days > 1) ? "s" : string.Empty) : string.Empty, (hours > 0) ? string.Format("{0:0} hour{1}, ", hours, (hours > 1) ? "s" : string.Empty) : string.Empty, (minutes > 0) ? string.Format("{0:0} minute{1}, ", minutes, (minutes > 1) ? "s" : string.Empty) : string.Empty);
		if (formatted.EndsWith(", ", StringComparison.InvariantCultureIgnoreCase))
		{
			formatted = formatted.Substring(0, formatted.Length - 2);
		}
		if (string.IsNullOrEmpty(formatted))
		{
			formatted = "0 minutes";
		}
		return formatted;
	}

	public static string Truncate(this string value, int maxLength, bool dots = false)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}
		return (value.Length <= maxLength) ? value : (value.Substring(0, maxLength) + (dots ? "..." : ""));
	}

	public static bool TryChooseRandomElementByWeight<T>(this IEnumerable<T> source, Func<T, float> weightSelector, out T result)
	{
		IList<T> list = source.ToList();
		if (list == null || list.Count() == 0)
		{
			Helper.Log("list is null");
			result = default(T);
			return false;
		}
		float totalWeight = 0f;
		for (int j = 0; j < list.Count(); j++)
		{
			float weight = weightSelector(list[j]);
			if (weight < 0f)
			{
				Log.Error("Negative weight in selector: " + weight + " from " + list[j]);
				weight = 0f;
			}
			totalWeight += weight;
		}
		float choice = Rand.Range(0f, totalWeight);
		float sum = 0f;
		int iterator = 0;
		foreach (T obj in list)
		{
			float weight2 = weightSelector(list[iterator]);
			for (int i = (int)sum; (float)i < weight2 + sum; i++)
			{
				if ((float)i >= choice)
				{
					result = obj;
					return true;
				}
			}
			iterator++;
			sum += weight2;
		}
		result = list.ElementAt(0);
		return true;
	}
}
**/