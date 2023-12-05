using System.Collections.Generic;
using UnityEngine;

public static class PolygonGenerator
{
	public static Mesh[] GeneratePolygon(int n, float r, int divide = 1)
	{
		if (n % divide != 0) return null;

		List<Mesh> meshList = new();

		int offset = n / divide;
		Debug.Log("offset: " + offset);
		for (int i = 0; i < divide; i++)
		{
			Mesh mesh = new Mesh();
			mesh.name = "Szelet" + i;

			List<Vector3> verticesList = new() { Vector3.zero };
			int first = 1 + offset * i;
			//Debug.Log($"first: {first}, last: {first + offset}");
			for (int j = first; j <= first + offset; j++)
			{
				bool end = false;
				if (j > n)
				{
					j = 1;
					end = true;
				}

				//a következõ ciklus elsõ indexét úgy kapjuk meg, hogy az elõzõ ciklus elsõ indexéhez hozzáadunk 'offset'-et
				float x = r * Mathf.Sin(2f * Mathf.PI * (j - 1) / n);
				//Debug.Log($"x = {radius} * Mathf.Sin(2f * Mathf.Pi * {j}) = {radius} * Mathf.Sin({2f * Mathf.PI * (float)j}) = {radius} * {Mathf.Sin(2f * Mathf.PI * (float)j)}");
				float y = r * Mathf.Cos(2f * Mathf.PI * (j - 1) / n);
				//Debug.Log(new Vector2(x, y));
				verticesList.Add(new Vector2(x, y));

				if (end) break;
			}
			var vertices = verticesList.ToArray();
			//Debug.Log(string.Join(", ", vertices));

			List<int> tris = new();
			for (int j = 1; j <= offset; j++)
			{
				tris.Add(0);
				tris.Add(j);
				int next = j + 1;
				if (next > n) next = 1;
				tris.Add(next);
				//Debug.Log($"0,{j},{next}");
			}

			mesh.vertices = vertices;
			mesh.triangles = tris.ToArray();
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			meshList.Add(mesh);
		}
		return meshList.ToArray();
	}
}