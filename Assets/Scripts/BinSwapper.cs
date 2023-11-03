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
	public Button nextBinButton;
	bool canSkip;
	Debris.DebrisType nextBin;
	Debris.DebrisType[] availableBins;
	public float rotateCooldown = 2f;

	void Start()
	{
		availableBins = System.Enum.GetValues(typeof(Debris.DebrisType)) as Debris.DebrisType[];
		availableBins = availableBins.Where(type => GameManager.instance.gameData.ShouldSpawnBin(type)).ToArray();

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
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLineStrip(new System.ReadOnlySpan<Vector3>(new Vector3[] { new Vector2(binArea.x, binArea.y), new Vector2(binArea.x, binArea.y + binArea.height), new Vector2(binArea.x + binArea.width, binArea.y + binArea.height), new Vector2(binArea.x + binArea.width, binArea.y) }), true);
	}

	void Setup()
	{
		List<Debris.DebrisType> types = new();
		for (int i = 0; i < binCount; i++)
		{
			//Debris.DebrisType type = (Debris.DebrisType)Random.Range(0, typeCount);
			//while (typeCount >= binCount && types.Contains(type))
			//{
			//	type = (Debris.DebrisType)Random.Range(0, typeCount);
			//}
			//types.Add(type);
			types.Add(availableBins.GetRandom(type => !types.Contains(type)));
		}
		canSkip = availableBins.Length > binCount;

		ShowBins(types.ToArray());

		if (canSkip)
		{
			GetNextBin();
		}
		else
		{
			nextBinButton.gameObject.SetActive(false);
		}
	}

	public void Rotate()
	{
		Debris.DebrisType[] types = new Debris.DebrisType[binCount];
		types[0] = nextBin;
		for (int i = 1; i < binCount; i++)
		{
			types[i] = bins[i - 1].type;
		}
		ShowBins(types);
		GetNextBin();
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

	void GetNextBin()
	{
		nextBin = availableBins.GetRandom(type => !bins.Any(bin => bin.type == type));
		Debug.Log($"Next bin: {nextBin}");

		nextBinButton.transform.GetChild(1).GetComponent<Image>().color = GameManager.instance.gameData.debrisTypeData.GetData(nextBin).color;
	}

	IEnumerator RotateCooldown()
	{
		nextBinButton.interactable = false;
		yield return new WaitForSeconds(rotateCooldown);
		nextBinButton.interactable = true;
	}
}