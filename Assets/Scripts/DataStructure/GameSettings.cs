using GitEgylet.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Game Settings")]
public class GameSettings : ScriptableObject
{
	[Header("Debris")]
	public GameObject debrisPrefab;
	public DebrisTypeData[] debrisTypeData;
	public float minDistance = .4f;

	[Header("Special Debris")]
	public GameObject goodSDPrefab;
	public GameObject badSDPrefab;
	public float minSDDistance = 5f;

	[Header("Gameplay")]
	public int defaultBinCapacity = 25;

	[Header("Random Events")]
	public MinMaxRange eventSpawnInterval;
	public RandomEventData[] randomEventData;
	public GameObject blackHolePrefab;
	public GameObject convoyPrefab;
	public ModifierSettings modifierSettings;

	[Header("Shop & Items")]
	public ShopItemData[] shopItemData;

	public Sprite GetRandomSprite(Debris.DebrisType debrisType)
	{
		DebrisTypeData data = debrisTypeData.GetData(debrisType);
		if (data == null) return null;
		return data.sprites[Random.Range(0, data.sprites.Length)];
	}
	public bool ShouldSpawnBin(Debris.DebrisType debrisType)
	{
		DebrisTypeData data = debrisTypeData.GetData(debrisType);
		if (data == null) return false;
		return data.shouldSpawnBin;
	}
	public bool ShouldCount(Debris.DebrisType debrisType)
	{
		DebrisTypeData data = debrisTypeData.GetData(debrisType);
		if (data == null) return false;
		return data.shouldCount;
	}

	/// <param name="exclude">IDs of events to exclude</param>
	/// <returns>ID of RandomEventData or null if no events meet the criteria.</returns>
	public string GetRandomEvent(params string[] exclude)
	{
		float sum = randomEventData.Where(x => !exclude.Contains(x.ID)).Select(x => x.spawnChance).Sum();
		float random = Random.Range(0, sum);
		float a = 0;
		foreach (var item in randomEventData)
		{
			if (random > a && random <= a + item.spawnChance) return item.ID;
			else a += item.spawnChance;
		}
		return null;
	}
	public RandomEventData FetchRandomEventData(string ID)
	{
		return randomEventData.Where(x => x.ID == ID).FirstOrDefault();
	}

	public ShopItemData GetShopItemData(string ID)
	{
		return shopItemData.Where(x => x.id == ID).FirstOrDefault();
	}

	[System.Serializable]
	public class DebrisTypeData
	{
		public Debris.DebrisType debrisType;
		public string displayName;
		public Color color;
		public bool shouldSpawnBin = true;
		public bool shouldCount = true;

		public Sprite[] sprites;
	}

	[System.Serializable]
	public class RandomEventData
	{
		public string ID;
		public float spawnChance;
		public MinMaxRange duration;
		public bool doPopUp = true;
		public string name;
		public string description;
		public Sprite icon;
	}
}
public static class DebrisTypeDataExtensions
{
	public static GameSettings.DebrisTypeData GetData(this IEnumerable<GameSettings.DebrisTypeData> data, Debris.DebrisType debrisType)
	{
		var query = data.Where(x => x.debrisType == debrisType).FirstOrDefault();
		return query;
	}
}