using NohaSoftware.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

public class SaveData
{
	[NonSerialized]
	public Dictionary<string, object> data;

	public SaveData()
	{
		data = new Dictionary<string, object>();
	}
	public SaveData(SerializableTuple<string, object>[] serialized)
	{
		data = serialized.ToDictionary(x => x.Key, x => x.Value);
	}

	public void EditData(string key, object value)
	{
		if (data.ContainsKey(key)) data[key] = value;
		else data.Add(key, value);
	}
	public object GetData(string key)
	{
		if (data.TryGetValue(key, out var obj)) return obj;
		else return null;
	}
	public void RemoveData(string key)
	{
		data.Remove(key);
	}

	public SerializableTuple<string, object>[] Serialize() => data.Select(x => new SerializableTuple<string, object>(x.Key, x.Value)).ToArray();
}