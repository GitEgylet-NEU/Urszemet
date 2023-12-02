using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Settings")]
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
}
public static class DebrisTypeDataExtensions
{
	public static GameSettings.DebrisTypeData GetData(this IEnumerable<GameSettings.DebrisTypeData> data, Debris.DebrisType debrisType)
	{
		var query = data.Where(x => x.debrisType == debrisType).FirstOrDefault();
		return query;
	}
}