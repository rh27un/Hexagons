using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogGrid : HexGrid
{
	protected override void Start()
	{
		hexMesh.Triangulate(cells);
	}
	public void RegenerateFog()
	{
		foreach (var cell in cells)
		{
			if(cell != null)
				Destroy(cell.gameObject);
		}
		for (int z = 0, i = 0; z < height; z++)
		{
			for (int x = 0; x < width; x++)
			{
				CreateCell(x, z, i++);
			}
		}
		Generate();
		hexMesh.Triangulate(cells);
	}
	public void UpdateFog(Vector3 pos)
	{
		var coords = GetCoordinatesFromPosition(pos);
		if (GetCell(coords) != null)
		{
			Destroy(cells[IndexFromCoordinates(coords)].gameObject);
			cells[IndexFromCoordinates(coords)] = null;
			hexMesh.Triangulate(cells);
		}
	}
}
