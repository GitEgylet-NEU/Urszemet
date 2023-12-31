using NohaSoftware.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
	public static InventoryManager instance;

	public Dictionary<string, int> Items { get; private set; }
	public float Currency { get; private set; }

	private void Awake()
	{
		instance = this;

		SaveManager.instance.LoadSaveData();

		var query = SaveManager.instance.saveData.GetData("inventoryItems");
		if (query == null) Items = new Dictionary<string, int>();
		else Items = (query as SerializableTuple<string, int>[]).DeserializeDictionary();

		query = SaveManager.instance.saveData.GetData("currency");
		if (query == null) Currency = 0f;
		else Currency = (float)query;

		//create entries for abilities
		foreach (var ability in SaveManager.instance.gameSettings.modifierSettings.abilities) if (!Items.ContainsKey(ability.id)) Items.Add(ability.id, 0);
	}

	private void OnApplicationQuit()
	{
		SaveChanges();
	}

	public void SaveChanges()
	{
		SaveManager.instance.saveData.EditData("inventoryItems", Items.SerializeDictionary().ToArray());
		SaveManager.instance.saveData.EditData("currency", Currency);
	}

	#region Items
	public void AddItem(string id, int amount)
	{
		if (Items.ContainsKey(id)) Items[id] += amount;
		else Items.Add(id, amount);
		SaveChanges();

	}
	public void SetItem(string id, int amount)
	{
		if (Items.ContainsKey(id)) Items[id] = amount;
		else Items.Add(id, amount);
		SaveChanges();
	}
	public bool SubtractItem(string id, int amount)
	{
		if (!Items.ContainsKey(id)) return false;
		Items[id] -= amount;
		if (Items[id] <= 0) Items.Remove(id);
		SaveChanges();
		return true;
	}
	public void RemoveItem(string id)
	{
		Items.Remove(id);
		SaveChanges();
	}
	public int GetItem(string id)
	{
		if (Items.ContainsKey(id)) return Items[id];
		else return 0;
	}
	#endregion
	#region Currency
	public void AddCurrency(float amount)
	{
		Currency += amount;
		SaveChanges();
	}
	public void SetCurrency(float amount)
	{
		Currency = amount;
		SaveChanges();
	}
	/// <summary>Only subtracts amount if the player has enough currency</summary>
	public bool SubtractCurrency(float amount)
	{
		if (Currency - amount < 0) return false;
		Currency -= amount;
		SaveChanges();
		return true;
	}
	public bool CanAfford(float amount) => Currency >= amount;
	#endregion
}