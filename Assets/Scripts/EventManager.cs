using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventManager : MonoBehaviour
{
	public static EventManager instance;
	private void Awake()
	{
		instance = this;
	}

	GameSettings settings;
	private void Start()
	{
		//fetch settings
		settings = GameManager.instance.gameSettings;

		nextEventStart = settings.eventSpawnInterval.Get();
	}

	public float lastEventEnd;
	public float nextEventStart;
	public string lastEventID;

	public string currentEventID = null;
	public GameSettings.RandomEventData currentEvent = null;
	public float timeRemaining;
	public bool canEndEvent = true;

	private void Update()
	{
		if (string.IsNullOrWhiteSpace(currentEventID))
		{
			lastEventEnd += Time.deltaTime;
			if (lastEventEnd >= nextEventStart)
			{
				StartEvent();
			}
		}
		else
		{
			timeRemaining -= Time.deltaTime;
			if (timeRemaining <= 0f && canEndEvent)
			{
				//end event
				EndEvent();
				lastEventID = currentEventID;
				currentEvent = null;
				currentEventID = null;
				timeRemaining = 0f;
			}
			else
			{
				UpdateEvent();
			}
		}
	}

	void StartEvent()
	{
		currentEventID = settings.GetRandomEvent(lastEventID);
		currentEvent = settings.FetchRandomEventData(currentEventID);
		lastEventEnd = 0;
		timeRemaining = currentEvent.duration.Get();

		//run logic
		switch (currentEventID)
		{
			case "powerup_good":
				DebrisSpawner.instance.SpawnSpecialDebris(true);
				canEndEvent = false;
				break;
			case "powerup_bad":
				DebrisSpawner.instance.SpawnSpecialDebris(false);
				canEndEvent = false;
				break;
			case "gravity_anomaly":
				var obj = DebrisSpawner.instance.SpawnObject(GameManager.instance.gameSettings.blackHolePrefab, true);
				canEndEvent = false;
				StartCoroutine(EndGravityEvent());
				IEnumerator EndGravityEvent()
				{
					yield return new WaitUntil(() => Vector2.Distance(obj.transform.position, Vector2.zero) / 4f >= DebrisManager.instance.deleteDistance + 0.1f);
					Destroy(obj);
					canEndEvent = true;
				}
				break;
			default:
				canEndEvent = true;
				break;
		}

		if (currentEvent.doPopUp)
		{
			UIManager.instance.AddInfoPanelToQueue(currentEvent.name, currentEvent.description, currentEvent.icon);
		}

		Debug.Log("Started " + currentEvent.ID + $" ({timeRemaining}s)");
	}

	void UpdateEvent()
	{
		switch (currentEventID)
		{
			case "powerup_good":
				canEndEvent = !DebrisManager.instance.debrisList.Any(x => x.type == Debris.DebrisType.SpecialGood);
				break;
			case "powerup_bad":
				canEndEvent = !DebrisManager.instance.debrisList.Any(x => x.type == Debris.DebrisType.SpecialBad);
				break;
			default:
				break;
		}
		//run logic
	}

	void EndEvent()
	{
		if (string.IsNullOrWhiteSpace(currentEventID))
		{
			Debug.LogError("Can't end event because currentEventID is null");
			return;
		}

		//run logic
		nextEventStart = settings.eventSpawnInterval.Get();

		Debug.Log(currentEvent.ID + " has finished");
	}
}