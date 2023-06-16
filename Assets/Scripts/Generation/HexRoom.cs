using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallType
{
	None = 0,
	Wall,
	Door,
	Secret
}
public class HexRoom : HexCell
{
	public WallType[] walls = new WallType[6];
}
