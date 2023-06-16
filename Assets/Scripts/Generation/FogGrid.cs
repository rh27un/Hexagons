using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	public bool UpdateFog(Vector3 pos)
	{
		var coords = GetCoordinatesFromPosition(pos);
		if (GetCell(coords) != null)
		{
			Destroy(cells[IndexFromCoordinates(coords)].gameObject);
			cells[IndexFromCoordinates(coords)] = null;
			hexMesh.Triangulate(cells);
			return true;
		}
		return false;
	}

	public bool AnyEnemyInRoom(Vector3 pos)
	{
		var coords = GetCoordinatesFromPosition(pos);
		var middle = GetPositionFromCoordinates(coords) - Vector3.up * 2f;
		var colliders = Physics.OverlapSphere(middle, 15f);
w		foreach(var collider in colliders)
		{
			if (collider.tag == "Enemy")
			{
				RaycastHit[] hits = Physics.RaycastAll(middle, collider.transform.position - middle, 15f).OrderBy(h => h.distance).ToArray();
				if (hits.Length > 0)
				{
					foreach (var hit in hits)
					{
						if (hit.collider.tag == "Wall")
							break;
						if (hit.collider == collider)
							return true;
					}
				}
			}
		}
		return false;
	}
}
