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
			points = 0;
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
		if (PlayerPrefs.HasKey("currency"))
		{
			points = PlayerPrefs.GetFloat("currency");
		}
		if (PlayerPrefs.HasKey("binCapacity"))
		{
			binCapacity = PlayerPrefs.GetInt("binCapacity");
		}
		else binCapacity = gameSettings.defaultBinCapacity;
	}
	public void SaveData()
	{
		PlayerPrefs.SetFloat("currency", points);
		PlayerPrefs.SetInt("binCapacity", binCapacity);
		PlayerPrefs.Save();
	}

	private void OnApplicationQuit()
	{
		points += counter;
		if (points < 0) points = 0;
		counter = 0;
		SaveData();
	}
}