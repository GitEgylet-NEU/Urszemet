using GitEgylet.Utilities;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Modifier Settings")]
public class ModifierSettings : ScriptableObject
{
	public Modifier[] modifiers;
	public Modifier[] abilities;
	public float abilityCooldown = 10f;

	public string GetRandomModifier(bool good, bool avoidActive = true)
	{
		if (avoidActive) return modifiers.GetRandom(m => m.good == good && !ModifierManager.instance.activeModifiers.ContainsKey(m.id)).id;
		else return modifiers.GetRandom(m => m.good == good).id;
	}

	public Modifier GetModifier(string id)
	{
		return modifiers.Where(x => x.id == id).FirstOrDefault();
	}
	public Modifier GetAbility(string id)
	{
		return abilities.Where(x => x.id == id).FirstOrDefault();
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