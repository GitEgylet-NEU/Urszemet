using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class DebrisLauncher : MonoBehaviour
{
	public LineRenderer line;

	Collider2D c;
	Rigidbody2D rb;
	private void Start()
	{
		c = GetComponent<Collider2D>();
		rb = GetComponent<Rigidbody2D>();
	}

	Vector2 startPos, newPos;
	bool dragging = false;

	private void Update()
	{
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			Vector2 pos = Camera.main.ScreenToWorldPoint(touch.position);
			Debug.DrawLine(Vector3.zero, pos, Color.red);

			switch (touch.phase)
			{
				case TouchPhase.Began:
					if (c.OverlapPoint(pos))
					{
						startPos = pos;
						newPos = pos;
						dragging = true;
						line.positionCount = 2;
						line.enabled = true;
					}
					break;
				case TouchPhase.Moved or TouchPhase.Stationary:
					if (!dragging) break;
					newPos = pos;
					//Debug.DrawLine(transform.position, (Vector2)transform.position - (newPos - startPos), Color.green);
					line.SetPosition(0, transform.position);
					line.SetPosition(1, (Vector2)transform.position - (newPos - startPos));
					break;
				case TouchPhase.Ended:
					if (!dragging) break;
					rb.AddForce(-(newPos - startPos), ForceMode2D.Impulse);
					startPos = Vector2.zero;
					newPos = Vector2.zero;
					dragging = false;
					line.enabled = false;
					line.positionCount = 0;
					break;
			}
		}
	}

	public void ResetButton()
	{
		startPos = Vector2.zero;
		newPos = Vector2.zero;
		dragging = false;
		rb.velocity = Vector2.zero;
		rb.angularVelocity = 0f;
		transform.SetPositionAndRotation(Vector2.zero - new Vector2(5f, 0f), Quaternion.identity);
	}
}