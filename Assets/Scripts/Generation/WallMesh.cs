using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WallMesh : MonoBehaviour
{

	Mesh wallMesh;
	List<Vector3> vertices;
	List<Color> colors;
	List<int> triangles;

	MeshCollider meshCollider;

	void Awake()
	{
		GetComponent<MeshFilter>().mesh = wallMesh = new Mesh();
		meshCollider = gameObject.AddComponent<MeshCollider>();
		wallMesh.name = "Wall Mesh";
		vertices = new List<Vector3>();
		colors = new List<Color>();
		triangles = new List<int>();
	}

	public void Triangulate(HexRoom[] rooms)
	{
		wallMesh.Clear();
		vertices.Clear();
		colors.Clear();
		triangles.Clear();
		foreach (var room in rooms)
		{
			if(room != null)
				Triangulate(room);
		}
		wallMesh.vertices = vertices.ToArray();
		wallMesh.colors = colors.ToArray();
		wallMesh.triangles = triangles.ToArray();
		wallMesh.RecalculateNormals();
		meshCollider.sharedMesh = wallMesh;
	}

	void Triangulate(HexRoom room)
	{
		for (int i = 0; i < 6; i++)
		{
			if (!room.walls[i])
			{
				Triangulate(room, i);
			}
		}
	}

	void Triangulate(HexRoom room, int wall)
	{
		Vector3 center = room.transform.localPosition;

		AddTriangle(
			center + HexMetrics.wallCorners[wall * 6],
			center + HexMetrics.wallCorners[wall * 6 + 1],
			center + HexMetrics.wallCorners[wall * 6 + 2]
		);
		AddTriangleColor(room.color);
		AddTriangle(
				center + HexMetrics.wallCorners[wall * 6 + 3],
				center + HexMetrics.wallCorners[wall * 6 + 4],
				center + HexMetrics.wallCorners[wall * 6 + 5]
			);
		AddTriangleColor(room.color);

	}

	void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

	void AddTriangleColor(Color color)
	{
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}
}