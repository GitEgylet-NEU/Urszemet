using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Debris : MonoBehaviour
{
	public bool flicked = false;
	public DebrisType type = DebrisType.Communal;
	
	public enum DebrisType
	{
		None,
		Communal,
		Plastic,
		Metal
	}

	private void Start()
	{
		DebrisManager.instance.debrisList.Add(this);
	}
	private void OnDestroy()
	{
		DebrisManager.instance.debrisList.Remove(this);
	}
}