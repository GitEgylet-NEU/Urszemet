using GitEgylet.Utilities;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
	public MinMaxRange spawnRate;
	float lastSpawn = 0f, nextSpawn = 0f;
	public MinMaxRange spawnRateSDG, spawnRateSDB;
	float lastSpawnSDG = 0f, nextSpawnSDG = 0f;
	float lastSpawnSDB = 0f, nextSpawnSDB = 0f;

	public float spawnAreaDistance;
	Rect spawnArea;
	Rect spawnAreaSD, playArea;

	public MinMaxRange spawnVelocity, spawnVelocityAngle, spawnAngularVelocity;

	void Start()
	{
		Vector2 corner1 = Camera.main.ScreenToWorldPoint(Vector2.zero);
		Vector2 corner2 = Camera.main.ScreenToWorldPoint(new Vector2(0, Camera.main.pixelHeight));

		spawnArea = new Rect(corner1.x - 4f - spawnAreaDistance, corner1.y * .8f, 4f, (corner2.y - corner1.y) * .8f);

		Vector2 corner3 = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight));
		spawnAreaSD = new Rect(corner1.x - 4f, corner1.y - 4f, corner3.x - corner1.x, (corner3.y - corner1.y) + 8f);
		playArea = new Rect(corner1.x, corner1.y, corner3.x - corner1.x, corner3.y - corner1.y);

		nextSpawnSDG = spawnRateSDG.Get();
		nextSpawnSDB = spawnRateSDB.Get();
	}

	void Update()
	{
		if (lastSpawn >= nextSpawn || Input.GetKeyDown(KeyCode.Space))
		{
			lastSpawn = 0f;
			nextSpawn = spawnRate.Get();
			SpawnDebris(GetRandomPoint());
		}
		if (lastSpawnSDG >= nextSpawnSDG && DebrisManager.instance.CanSpawnSD)
		{
			lastSpawnSDG = 0f;
			nextSpawnSDG = spawnRateSDG.Get();
			SpawnSpecialDebris(true);
		}
		else if (lastSpawnSDB >= nextSpawnSDB && DebrisManager.instance.CanSpawnSD)
		{
			lastSpawnSDB = 0f;
			nextSpawnSDB = spawnRateSDB.Get();
			SpawnSpecialDebris(false);
		}


		lastSpawn += Time.deltaTime;
		lastSpawnSDG += Time.deltaTime;
		lastSpawnSDB += Time.deltaTime;
	}

	void SpawnDebris(Vector2 position)
	{
		Debris.DebrisType type = (Debris.DebrisType)Random.Range(0, System.Enum.GetValues(typeof(Debris.DebrisType)).Length);
		Sprite sprite = GameManager.instance.gameData.GetRandomSprite(type);
		int i = 0;
		while (sprite == null && i < 10)
		{
			type = (Debris.DebrisType)Random.Range(0, System.Enum.GetValues(typeof(Debris.DebrisType)).Length);
			sprite = GameManager.instance.gameData.GetRandomSprite(type);
			i++;
		}
		if (sprite == null) return;

		GameObject debris = Instantiate(GameManager.instance.gameData.debrisPrefab, position, Quaternion.Euler(0, 0, Random.Range(0f, 360f)), transform);
		debris.GetComponent<SpriteRenderer>().sprite = sprite;
		debris.GetComponent<Debris>().type = type;
		Rigidbody2D rb = debris.GetComponent<Rigidbody2D>();
		rb.AddForce(Rotate(Vector2.right, spawnVelocityAngle.Get()) * spawnVelocity.Get(), ForceMode2D.Impulse);
		rb.angularVelocity = spawnAngularVelocity.Get();
	}

	void SpawnSpecialDebris(bool good)
	{
		GameObject prefab = good ? GameManager.instance.gameData.goodSDPrefab : GameManager.instance.gameData.badSDPrefab;
		Vector2 pos1 = GetRandomPointForSD();
		Vector2 pos2 = GetRandomPointForSD(pos1);

		GameObject debris1 = Instantiate(prefab, pos1, Quaternion.Euler(0, 0, Random.Range(0f, 360f)), transform);
		Rigidbody2D rb1 = debris1.GetComponent<Rigidbody2D>();
		rb1.AddForce((Vector2.zero - pos1) * (spawnVelocity.Get() / 3f), ForceMode2D.Impulse);
		rb1.angularVelocity = spawnAngularVelocity.Get();
		Debris d1 = debris1.GetComponent<Debris>();
		d1.type = good ? Debris.DebrisType.SpecialGood : Debris.DebrisType.SpecialBad;
		d1.flicked = true;

		GameObject debris2 = Instantiate(prefab, pos2, Quaternion.Euler(0, 0, Random.Range(0f, 360f)), transform);
		Rigidbody2D rb2 = debris2.GetComponent<Rigidbody2D>();
		rb2.AddForce((Vector2.zero - pos2) * (spawnVelocity.Get() / 3f), ForceMode2D.Impulse);
		rb2.angularVelocity = spawnAngularVelocity.Get();
		Debris d2 = debris2.GetComponent<Debris>();
		d2.type = good ? Debris.DebrisType.SpecialGood : Debris.DebrisType.SpecialBad;
		d2.flicked = true;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawLineStrip(new System.ReadOnlySpan<Vector3>(new Vector3[] { new Vector2(spawnArea.x, spawnArea.y), new Vector2(spawnArea.x, spawnArea.y + spawnArea.height), new Vector2(spawnArea.x + spawnArea.width, spawnArea.y + spawnArea.height), new Vector2(spawnArea.x + spawnArea.width, spawnArea.y) }), true);

		Gizmos.color = Color.magenta;
		Gizmos.DrawLineStrip(new System.ReadOnlySpan<Vector3>(new Vector3[] { new Vector2(spawnAreaSD.x, spawnAreaSD.y), new Vector2(spawnAreaSD.x, spawnAreaSD.y + spawnAreaSD.height), new Vector2(spawnAreaSD.x + spawnAreaSD.width, spawnAreaSD.y + spawnAreaSD.height), new Vector2(spawnAreaSD.x + spawnAreaSD.width, spawnAreaSD.y) }), true);

		Gizmos.color = Color.green;
		Gizmos.DrawLineStrip(new System.ReadOnlySpan<Vector3>(new Vector3[] { new Vector2(playArea.x, playArea.y), new Vector2(playArea.x, playArea.y + playArea.height), new Vector2(playArea.x + playArea.width, playArea.y + playArea.height), new Vector2(playArea.x + playArea.width, playArea.y) }), true);
	}

	Vector2 GetRandomPoint()
	{
		float x = spawnArea.x + Random.Range(0, spawnArea.width);
		float y = spawnArea.y + Random.Range(0, spawnArea.height);
		// TODO: don't overlap an existing debris
		return new Vector2(x, y);
	}
	/// <summary>Get a random <see cref="Vector2"/> for a Special Debris</summary>
	Vector2 GetRandomPointForSD()
	{
		Vector2 p = new(spawnAreaSD.x + Random.Range(0, spawnAreaSD.width), spawnAreaSD.y + Random.Range(0, spawnAreaSD.height));
		int i = 1;
		while (i < 100 && playArea.Contains(p))
		{
			p = new(spawnAreaSD.x + Random.Range(0, spawnAreaSD.width), spawnAreaSD.y + Random.Range(0, spawnAreaSD.height));
			i++;
		}
		return p;
	}
	/// <summary>Get a random <see cref="Vector2"/> for a Special Debris while avoiding another</summary>
	Vector2 GetRandomPointForSD(Vector2 other)
	{
		Vector2 p = new(spawnAreaSD.x + Random.Range(0, spawnAreaSD.width), spawnAreaSD.y + Random.Range(0, spawnAreaSD.height));
		int i = 1;
		while (i < 100 && playArea.Contains(p) && Vector2.Distance(other, p) < GameManager.instance.gameData.minSDDistance)
		{
			p = new(spawnAreaSD.x + Random.Range(0, spawnAreaSD.width), spawnAreaSD.y + Random.Range(0, spawnAreaSD.height));
			i++;
		}
		return p;
	}

	//https://stackoverflow.com/a/71710330
	Vector3 Rotate(Vector3 start, float angle)
	{
		start.Normalize();
		Vector3 axis = Vector3.Cross(start, Vector3.up);
		if (axis == Vector3.zero) axis = Vector3.right;
		return Quaternion.AngleAxis(angle, axis) * start;
	}
}
