/*
 * Project: TwitchToolkit
 * File: StoreItemEditor.cs
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

using RimWorld;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TwitchToolkit.Utilities;
using Verse;

namespace TwitchToolkit.Store;

[StaticConstructorOnStartup]
public static class Store_ItemEditor
{
	public static string dataPath;

	static Store_ItemEditor()
	{
		dataPath = SaveHelper.dataPath;
		LoadStoreItemList();
	}

	public static IEnumerable<ThingDef> GetDefaultItems()
	{
		return (from t in DefDatabase<ThingDef>.AllDefs
			where (TradeabilityUtility.TraderCanSell(t.tradeability) || ThingSetMakerUtility.CanGenerate(t)) && (t.building == null || t.Minifiable || ToolkitSettings.MinifiableBuildings) && (t.FirstThingCategory != null || t.race != null) && t.BaseMarketValue > 0f && ((Def)t).defName != "Human"
			select t).OrderBy(delegate(ThingDef t)
		{
			TaggedString labelCap = ((Def)t).LabelCap;
			return ((TaggedString)( labelCap)).RawText;
		});
	}

	public static void ResetItemsToDefault()
	{
		IEnumerable<ThingDef> tradeableItems = GetDefaultItems();
		List<Item> allItems = new List<Item>();
		foreach (ThingDef def in tradeableItems)
		{
			allItems.Add(new Item(Convert.ToInt32(def.BaseMarketValue * 10f / 6f), string.Join("", ((Def)def).label.Split(' ')).ToLower().Replace("\"", ""), ((Def)def).defName));
		}
		StoreInventory.items = allItems;
	}

	public static void FindItemsNotInList()
	{
		IEnumerable<ThingDef> tradeableItems = GetDefaultItems();
		foreach (ThingDef def in tradeableItems)
		{
			Item item2 = StoreInventory.items.Find((Item s) => s.defname == ((Def)def).defName);
			if (item2 == null)
			{
				StoreInventory.items.Add(new Item(Convert.ToInt32(def.BaseMarketValue * 10f / 6f), string.Join("", ((Def)def).label.Split(' ')).ToLower().Replace("\"", ""), ((Def)def).defName));
			}
		}
		if (StoreInventory.items.Find((Item item) => item.defname == "Human") != null)
		{
			StoreInventory.items = StoreInventory.items.Where((Item item) => item.defname != "Human").ToList();
		}
		UpdateStoreItemList();
	}

	public static void UpdateStoreItems(List<ThingDef> allItems, List<int> itemPrices)
	{
		int i;
		for (i = 0; i < allItems.Count; i++)
		{
			Item item = StoreInventory.items.Find((Item s) => s.defname == ((Def)allItems[i]).defName);
			if (item != null)
			{
				item.price = itemPrices[i];
				continue;
			}
			List<Item> items = StoreInventory.items;
			int price = itemPrices[i];
			TaggedString val = ((Def)allItems[i]).LabelCap;
			val = ((TaggedString)(val)).ToLower();
			items.Add(new Item(price, string.Join("", ((TaggedString)(val)).RawText.Split(' ')), ((Def)allItems[i]).defName));
		}
		UpdateStoreItemList();
	}

	public static void UpdateStoreItemList()
	{
		StringBuilder json = new StringBuilder();
		if (!Directory.Exists(dataPath))
		{
			Directory.CreateDirectory(dataPath);
		}
		json.AppendLine("{");
		json.AppendLine("\t\"items\" : [");
		if (StoreInventory.items == null)
		{
			return;
		}
		List<Item> allItems = StoreInventory.items;
		int finalCount = allItems.Count;
		int i;
		for (i = 0; i < finalCount; i++)
		{
			IEnumerable<ThingDef> search = from s in DefDatabase<ThingDef>.AllDefs
				where ((Def)s).defName == allItems[i].defname
				select s;
			if (search == null || search.Count() < 1)
			{
				finalCount--;
				// ToolkitCoreLogger.Debug($"Skipping Item: {allItems[i].defname} i: {i} finalCount: {finalCount}");
				continue;
			}
			ThingDef thing = search.ElementAt(0);
			string category = (TaggedString)((thing.FirstThingCategory != null) ? ((Def)thing.FirstThingCategory).LabelCap : (TaggedString)((string)null));
			if (category == null && thing.race != null)
			{
				category = "Animal";
			}
			json.AppendLine("\t{");
			json.AppendLine("\t\t\"abr\": \"" + allItems[i].abr + "\",");
			json.AppendLine("\t\t\"price\": " + allItems[i].price + ",");
			json.AppendLine("\t\t\"category\": \"" + category + "\",");
			json.AppendLine("\t\t\"defname\": \"" + allItems[i].defname + "\"");
			json.AppendLine("\t}" + ((i < finalCount - 1) ? "," : ""));
		}
		json.AppendLine("\t],");
		json.AppendLine("\t\"total\": " + finalCount);
		json.AppendLine("}");
		using StreamWriter streamWriter = File.CreateText(Path.Combine(dataPath, "StoreItems.json"));
		streamWriter.Write(json.ToString());
	}

	public static void LoadStoreItemList()
	{
		string filePath = Path.Combine(dataPath, "StoreItems.json");
		try
		{
			if (!File.Exists(filePath))
			{
				return;
			}
			using StreamReader streamReader = File.OpenText(filePath);
			string jsonString = streamReader.ReadToEnd();
			JSONNode node = JSON.Parse(jsonString);
            //ToolkitCoreLogger.Log(node.ToString()); commented out to reduce log spam
            if (StoreInventory.items == null)
			{
				StoreInventory.items = new List<Item>();
			}
			int i;
			for (i = 0; i < (int)node["total"]; i++)
			{
				Item item = StoreInventory.items.Find((Item x) => (JSONNode)x.defname == (object)node["items"][i]["defname"]);
				if (item != null)
				{
					item.price = node["items"][i]["price"].AsInt;
				}
				else
				{
					StoreInventory.items.Add(new Item(node["items"][i]["price"].AsInt, node["items"][i]["abr"], node["items"][i]["defname"]));
				}
			}
		}
		catch (UnauthorizedAccessException e)
		{
            ToolkitLogger.Error(e.Message);
		}
		FindItemsNotInList();
	}
}
