using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
	public static GameManager instance;
	private void Awake()
	{
		instance = this;
		counter = new int[System.Enum.GetValues(typeof(Debris.DebrisType)).Length];
	}

	public GameData gameData;

	[Header("Gameplay Data")]
	public int[] counter;
}