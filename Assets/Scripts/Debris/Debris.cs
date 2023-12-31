using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Debris : MonoBehaviour
{
	Collider2D c;
	Rigidbody2D rb;

	public int flicked = 0;
	public DebrisType type = DebrisType.Communal;
	public bool IsSpecial => type == DebrisType.SpecialGood || type == DebrisType.SpecialBad;
	
	public enum DebrisType
	{
		Communal = 0,
		Plastic = 1,
		Metal = 2,
		Glass = 4,
		Paper = 5,
		SpecialGood = 10,
		SpecialBad = 11
	}
	private void Awake()
	{
		c = GetComponent<Collider2D>();
		rb = GetComponent<Rigidbody2D>();
	}
	private void Start()
	{
		DebrisManager.instance.debrisList.Add(this);
		if (c.GetType() == typeof(PolygonCollider2D))
		{
			gameObject.AddComponent<PolygonCollider2D>();
			Destroy(c);
			c = GetComponent<PolygonCollider2D>();
		}
		if (!IsSpecial) rb.excludeLayers = DebrisManager.instance.noCollide ? LayerMask.GetMask("AffectedByNoCollide") : LayerMask.GetMask();
		if (TryGetComponent<SpriteRenderer>(out var sr))
		{
			var q = GameManager.instance.gameSettings.debrisTypeData.GetData(type);
			if (q != null) sr.material.SetColor("_Color", q.color);
			else sr.material.SetColor("_Color", new Color(0f,0f,0f,0f));
		}
	}
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (IsSpecial)
		{
			if (collision.gameObject.TryGetComponent(out Debris d) && d.type == type)
			{
				d.GetComponent<Debris>().enabled = false;
				ModifierManager.instance.ActivateRandomModifier(type == DebrisType.SpecialGood);
				Destroy(d.gameObject);
				Destroy(gameObject);
			}
		}
		else if (collision.gameObject.name == "Convoy")
		{
			//only deduct point if debris is visible to player
			if (DebrisSpawner.instance.playArea.Contains(transform.position)) GameManager.instance.counter -= .5f;
			Destroy(gameObject);
		}
	}
	private void OnDestroy()
	{
		DebrisManager.instance.debrisList.Remove(this);
	}

	private void OnDrawGizmosSelected()
	{
		if (rb == null) return;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, (Vector2)transform.position + rb.velocity);
	}
}