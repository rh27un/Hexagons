using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WallMesh : MonoBehaviour
{

	Mesh wallMesh;
	List<Vector3> vertices;
	List<Color> colors;
	List<int> triangles;

	Mesh doorMesh;
	List<Vector3> doorVertices;
	List<Color> doorColors;
	List<int> doorTriangles;

	Mesh secretMesh;
	List<Vector3> secretVertices;
	List<Color> secretColors;
	List<int> secretTriangles;

	MeshCollider meshCollider;
	MeshCollider doorCollider;
	MeshCollider secretCollider;

	void Awake()
	{
		GetComponent<MeshFilter>().mesh = wallMesh = new Mesh();
		GetComponentsInChildren<MeshFilter>()[1].mesh = doorMesh = new Mesh();
		GetComponentsInChildren<MeshFilter>()[2].mesh = secretMesh = new Mesh();
		meshCollider = gameObject.AddComponent<MeshCollider>();
		doorCollider = transform.GetChild(0).GetComponent<MeshCollider>();
		secretCollider = transform.GetChild(1).GetComponent<MeshCollider>();
		wallMesh.name = "Wall Mesh";
		vertices = new List<Vector3>();
		colors = new List<Color>();
		triangles = new List<int>();
		doorVertices = new List<Vector3>();
		doorColors = new List<Color>();
		doorTriangles = new List<int>();
		secretVertices = new List<Vector3>();
		secretColors = new List<Color>();
		secretTriangles = new List<int>();
	}

	public void Triangulate(HexRoom[] rooms)
	{
		wallMesh.Clear();
		doorMesh.Clear();
		secretMesh.Clear();
		vertices.Clear();
		colors.Clear();
		triangles.Clear();
		doorVertices.Clear();
		doorColors.Clear();
		doorTriangles.Clear();
		secretVertices.Clear();
		secretColors.Clear();
		secretTriangles.Clear();
		foreach (var room in rooms)
		{
			if(room != null)
				Triangulate(room);
		}
		wallMesh.vertices = vertices.ToArray();
		wallMesh.colors = colors.ToArray();
		wallMesh.triangles = triangles.ToArray();
		doorMesh.vertices = doorVertices.ToArray();
		doorMesh.colors = doorColors.ToArray();
		doorMesh.triangles = doorTriangles.ToArray();
		secretMesh.vertices = secretVertices.ToArray();
		secretMesh.colors = secretColors.ToArray();
		secretMesh.triangles = secretTriangles.ToArray();
		wallMesh.RecalculateNormals();
		doorMesh.RecalculateNormals();
		secretMesh.RecalculateNormals();
		meshCollider.sharedMesh = wallMesh;
		doorCollider.sharedMesh = doorMesh;
		secretCollider.sharedMesh = secretMesh;
	}

	void Triangulate(HexRoom room)
	{
		for (int i = 0; i < 6; i++)
		{
			if (room.walls[i] == WallType.Wall)
			{
				Triangulate(room, i);
			}
			else if (room.walls[i] == WallType.Door)
			{
				DoorTriangulate(room, i);
			}
			else if (room.walls[i] == WallType.Secret)
			{
				SecretTriangulate(room, i);
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

	void DoorTriangulate(HexRoom room, int wall)
	{
		Vector3 center = room.transform.localPosition;

		AddDoorTriangle(
			center + HexMetrics.wallCorners[wall * 6],
			center + HexMetrics.wallCorners[wall * 6 + 1],
			center + HexMetrics.wallCorners[wall * 6 + 2]
		);
		AddDoorTriangleColor(Color.magenta);
		AddDoorTriangle(
				center + HexMetrics.wallCorners[wall * 6 + 3],
				center + HexMetrics.wallCorners[wall * 6 + 4],
				center + HexMetrics.wallCorners[wall * 6 + 5]
			);
		AddDoorTriangleColor(Color.magenta);

	}

	void AddDoorTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = doorVertices.Count;
		doorVertices.Add(v1);
		doorVertices.Add(v2);
		doorVertices.Add(v3);
		doorTriangles.Add(vertexIndex);
		doorTriangles.Add(vertexIndex + 1);
		doorTriangles.Add(vertexIndex + 2);
	}

	void AddDoorTriangleColor(Color color)
	{
		doorColors.Add(color);
		doorColors.Add(color);
		doorColors.Add(color);
	}
	void SecretTriangulate(HexRoom room, int wall)
	{
		Vector3 center = room.transform.localPosition;

		AddSecretTriangle(
			center + HexMetrics.wallCorners[wall * 6],
			center + HexMetrics.wallCorners[wall * 6 + 1],
			center + HexMetrics.wallCorners[wall * 6 + 2]
		);
		AddSecretTriangleColor(room.color);
		AddSecretTriangle(
				center + HexMetrics.wallCorners[wall * 6 + 3],
				center + HexMetrics.wallCorners[wall * 6 + 4],
				center + HexMetrics.wallCorners[wall * 6 + 5]
			);
		AddSecretTriangleColor(room.color);

	}

	void AddSecretTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = secretVertices.Count;
		secretVertices.Add(v1);
		secretVertices.Add(v2);
		secretVertices.Add(v3);
		secretTriangles.Add(vertexIndex);
		secretTriangles.Add(vertexIndex + 1);
		secretTriangles.Add(vertexIndex + 2);
	}

	void AddSecretTriangleColor(Color color)
	{
		secretColors.Add(color);
		secretColors.Add(color);
		secretColors.Add(color);
	}
}