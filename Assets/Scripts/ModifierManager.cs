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

	public Dictionary<string, float> activeModifiers = new();
	public Dictionary<string, bool> hasSeenModifier;

	bool canSpawn = true;
	Coroutine spawnCooldown;

	private void Start()
	{
		hasSeenModifier = new();
		foreach (Modifier m in GameManager.instance.gameSettings.modifierSettings.modifiers)
		{
			hasSeenModifier.Add(m.id, false);
		}

		//HandleModifierStart("no_collision");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.O)) HandleModifierStart("jolly_joker");
		else if (Input.GetKeyDown(KeyCode.L)) HandleModifierEnd("jolly_joker");
	}

	public string ActivateRandomModifier(bool good)
	{
		if (!canSpawn) return string.Empty;

		string id = GameManager.instance.gameSettings.modifierSettings.GetRandomModifier(good);
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
		var query = GameManager.instance.gameSettings.modifierSettings.modifiers.Where(m => m.id == id);
		if (query.Any()) modifier = query.FirstOrDefault();
		if (modifier == null)
		{
			Debug.LogWarning($"Can't find Modifier with ID \'{id}\'");
			return false;
		}

		activeModifiers.Add(id, modifier.duration);
		UIManager.instance.AddModifier(modifier);
		// do logic
		HandleModifierStart(id);
		return true;
	}

	IEnumerator SpawnCooldown()
	{
		canSpawn = false;
		//TODO: expose cooldown time
		yield return new WaitForSeconds(2f);
		canSpawn = true;
	}

	void HandleModifierStart(string id)
	{
		switch (id)
		{
			//good modifiers
			case "two_hits":
				DebrisManager.instance.maxFlicks = 2;
				break;
			case "jolly_joker":
				UIManager.instance.swapper.ToggleJoker(true);
				break;
			case "double_points":
				GameManager.instance.pointMultiplier = 2f;
				break;
			case "no_collision":
				DebrisManager.instance.ToggleNoCollide(true);
				break;
			//bad modifiers
			case "time_speed":
				GameManager.instance.timeMultipliers.Add(2f);
				break;
			case "mono_vision":
				break;
			case "half_points":
				GameManager.instance.pointMultiplier = .5f;
				break;
			default:
				Debug.LogWarning($"Modifier's start function has not been implemented! (ID: {id})");
				break;
		}
	}
	public void HandleModifierEnd(string id)
	{
		switch (id)
		{
			//good modifiers
			case "two_hits":
				DebrisManager.instance.maxFlicks = 1;
				break;
			case "jolly_joker":
				UIManager.instance.swapper.ToggleJoker(false);
				break;
			case "double_points":
				GameManager.instance.pointMultiplier = 1f;
				break;
			case "no_collision":
				DebrisManager.instance.ToggleNoCollide(false);
				break;
			//bad modifiers
			case "time_speed":
				GameManager.instance.timeMultipliers.Remove(2f);
				break;
			case "mono_vision":
				break;
			case "half_points":
				GameManager.instance.pointMultiplier = 1f;
				break;
			default:
				Debug.LogWarning($"Modifier's end function has not been implemented! (ID: {id})");
				break;
		}
	}
}