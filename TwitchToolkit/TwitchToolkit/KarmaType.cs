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