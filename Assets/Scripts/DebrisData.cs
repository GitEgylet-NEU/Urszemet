using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Debris Data")]
public class DebrisData : ScriptableObject
{
	public GameObject debrisPrefab;
	public DebrisTypeData[] debrisTypeData;

	public Sprite GetRandomSprite(Debris.DebrisType debrisType)
	{
		DebrisTypeData data = debrisTypeData.GetData(debrisType);
		if (data == null) return null;
		return data.sprites[Random.Range(0, data.sprites.Length)];
	}

	[System.Serializable]
	public class DebrisTypeData
	{
		public Debris.DebrisType debrisType;
		public Sprite[] sprites;
	}
}
public static class DebrisTypeDataExtensions
{
	public static DebrisData.DebrisTypeData GetData(this IEnumerable<DebrisData.DebrisTypeData> data, Debris.DebrisType debrisType)
	{
		var query = data.Where(x => x.debrisType == debrisType).FirstOrDefault();
		return query;
	}
}