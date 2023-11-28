using GitEgylet.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModifierManager : MonoBehaviour
{
	public static ModifierManager instance;
	private void Awake()
	{
		instance = this;
	}

	public Modifier[] modifiers;
	public Dictionary<string, float> activeModifiers = new();

	bool canSpawn = true;
	Coroutine spawnCooldown;

	public string ActivateRandomModifier(bool good)
	{
		if (!canSpawn) return string.Empty;

		string id = GetRandomModifier(good);
		if (id == null)
		{
			Debug.LogError("Can't find a " + (good ? "good" : "bad") + " Modifier. Make sure the corresponding Modifier pool isn't empty!");
			return string.Empty;
		}

		if (ActivateModifier(id))
		{
			Debug.Log($"Activated {id}");
			if (spawnCooldown != null) StopCoroutine(spawnCooldown);
			spawnCooldown = StartCoroutine(SpawnCooldown());
			return id;
		}
		else
		{
			Debug.LogError($"There was an error activating {id}");
			return string.Empty;
		}
	}

	public bool ActivateModifier(string id)
	{
		Modifier modifier = null;
		var query = modifiers.Where(m => m.id == id);
		if (query.Any()) modifier = query.FirstOrDefault();
		if (modifier == null)
		{
			Debug.LogWarning($"Can't find Modifier with ID \'{id}\'");
			return false;
		}

		activeModifiers.Add(id, modifier.duration);
		UIManager.instance.AddModifier(modifier);
		// do logic
		return true;
	}

	public string GetRandomModifier(bool good, bool avoidActive = true)
	{
		if (avoidActive) return modifiers.GetRandom(m => m.good == good && !activeModifiers.ContainsKey(m.id)).id;
		else return modifiers.GetRandom(m => m.good == good).id;
	}
	IEnumerator SpawnCooldown()
	{
		canSpawn = false;
		//TODO: expose cooldown time
		yield return new WaitForSeconds(2f);
		canSpawn = true;
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
		[Range(0f, 1f)] public float rarity; //0 is never

		public Color GetColor() => good ? Color.blue : Color.red;
	}
}