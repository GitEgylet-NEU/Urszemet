using NohaSoftware.Utilities;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
	public static SaveManager instance;
	public GameSettings gameSettings;
	private void Awake()
	{
		instance = this;
	}
	string filePath;

	public SaveData saveData;

	private void OnApplicationQuit()
	{
		SaveData();
	}

	public bool LoadSaveData()
	{
		filePath = Path.Combine(Application.persistentDataPath, gameSettings.savePath) + '.' + gameSettings.saveExtension;
		if (!File.Exists(filePath))
		{
			Debug.LogWarning($"Couldn't load save, starting new savegame");
			saveData = new SaveData();
			return true;
		}

		FileStream file = File.OpenRead(filePath);

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

		filePath = Path.Combine(Application.persistentDataPath, gameSettings.savePath) + '.' + gameSettings.saveExtension;
		FileStream file = File.Open(filePath, FileMode.Create, FileAccess.Write);
		BinaryFormatter bf = new BinaryFormatter();

		try
		{
			bf.Serialize(file, serialized);
		}
		catch (System.Exception e)
		{
			Debug.LogError($"Couldn't save file to {filePath}");
			Debug.LogException(e);
			file.Close();
			return false;
		}
		file.Close();
		return true;
	}
}