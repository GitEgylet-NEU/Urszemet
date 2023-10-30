using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisManager : MonoBehaviour
{
	internal struct Drag
	{
		public GameObject debris;
		public Vector2 startPos, currentPos;
		public int touch;
		public LineRenderer line;
		public Drag(GameObject debris, Vector2 startPos, Vector2 currentPos, int touch, LineRenderer line)
		{
			this.debris = debris;
			this.startPos = startPos;
			this.currentPos = currentPos;
			this.touch = touch;
			this.line = line;
		}
	}

	public GameObject linePrefab;
	public Dictionary<GameObject, bool> debrisList; //bool -> has been flicked?
	List<Drag> drags; //debrisObj, startPos, currentPos, touchIdx

	private void Awake()
	{
		debrisList = new();
		drags = new();
	}

	private void Start()
	{
		var objs = FindObjectsOfType<DebrisLauncher>();
		foreach (var obj in objs)
		{
			debrisList.Add(obj.gameObject, false);
		}
	}

	private void Update()
	{
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);

			if (touch.phase == TouchPhase.Began)
			{
				Vector2 pos = Camera.main.ScreenToWorldPoint(touch.position);
				foreach (var debris in debrisList)
				{
					if (debris.Value) continue;
					if (debris.Key.TryGetComponent<Collider2D>(out var c))
					{
						if (c.OverlapPoint(pos))
						{
							Debug.Log($"touch {i} overlaps with {debris.Key.name}");
							//start dragging

							LineRenderer line = Instantiate(linePrefab, transform).GetComponent<LineRenderer>();
							line.positionCount = 2;
							line.SetPosition(0, pos);
							line.SetPosition(1, pos);
							line.enabled = true;
							
							Drag drag = new(debris.Key, pos, pos, i, line);
							drags.Add(drag);

							debrisList[debris.Key] = true;
							break;
						}
					}
				}
			}
		}

		//handle drags
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
					drag.line.SetPosition(1, (Vector2)drag.debris.transform.position - (drag.currentPos - drag.startPos));
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
		drag.debris.GetComponent<Rigidbody2D>().AddForce(-(drag.currentPos - drag.startPos), ForceMode2D.Impulse);
		drags.Remove(drag);
	}
}