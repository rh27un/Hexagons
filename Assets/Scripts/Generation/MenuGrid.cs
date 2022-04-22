using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGrid : HexGrid
{
	protected override void Generate()
	{
		foreach(var cell in cells)
		{
			cell.color = Random.ColorHSV();
		}
		hexMesh.Triangulate(cells);
	}
}
