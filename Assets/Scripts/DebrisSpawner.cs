using GitEgylet.Utilities;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
	public DebrisData debrisData;

	public MinMaxRange spawnRate;
	float lastSpawn = 0f, nextSpawn = 0f;

	public float spawnAreaDistance;
	Rect spawnArea;
	Vector2 p;

	public MinMaxRange spawnVelocity, spawnVelocityAngle, spawnAngularVelocity;

	void Start()
	{
		Vector2 corner1 = Camera.main.ScreenToWorldPoint(Vector2.zero);
		Vector2 corner2 = Camera.main.ScreenToWorldPoint(new Vector2(0, Camera.main.pixelHeight));

		spawnArea = new Rect(corner1.x - 4f - spawnAreaDistance, corner1.y * .8f, 4f, (corner2.y - corner1.y) * .8f);
	}

	void Update()
	{
		if (lastSpawn >= nextSpawn || Input.GetKeyDown(KeyCode.Space))
		{
			lastSpawn = 0f;
			nextSpawn = spawnRate.Get();
			SpawnDebris(GetRandomPoint());
		}
		lastSpawn += Time.deltaTime;
	}

	void SpawnDebris(Vector2 position)
	{
		Debris.DebrisType type = (Debris.DebrisType)Random.Range(0, System.Enum.GetValues(typeof(Debris.DebrisType)).Length);
		Sprite sprite = debrisData.GetRandomSprite(type);
		int i = 0;
		while (sprite == null && i < 10)
		{
			type = (Debris.DebrisType)Random.Range(0, System.Enum.GetValues(typeof(Debris.DebrisType)).Length);
			sprite = debrisData.GetRandomSprite(type);
			i++;
		}
		if (sprite == null) return;

		GameObject debris = Instantiate(debrisData.debrisPrefab, position, Quaternion.Euler(0, 0, Random.Range(0f, 360f)), transform);
		debris.GetComponent<SpriteRenderer>().sprite = sprite;
		debris.GetComponent<Debris>().type = type;
		Rigidbody2D rb = debris.GetComponent<Rigidbody2D>();
		rb.AddForce(Rotate(Vector2.right, spawnVelocityAngle.Get()) * spawnVelocity.Get(), ForceMode2D.Impulse);
		rb.angularVelocity = spawnAngularVelocity.Get();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(p, .5f);

		Gizmos.color = Color.cyan;
		Gizmos.DrawLineStrip(new System.ReadOnlySpan<Vector3>(new Vector3[] { new Vector2(spawnArea.x, spawnArea.y), new Vector2(spawnArea.x, spawnArea.y + spawnArea.height), new Vector2(spawnArea.x + spawnArea.width, spawnArea.y + spawnArea.height), new Vector2(spawnArea.x + spawnArea.width, spawnArea.y) }), true);
	}

	Vector2 GetRandomPoint()
	{
		float x = spawnArea.x + Random.Range(0, spawnArea.width);
		float y = spawnArea.y + Random.Range(0, spawnArea.height);
		// don't overlap an existing debris
		return new Vector2(x, y);
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
