using NohaSoftware.Utilities;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
	public static SaveManager instance;
	private void Awake()
	{
		instance = this;
	}

	[Header("Settings")]
	[Tooltip("Relative to persistentDataPath")] public string filePath;

	public SaveData saveData;

	private void OnApplicationQuit()
	{
		SaveData();
	}

	public bool LoadSaveData()
	{
		string path = Path.Combine(Application.persistentDataPath, filePath);
		if (!File.Exists(path))
		{
			Debug.LogWarning($"Couldn't load save, starting new savegame");
			saveData = new SaveData();
			return true;
		}

		FileStream file = File.OpenRead(path);

		BinaryFormatter bf = new BinaryFormatter();
		//TODO: surrogates

		try
		{
			var serialized = bf.Deserialize(file) as SerializableTuple<string, object>[];
			saveData = new(serialized);
			file.Close();
			return true;
		}
		catch (System.Exception)
		{
			saveData = new();
			file.Close();
			return false;
		}
	}
	public bool SaveData()
	{
		if (saveData == null) return false;

		var serialized = saveData.Serialize();

		string path = Path.Combine(Application.persistentDataPath, filePath);
		FileStream file = File.Open(path, FileMode.Create, FileAccess.Write);
		BinaryFormatter bf = new BinaryFormatter();

		try
		{
			bf.Serialize(file, serialized);
		}
		catch (System.Exception e)
		{
			Debug.LogError($"Couldn't save file to {path}");
			Debug.LogException(e);
			file.Close();
			return false;
		}
		file.Close();
		return true;
	}
}