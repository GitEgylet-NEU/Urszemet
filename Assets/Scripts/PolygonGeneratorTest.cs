using UnityEngine;

public class PolygonGeneratorTest : MonoBehaviour
{
	public int oldalak;
	public int felosztas;
	public float radius;

	public Mesh[] meshes;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.V)) meshes = PolygonGenerator.GeneratePolygon(oldalak, radius, felosztas);
	}
}