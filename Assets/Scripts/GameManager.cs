using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	bool loaded = false;

	private void Start()
	{
		StartCoroutine(WaitAndFetchData());
		IEnumerator WaitAndFetchData()
		{
			yield return new WaitForSeconds(.5f);
			FetchData();
			loaded = true;
		}
	}
	private void Update()
	{
		if (!loaded) return;
		//reset data
		//if (Input.GetKeyDown(KeyCode.R))
		//{
		//	counter = 0;
		//	points = 0;
		//	InventoryManager.instance.SetItem("capacity_upgrade", 0);
		//	SaveData();
		//}

		if (strikes <= 0)
		{
			UIManager.instance.ShowGameOver(true);
		}
		else if (binCapacity != 0 && binFilled >= binCapacity)
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
	public void ReturnToMainMenu()
	{
		Debug.Log("omg");
		points += counter;
		if (points < 0) points = 0;
		counter = 0;
		SaveData();

		SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
	}

	public void FetchData()
	{
		var query = SaveManager.instance.saveData.GetData("currency");
		if (query != null) points = (float)query;
		int cap = InventoryManager.instance.GetItem("capacity_upgrade");
		binCapacity = gameSettings.capacityUpgradeLevels[cap];
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