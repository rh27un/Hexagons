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
public class LevelCell : HexCell
{
	public WallType[] walls = new WallType[6];
	public int roomId;
}

 // a room being defined as one cell with a bunch of neighbours that have no walls in between them
 // so a big circular room might be defined as being 1 1 1 1 1 1
 // a room with 2 cells would be something like 1 0 0 0 0 0
 // rotate the room by shifting this 

public class Room
{
	public byte cellNeighboursInRoom;
}