using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
	public static GameManager instance;
	private void Awake()
	{
		instance = this;
	}

	public GameSettings gameSettings;

	public bool paused = false;
	public List<float> timeMultipliers = new() { 1f };

	[Header("Gameplay Data")]
	public float counter;
	public float pointMultiplier = 1f;
	public float points;
	public int strikes = 3;
	public int binCapacity;
	public int binFilled = 0;

	private void Start()
	{
		FetchData();
	}
	private void Update()
	{
		//reset data
		if (Input.GetKeyDown(KeyCode.R))
		{
			counter = 0;
			points = 0;
			binCapacity = gameSettings.defaultBinCapacity;
			SaveData();
		}

		if (strikes <= 0)
		{
			UIManager.instance.ShowGameOver(true);
		}
		else if (binFilled >= binCapacity)
		{
			UIManager.instance.ShowGameOver(false);
		}

		Time.timeScale = paused ? 0f : timeMultipliers.Aggregate((total, next) => total *= next);
	}

	public void ExitGame()
	{
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}

	public void FetchData()
	{
		var query = SaveManager.instance.saveData.GetData("currency");
		if (query != null) points = (float)query;
		query = SaveManager.instance.saveData.GetData("bin_capacity");
		if (query != null) binCapacity = (int)query;
		else binCapacity = gameSettings.defaultBinCapacity;
	}
	public void SaveData()
	{
		SaveManager.instance.saveData.EditData("currency", points);
		SaveManager.instance.saveData.EditData("bin_capacity", binCapacity);
		SaveManager.instance.SaveData();
	}

	private void OnApplicationQuit()
	{
		points += counter;
		if (points < 0) points = 0;
		counter = 0;
		SaveData();
	}
}