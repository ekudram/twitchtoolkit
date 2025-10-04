/*
 * TwitchToolkit Mod - Community Preservation Fork
 * 
 * Original Source: TwitchToolkit (GNU Affero GPL v3)
 * Original Copyright: 2019 from original repository
 * 
 * MAJOR MODIFICATIONS � 2025 Captolamia:
 * Modifications listed above if any.
 * 
 * This work is licensed under GNU Affero GPL v3
 * This is a community preservation effort to maintain and improve
 * abandoned mod code for the benefit of all users.
 * 
 * See LICENSE file for full terms.
 */
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TwitchToolkit.Store;

public class Item
{
	public int id;

	public int price;

	public string abr;

	public string defname;

	public Item(int price, string abr, string defname, int id = -1)
	{
		if (id < 0)
		{
			this.id = StoreInventory.items.Count();
		}
		else
		{
			this.id = id;
		}
		this.price = price;
		this.abr = abr;
		this.defname = defname;
	}

	public static Item GetItemFromAbr(string abr)
	{
		return StoreInventory.items.Find((Item x) => x.abr == abr);
	}

	public static Item GetItemFromDefName(string defname)
	{
		return StoreInventory.items.Find((Item x) => x.defname == defname);
	}

	public void SetItemPrice(int price)
	{
		this.price = price;
	}

	public int CalculatePrice(int quanity)
	{
		return quanity * price;
	}

	public void PutItemInCargoPod(string quote, int amount, string username)
	{
        ThingDef itemDef = ThingDef.Named("DropPodIncoming");
		Thing itemThing = new Thing();
		ThingDef stuff = null;
		ThingDef itemThingDef = ThingDef.Named(defname);
		if (((BuildableDef)itemThingDef).MadeFromStuff && !GenCollection.TryRandomElementByWeight<ThingDef>(from x in GenStuff.AllowedStuffsFor((BuildableDef)(object)itemThingDef, (TechLevel)0)
			where !PawnWeaponGenerator.IsDerpWeapon(itemThingDef, x)
			select x, ((ThingDef x) => x.stuffProps.commonality), out stuff))
		{
			stuff = GenStuff.RandomStuffByCommonalityFor(itemThingDef, (TechLevel)0);
		}
		itemThing = ThingMaker.MakeThing(itemThingDef, (stuff != null) ? stuff : null);
		QualityCategory q = 0;
		if (QualityUtility.TryGetQuality(itemThing, out q))
		{
			setItemQualityRandom(itemThing);
		}
		IntVec3 vec;
		if (itemThingDef.Minifiable)
		{
			itemThingDef = itemThingDef.minifiedDef;
			MinifiedThing minifiedThing = (MinifiedThing)ThingMaker.MakeThing(itemThingDef, (ThingDef)null);
			minifiedThing.InnerThing = (itemThing);
			((Thing)minifiedThing).stackCount = amount;
			vec = Helper.Rain(itemDef, minifiedThing);
		}
		else
		{
			itemThing.stackCount = amount;
			vec = Helper.Rain(itemDef, itemThing);
		}
		quote = Helper.ReplacePlaceholder(quote, null, null, null, null, null, null, from: username, amount: amount.ToString(), item: abr);
		Helper.CarePackage(quote, LetterDefOf.PositiveEvent, vec);
	}

	public static void setItemQualityRandom(Thing thing)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing erences)
		//IL_0006: Unknown result type (might be due to invalid IL or missing erences)
		//IL_000d: Unknown result type (might be due to invalid IL or missing erences)
		QualityCategory qual = QualityUtility.GenerateQualityTraderItem();
		ThingCompUtility.TryGetComp<CompQuality>(thing).SetQuality(qual, (ArtGenerationContext)0);
	}

	public static void TryMakeAllItems()
	{
		IEnumerable<ThingDef> tradeableitems = from t in DefDatabase<ThingDef>.AllDefs
			where (TradeabilityUtility.TraderCanSell(t.tradeability) || ThingSetMakerUtility.CanGenerate(t)) && (t.building == null || t.Minifiable || ToolkitSettings.MinifiableBuildings)
			select t;
        ToolkitLogger.Log("Found " + tradeableitems.Count() + " items");
		foreach (ThingDef item in tradeableitems)
		{
			string label = string.Join("", ((Def)item).label.Split(' ')).ToLower();
			Item checkforexistingitembydefname = GetItemFromDefName(((Def)item).defName);
			Item checkforexistingitembylabel = GetItemFromAbr(label);
			if (checkforexistingitembydefname != null || checkforexistingitembylabel != null)
			{
				continue;
			}
			try
			{
				if (item.BaseMarketValue > 0f)
				{
					int id = StoreInventory.items.Count();
					StoreInventory.items.Add(new Item(Convert.ToInt32(item.BaseMarketValue * 10f / 6f), label, ((Def)item).defName));
				}
			}
			catch (InvalidCastException e)
			{
                ToolkitLogger.Log("Existing item exception " + e.Message);
			}
		}
	}
}
