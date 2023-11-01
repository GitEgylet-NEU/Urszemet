using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Debris : MonoBehaviour
{
	Collider2D c;
	Rigidbody2D rb;

	public bool flicked = false;
	public DebrisType type = DebrisType.Communal;
	
	public enum DebrisType
	{
		Communal,
		Plastic,
		Metal
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