using System;

/*
 * File: KarmaType.cs
 * Project: TwitchToolkit
 * 
 * Updated: [Current Date]
 * 
 * Summary of Changes:
 * 1. Added comprehensive XML documentation
 * 2. Added [Description] attributes for better serialization
 * 3. Ensured proper naming conventions
 */

using System.ComponentModel;
using System.Xml.Serialization;

namespace TwitchToolkit
{
    /// <summary>
    /// Defines the karma types for events and viewer actions
    /// </summary>
    public enum KarmaType
    {
        [Description("Good Karma")]
        [XmlEnum("Good")]
        Good,

        [Description("Neutral Karma")]
        [XmlEnum("Neutral")]
        Neutral,

        [Description("Bad Karma")]
        [XmlEnum("Bad")]
        Bad,

        [Description("Doom Karma")]
        [XmlEnum("Doom")]
        Doom
    }
}