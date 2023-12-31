using GitEgylet.Utilities;
using System.Linq;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
	public static DebrisSpawner instance;
	private void Awake()
	{
		instance = this;
	}

	public float spawnAreaDistance;
	Rect spawnArea;
	Rect spawnAreaSD;
	public Rect playArea {  get; private set; }

	[Header("Spawn Rates")]
	public MinMaxRange spawnRate;
	float lastSpawn = 0f, nextSpawn = 0f;

	public int baseSpawnLimit = 10;
	public int spawnLimitChange = 3;
	public float spawnLimitChangeInterval = 15f;
	int spawnLimit;

	[Header("Debris Properties")]
	public MinMaxRange spawnVelocity;
	public MinMaxRange spawnVelocityAngle;
	public MinMaxRange spawnAngularVelocity;
	public GameObject debrisPrefab;

	void Start()
	{
		Vector2 corner1 = Camera.main.ScreenToWorldPoint(Vector2.zero);
		Vector2 corner2 = Camera.main.ScreenToWorldPoint(new Vector2(0, Camera.main.pixelHeight));

		spawnArea = new Rect(corner1.x - 4f - spawnAreaDistance, corner1.y * .8f, 4f, (corner2.y - corner1.y) * .8f);

		Vector2 corner3 = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight));
		spawnAreaSD = new Rect(corner1.x - 4f, corner1.y - 4f, corner3.x - corner1.x, (corner3.y - corner1.y) + 8f);
		playArea = new Rect(corner1.x, corner1.y, corner3.x - corner1.x, corner3.y - corner1.y);
	}

	void Update()
	{
		if ((lastSpawn >= nextSpawn && DebrisManager.instance.debrisList.Count < spawnLimit) || Input.GetKeyDown(KeyCode.Space))
		{
			var query = GetRandomPoint();
			if (query.HasValue)
			{
				lastSpawn = 0f;
				nextSpawn = spawnRate.Get();
				SpawnDebris(query.Value);
			}	
		}

		lastSpawn += Time.deltaTime;

		spawnLimit = baseSpawnLimit + spawnLimitChange * Mathf.FloorToInt(Time.time / spawnLimitChangeInterval);
		//Debug.Log($"{DebrisManager.instance.debrisList.Count}/{spawnLimit}");
	}

	void SpawnDebris(Vector2 position)
	{
		Debris.DebrisType type = (Debris.DebrisType)Random.Range(0, System.Enum.GetValues(typeof(Debris.DebrisType)).Length);
		Sprite sprite = GameManager.instance.gameSettings.GetRandomSprite(type);
		int i = 0;
		while (sprite == null && i < 10)
		{
			type = (Debris.DebrisType)Random.Range(0, System.Enum.GetValues(typeof(Debris.DebrisType)).Length);
			sprite = GameManager.instance.gameSettings.GetRandomSprite(type);
			i++;
		}
		if (sprite == null) return;

		GameObject debris = Instantiate(debrisPrefab, position, Quaternion.Euler(0, 0, Random.Range(0f, 360f)), transform);
		debris.name = sprite.name;
		debris.GetComponent<SpriteRenderer>().sprite = sprite;
		debris.GetComponent<Debris>().type = type;
		debris.transform.localPosition = new Vector3(debris.transform.position.x, debris.transform.position.y, 0);
		Rigidbody2D rb = debris.GetComponent<Rigidbody2D>();
		rb.AddForce(Rotate(Vector2.right, spawnVelocityAngle.Get()) * spawnVelocity.Get(), ForceMode2D.Impulse);
		rb.angularVelocity = spawnAngularVelocity.Get();
	}

	public GameObject SpawnObject(GameObject prefab, bool outside)
	{
		Vector2 pos = outside ? GetRandomPointForSD() : GetRandomPoint().Value;
		GameObject obj = Instantiate(prefab, transform);
		obj.transform.position = pos;
		if (obj.TryGetComponent(out Rigidbody2D rb))
		{
			//move to the middle of screen
			rb.AddForce(Vector2.zero - pos, ForceMode2D.Impulse);
		}
		return obj;
	}
	public void SpawnSpecialDebris(bool good)
	{
		GameObject prefab = good ? GameManager.instance.gameSettings.goodSDPrefab : GameManager.instance.gameSettings.badSDPrefab;
		Vector2 pos1 = GetRandomPointForSD();
		Vector2 pos2 = GetRandomPointForSD(pos1);

		GameObject debris1 = Instantiate(prefab, pos1, Quaternion.Euler(0, 0, Random.Range(0f, 360f)), transform);
		Rigidbody2D rb1 = debris1.GetComponent<Rigidbody2D>();
		rb1.AddForce((Vector2.zero - pos1) * (spawnVelocity.Get() / 3f), ForceMode2D.Impulse);
		rb1.angularVelocity = spawnAngularVelocity.Get();
		Debris d1 = debris1.GetComponent<Debris>();
		debris1.transform.localPosition = new Vector3(debris1.transform.position.x, debris1.transform.position.y, 0);
		d1.type = good ? Debris.DebrisType.SpecialGood : Debris.DebrisType.SpecialBad;
		d1.flicked = -1;

		GameObject debris2 = Instantiate(prefab, pos2, Quaternion.Euler(0, 0, Random.Range(0f, 360f)), transform);
		Rigidbody2D rb2 = debris2.GetComponent<Rigidbody2D>();
		rb2.AddForce((Vector2.zero - pos2) * (spawnVelocity.Get() / 3f), ForceMode2D.Impulse);
		rb2.angularVelocity = spawnAngularVelocity.Get();
		Debris d2 = debris2.GetComponent<Debris>();
		debris2.transform.localPosition = new Vector3(debris2.transform.position.x, debris2.transform.position.y, 0);
		d2.type = good ? Debris.DebrisType.SpecialGood : Debris.DebrisType.SpecialBad;
		d2.flicked = -1;
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

	Vector2? GetRandomPoint()
	{
		Vector2 p = new(spawnArea.x + Random.Range(0, spawnArea.width), spawnArea.y + Random.Range(0, spawnArea.height));
		int i = 0;
		while (i < 100 && DebrisManager.instance.debrisList.Any(d => Vector2.Distance(d.transform.position, p) < GameManager.instance.gameSettings.minDistance))
		{
			p = new(spawnArea.x + Random.Range(0, spawnArea.width), spawnArea.y + Random.Range(0, spawnArea.height));
			i++;
		}
		if (DebrisManager.instance.debrisList.Any(d => Vector2.Distance(d.transform.position, p) < GameManager.instance.gameSettings.minDistance)) return null;
		else return p;
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
		while (i < 100 && playArea.Contains(p) && Vector2.Distance(other, p) < GameManager.instance.gameSettings.minSDDistance)
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
