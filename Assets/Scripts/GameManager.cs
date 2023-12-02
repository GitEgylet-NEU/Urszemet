using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
	public static GameManager instance;
	private void Awake()
	{
		instance = this;
	}

	public GameSettings gameSettings;

	[Header("Gameplay Data")]
	public int counter;
	public int points;
	public int strikes = 3;

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
			UIManager.instance.ShowGameOver();
		}
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
			points = PlayerPrefs.GetInt("currency");
		}
	}
	public void SaveData()
	{
		PlayerPrefs.SetInt("currency", points);
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