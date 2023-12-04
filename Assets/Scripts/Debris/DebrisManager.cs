using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebrisManager : MonoBehaviour
{
	public static DebrisManager instance;

	internal struct Drag
	{
		public Debris debris;
		public Vector2 startPos, currentPos;
		public int touch;
		public LineRenderer line;
		public Drag(Debris debris, Vector2 startPos, Vector2 currentPos, int touch, LineRenderer line)
		{
			this.debris = debris;
			this.startPos = startPos;
			this.currentPos = currentPos;
			this.touch = touch;
			this.line = line;
		}
	}

	[Tooltip("Delete debris that are farther from the camera's viewport than this value.")] public float deleteDistance;
	Rect deleteBounds;

	public GameObject linePrefab;
	public List<Debris> debrisList;
	List<Drag> drags;
	public int maxFlicks = 1;

	public bool noCollide = false;

	public bool CanSpawnSD { get; private set; } = true;

	private void Awake()
	{
		instance = this;

		debrisList = new();
		drags = new();
	}

	private void Start()
	{
		Vector2 corner1 = Camera.main.ScreenToWorldPoint(Vector2.zero);
		Vector2 corner2 = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight));
		deleteBounds = new(corner1.x - deleteDistance, corner1.y - deleteDistance, corner2.x - corner1.x + deleteDistance * 2f, corner2.y - corner1.y + deleteDistance * 2f);
	}

	private void Update()
	{
		HandleTouches();

		HandleDrags();

		RemoveClutter();

		CanSpawnSD = !debrisList.Any(d => d.type == Debris.DebrisType.SpecialBad || d.type == Debris.DebrisType.SpecialGood);
	}

	public void ToggleNoCollide(bool enabled)
	{
		noCollide = enabled;
		foreach (Debris d in debrisList)
		{
			if (d.IsSpecial) continue;
			d.GetComponent<Rigidbody2D>().excludeLayers = enabled ? LayerMask.GetMask("AffectedByNoCollide") : ~0;
		}
	}

	void HandleTouches()
	{
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);

			if (touch.phase == TouchPhase.Began)
			{
				Vector2 pos = Camera.main.ScreenToWorldPoint(touch.position);
				foreach (var debris in debrisList)
				{
					if (debris.flicked == -1 || debris.flicked >= maxFlicks) continue;
					Collider2D c = debris.GetComponent<Collider2D>();
					if (c.OverlapPoint(pos))
					{
						LineRenderer line = Instantiate(linePrefab, transform).GetComponent<LineRenderer>();
						line.positionCount = 2;
						line.SetPosition(0, pos);
						line.SetPosition(1, pos);
						line.enabled = true;

						Drag drag = new(debris, pos, pos, i, line);
						drags.Add(drag);

						debris.flicked++;
						break;
					}
				}
			}
		}
	}

	void HandleDrags()
	{
		List<Drag> remove = new();
		for (int i = 0; i < drags.Count; i++)
		{
			var drag = drags[i];
			Touch touch;
			try
			{
				touch = Input.GetTouch(drag.touch);
			}
			catch (Exception)
			{
				Debug.LogError($"Can't retrieve touch {i}. Ending corresponding drag");
				remove.Add(drag);
				continue;
			}

			Vector2 pos = Camera.main.ScreenToWorldPoint(touch.position);
			switch (touch.phase)
			{
				case TouchPhase.Moved or TouchPhase.Stationary:
					drag.currentPos = pos;
					drag.line.SetPosition(0, drag.debris.transform.position);
					drag.line.SetPosition(1, (Vector2)drag.debris.transform.position + (drag.currentPos - drag.startPos));
					break;
				case TouchPhase.Ended:
					remove.Add(drag);
					break;
			}
			drags[i] = drag;
		}
		foreach (var drag in remove) EndDrag(drag);
	}

	void EndDrag(Drag drag)
	{
		Destroy(drag.line);
		drag.debris.GetComponent<Rigidbody2D>().AddForce(drag.currentPos - drag.startPos, ForceMode2D.Impulse);
		drags.Remove(drag);
	}

	void RemoveClutter()
	{
		foreach (Debris debris in debrisList.ToArray())
		{
			if (!deleteBounds.Contains(debris.transform.position))
			{
				debrisList.Remove(debris);
				Destroy(debris.gameObject);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLineStrip(new System.ReadOnlySpan<Vector3>(new Vector3[] { new Vector2(deleteBounds.x, deleteBounds.y), new Vector2(deleteBounds.x, deleteBounds.y + deleteBounds.height), new Vector2(deleteBounds.x + deleteBounds.width, deleteBounds.y + deleteBounds.height), new Vector2(deleteBounds.x + deleteBounds.width, deleteBounds.y) }), true);
	}
}