using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public sealed class BinSwapper : MonoBehaviour
{
	Rect binArea = new();
	[Tooltip("How wide is the bin area compared to the screen?")] public float widthRatio = .1f;

	public GameObject binPrefab;
	public int binCount = 3;
	public List<Bin> bins;

	[Header("Bin Rotation")]
	public Button upButton;
	public Button downButton;
	public bool canRotateUp, canRotateDown;
	bool canRotate;
	int firstBin;
	int nextIdx, prevIdx;
	Debris.DebrisType[] availableBins;
	public float rotateCooldown = 2f;

	void Start()
	{
		availableBins = System.Enum.GetValues(typeof(Debris.DebrisType)) as Debris.DebrisType[];
		availableBins = availableBins.Where(type => GameManager.instance.gameSettings.ShouldSpawnBin(type)).ToArray();

		if (availableBins.Length < binCount) binCount = availableBins.Length;

		Update();
		Setup();
	}

	void Update()
	{
		Vector2 corner1 = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, 0));
		Vector2 corner2 = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight));
		float width = 2f * corner1.x * widthRatio;
		//Debug.Log($"width = {width}");
		binArea.x = corner1.x - width;
		binArea.y = corner1.y;
		binArea.width = width;
		binArea.height = corner2.y - corner1.y;

		upButton.gameObject.SetActive(canRotateUp);
		downButton.gameObject.SetActive(canRotateDown);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLineStrip(new System.ReadOnlySpan<Vector3>(new Vector3[] { new Vector2(binArea.x, binArea.y), new Vector2(binArea.x, binArea.y + binArea.height), new Vector2(binArea.x + binArea.width, binArea.y + binArea.height), new Vector2(binArea.x + binArea.width, binArea.y) }), true);
	}

	void Setup()
	{
		firstBin = 0;
		ShowBins(availableBins.Take(binCount).ToArray());

		canRotate = availableBins.Length > binCount;
		if (canRotate)
		{
			GetNextBins();
		}
		else
		{
			upButton.gameObject.SetActive(false);
		}
	}

	public void RotateDown()
	{
		Debris.DebrisType[] types = new Debris.DebrisType[binCount];
		firstBin--;
		if (firstBin >= availableBins.Length) firstBin -= availableBins.Length;
		else if (firstBin < 0) firstBin += availableBins.Length;
		for (int i = 0; i < binCount; i++)
		{
			int idx = firstBin + i;
			if (idx >= availableBins.Length) idx -= availableBins.Length;
			else if (idx < 0) idx += availableBins.Length;
			types[i] = availableBins[idx];
		}
		ShowBins(types);
		GetNextBins();
		StartCoroutine(RotateCooldown());
	}
	public void RotateUp()
	{
		Debris.DebrisType[] types = new Debris.DebrisType[binCount];
		firstBin++;
		if (firstBin >= availableBins.Length) firstBin -= availableBins.Length;
		else if (firstBin < 0) firstBin += availableBins.Length;
		for (int i = 0; i < binCount; i++)
		{
			int idx = firstBin + i;
			if (idx >= availableBins.Length) idx -= availableBins.Length;
			else if (idx < 0) idx += availableBins.Length;
			types[i] = availableBins[idx];
		}
		ShowBins(types);
		GetNextBins();
		StartCoroutine(RotateCooldown());
	}

	void ShowBins(Debris.DebrisType[] types)
	{
		if (bins != null)
		{
			for (int i = 0; i < bins.Count; i++)
			{
				Destroy(bins[i].gameObject);
			}
		}

		bins = new();
		float binHeight = binArea.height / binCount;
		for (int i = 0; i < binCount; i++)
		{
			var binObj = Instantiate(binPrefab, transform);
			Bin bin = binObj.GetComponent<Bin>();
			bin.type = types[i];

			binObj.transform.position = new Vector2(binArea.x + binArea.width / 2f, binArea.y + binArea.height - (binHeight * i) - binHeight / 2f);
			binObj.transform.localScale = new Vector2(binArea.width, binHeight);
			binObj.name = $"{types[i]}Bin";

			bins.Add(bin);
		}
	}
	void GetNextBins()
	{
		nextIdx = firstBin + binCount;
		if (nextIdx >= availableBins.Length) nextIdx -= availableBins.Length;
		else if (nextIdx < 0) nextIdx += availableBins.Length;
		prevIdx = firstBin - 1;
		if (prevIdx >= availableBins.Length) prevIdx -= availableBins.Length;
		else if (prevIdx < 0) prevIdx += availableBins.Length;
		upButton.transform.GetChild(1).GetComponent<Image>().color = GameManager.instance.gameSettings.debrisTypeData.GetData(availableBins[prevIdx]).color;
		downButton.transform.GetChild(1).GetComponent<Image>().color = GameManager.instance.gameSettings.debrisTypeData.GetData(availableBins[nextIdx]).color;
	}

	IEnumerator RotateCooldown()
	{
		upButton.interactable = false;
		downButton.interactable = false;
		yield return new WaitForSeconds(rotateCooldown);
		upButton.interactable = true;
		downButton.interactable = true;
	}
}