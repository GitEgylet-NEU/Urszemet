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
		Electronics = 3,
		Glass = 4,
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
		if (!IsSpecial) rb.excludeLayers = DebrisManager.instance.noCollide ? LayerMask.GetMask("AffectedByNoCollide") : ~0;
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