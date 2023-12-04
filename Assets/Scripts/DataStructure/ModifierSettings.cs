using GitEgylet.Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Modifier Settings")]
public class ModifierSettings : ScriptableObject
{
	public Modifier[] modifiers;

	public string GetRandomModifier(bool good, bool avoidActive = true)
	{
		if (avoidActive) return modifiers.GetRandom(m => m.good == good && !ModifierManager.instance.activeModifiers.ContainsKey(m.id)).id;
		else return modifiers.GetRandom(m => m.good == good).id;
	}
}

[System.Serializable]
public class Modifier
{
	[Tooltip("Must be unique!")] public string id;
	public bool good;
	public string name;
	public string description;
	public Sprite icon;
	public float duration;
	//TODO: implement rarity
	[Range(0f, 1f)] public float rarity; //0 is never

	public Color GetColor() => good ? Color.blue : Color.red;
}