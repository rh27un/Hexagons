using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public struct ValidRoom
{
	public ValidRoom(HexCoordinates _coords, byte _roomType, HexCoordinates _doorCell, int _doorDirection)
	{
		coords = _coords;
		roomType = _roomType;
		doorCell = _doorCell;
		doorDirection = _doorDirection;
	}

	public HexCoordinates coords;
	public byte roomType;
	public HexCoordinates doorCell;
	public int doorDirection;
}

public class LevelGrid : HexGrid
{

	public const byte BIGROOM = 0b111111;
	public const byte MEDIUMROOM = 0b110000;
	public const byte SMALLROOM = 0b000000;
	public const byte LONGROOM = 0b100100;
	public const byte SHORTROOM = 0b100000;
	public const byte CROSSROOM = 0b101010;
	public const byte LCURVEDROOM = 0b101000;
	public const byte RCURVEDROOM = 0b100010;
	public const byte LRHOMBOIDROOM = 0b111000;
	public const byte RRHOMBOIDROOM = 0b110001;
	public const byte SEMIROOM = 0b111100;
	public const byte CRESCENTROOM = 0b111110;
	public const byte BONEROOM = 0b110110;
	public const byte PICKROOM = 0b111010;


	[SerializeField]
	protected int roomsPerPath;
	[SerializeField]
	protected int pathDepth;
	[SerializeField]
	protected int branchesPerPath;

	[SerializeField]
	protected GameObject spawnRoomPrefab;

	[SerializeField]
	protected List<GameObject> roomPrefabs;

	[SerializeField]
	protected List<GameObject> secretRoomPrefabs;

	[SerializeField]
	protected List<GameObject> bossRoomPrefabs;

	[SerializeField]
	protected GameObject exitRoomPrefab;

	protected WallMesh wallMesh;
	protected List<HexRoom> rooms = new List<HexRoom>();
	protected Dictionary<int, HexRoom> roomdictionary = new Dictionary<int, HexRoom>();
	protected List<GameObject> generatedObjects = new List<GameObject>();
	protected Player player;
	protected LevelName levelName;
	protected GameManager gameManager;
	protected bool isFirstLevel = true;
	int rot = 0;
	// Start is called before the first frame update
	protected override void Start()
	{
		hexMesh.Triangulate(cells);
		wallMesh = GetComponentInChildren<WallMesh>();
		//CalculateWalls();
		wallMesh.Triangulate(roomdictionary.Values.ToArray());
		levelName = GameObject.Find("Level Name").GetComponent<LevelName>();
	}
	private void Update()
	{
		if (Input.GetKeyDown("space"))
		{
			Regenerate();
		}
	}

	protected List<ValidRoom> FindValidMoves(HexCoordinates coord, byte[] roomsToTry)
	{
		// list of valid rooms for this coordinate
		List<ValidRoom> validRooms = new List<ValidRoom>();
		// if this coordinate is somehow invalid
		if (GetCell(coord) == null || GetRoom(coord) != null)
			return validRooms;
		HexCoordinates doorCell = new HexCoordinates();
		int doorDir = 0;
		foreach(byte room in roomsToTry)
		{
			for (int r = 0; r < 6; r++)
			{
				byte rotatedRoom = RotateRoom(room, r);
				if (r > 0 && AreRoomsIdentical(room, rotatedRoom))
					continue;
				bool valid = true;
				bool isConnected = false;
				var myNeighbours = coord.GetNeighbours();
				// for each neighbouring cell
				for (int i = 0; i < 6; i++)
				{
					// we don't care if it's not part of the room
					if (IsBitSet(rotatedRoom, i))
					{
						// room is invalid if any part of it is either null (out of bounds) or already occupied 
						if (GetCell(myNeighbours[i]) == null || GetRoom(myNeighbours[i]) != null)
						{
							valid = false;
							break;
						}
						// room has to be connected to another room to be valid
						if (!isConnected)
						{
							// ooh the rare triple-for-loop
							// gotta loop through all our neighbours to see 

							var ourNeighbours = myNeighbours[i].GetNeighbours();
							//foreach (var ourNeighbour in myNeighbours[i].GetNeighbours().Where(on => on != coord && !myNeighbours.Contains(on)))
							for (int j = 0; j < ourNeighbours.Count(); j++)
							{
								var ourNeighbour = ourNeighbours[j];
								// if this neighbouring cell both exists and consists of a room we're connected
								if (GetCell(ourNeighbour) != null && GetRoom(ourNeighbour) != null)
								{
									doorCell = myNeighbours[i];
									doorDir = j;
									isConnected = true;
									break;
								}
							}

						}
					}
					else
					{
						// just in case
						if (!isConnected && GetCell(myNeighbours[i]) != null && GetRoom(myNeighbours[i]) != null)
						{
							doorCell = coord;
							doorDir = i;
							isConnected = true;
						}
					}

				}
				if (valid && isConnected)
					validRooms.Add(new ValidRoom(coord, rotatedRoom, doorCell, doorDir));
			}
		}
		return validRooms;
	}
	public void Regenerate()
	{
		rot++;
		foreach (var cell in cells)
		{
			if(cell	 != null)
			Destroy(cell.gameObject);
		}
		foreach (var go in generatedObjects)
		{
			Destroy(go);
		}
		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++)
		{
			for (int x = 0; x < width; x++)
			{
				CreateCell(x, z, i++);
			}
		}
		NewGenerate();
		hexMesh.Triangulate(cells);
		wallMesh.Triangulate(roomdictionary.Values.ToArray());
		GameObject.Find("Fog Grid").GetComponent<FogGrid>().RegenerateFog();
	}

	protected byte RotateRoom(byte room, int rotations)
	{
		rotations = rotations % 6;
		for(int i = 0; i < rotations; i++)
		{
			byte mask = 0b000001;
			byte lastbit = (byte)(room & mask);
			room >>= 1;
			lastbit <<= 5;
			room |= lastbit;
		}
		return room;
	}

	protected bool AreRoomsIdentical(byte a, byte b)
	{
		byte u_b = (byte)~b;
		byte ab = (byte)(a | u_b);
		return (ab | 0x30) == byte.MaxValue;
	}

	protected bool IsBitSet(byte val, int bit)
	{
		byte mask = (byte)(1 << bit);
		byte c = (byte)(val & mask);
		return c > 0;
	}

	protected HexRoom NewPlaceRoom(HexCoordinates coords, byte neighboursInRoom, int roomId)
	{
		int i = IndexFromCoordinates(coords);

		if (cells[i] != null)
		{
			if (rooms.Contains((HexRoom)cells[i]))
			{
				return (HexRoom)cells[i];
			}
			HexRoom room = (HexRoom)Instantiate<HexCell>(cellPrefab);
			room.gameObject.name = i.ToString();
			room.walls = new WallType[6]; // dont fucking ask me
			room.transform.SetParent(transform, false);
			room.roomId = roomId;
			room.transform.localPosition = cells[i].transform.localPosition;
			room.coordinates = coords;
			room.color = Color.red;
			roomdictionary.Add(i, room);
			Destroy(cells[i].gameObject);
			cells[i] = room;
			for (int j = 0; j < 6; j++)
			{
				if (IsBitSet(neighboursInRoom, j))
				{
					var ncoords = coords.GetNeighbours()[j];
					int ni = IndexFromCoordinates(ncoords);
					if (cells[ni] != null)
					{
						if (rooms.Contains((HexRoom)cells[ni]))
						{
							break;
						}
						HexRoom nroom = (HexRoom)Instantiate<HexCell>(cellPrefab);
						nroom.gameObject.name = ni.ToString();
						nroom.walls = new WallType[6]; // dont fucking ask me
						nroom.transform.SetParent(transform, false);
						nroom.roomId = roomId;
						nroom.transform.localPosition = cells[ni].transform.localPosition;
						nroom.coordinates = ncoords;
						nroom.color = Color.red;
						roomdictionary.Add(ni, nroom);
						Destroy(cells[ni].gameObject);
						cells[ni] = nroom;
					}
				}
			}

			return room;
		}
		return null;
	}

	protected HexRoom NewPlaceRoom(HexCoordinates coords, byte neighboursInRoom, int roomId, HexCoordinates doorCell, int doorDir)
	{
		int i = IndexFromCoordinates(coords);

		if (cells[i] != null)
		{
			if (rooms.Contains((HexRoom)cells[i]))
			{
				return (HexRoom)cells[i];
			}
			HexRoom room = (HexRoom)Instantiate<HexCell>(cellPrefab);
			room.gameObject.name = i.ToString();
			room.walls = new WallType[6]; // dont fucking ask me
			room.transform.SetParent(transform, false);
			room.roomId = roomId;
			room.transform.localPosition = cells[i].transform.localPosition;
			room.coordinates = coords;
			room.color = roomId == 0 ? Color.red : Color.blue;
			roomdictionary.Add(i, room);
			Destroy(cells[i].gameObject);
			cells[i] = room;
			for (int j = 0; j < 6; j++)
			{
				if (IsBitSet(neighboursInRoom, j))
				{
					var ncoords = coords.GetNeighbours()[j];
					int ni = IndexFromCoordinates(ncoords);
					if (cells[ni] != null)
					{
						if (rooms.Contains((HexRoom)cells[ni]))
						{
							break;
						}
						HexRoom nroom = (HexRoom)Instantiate<HexCell>(cellPrefab);
						nroom.walls = new WallType[6]; // dont fucking ask me
						nroom.gameObject.name = ni.ToString();
						nroom.transform.SetParent(transform, false);
						nroom.roomId = roomId;
						nroom.transform.localPosition = cells[ni].transform.localPosition;
						nroom.coordinates = ncoords;
						nroom.color = roomId == 0 ? Color.red : Color.blue;
						roomdictionary.Add(ni, nroom);
						Destroy(cells[ni].gameObject);
						cells[ni] = nroom;
					}
				}
			}

			if(GetRoom(doorCell) != null && GetRoom(doorCell).roomId == roomId)
			{
				if(GetRoom(doorCell.GetNeighbours()[doorDir]) != null)
				{
					GetRoom(doorCell).walls[doorDir] = WallType.Door;
					GetRoom(doorCell.GetNeighbours()[doorDir]).walls[(doorDir + 3) % 6] = WallType.Door;
				} else
				{
					Debug.LogError($"Cell {doorCell.GetNeighbours()[doorDir]} (Cell {doorCell} direction {doorDir}) invalid to place door");
				}
			} else
			{
				Debug.LogError($"Cell {doorCell} invalid to place door");
			}
			return room;
		}
		return null;
	}
	protected void NewGenerate()
	{
		// Set up object references
		if (levelName == null)
			levelName = GameObject.Find("Level Name").GetComponent<LevelName>();
		if (gameManager == null)
			gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

		// find player
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

		// clear rooms
		rooms.Clear();
		roomdictionary.Clear();

		// place the first room in the centre
		var coord = HexCoordinates.FromOffsetCoordinates(width / 2, height / 2);
		if (isFirstLevel)
		{
			NewPlaceRoom(coord, SHORTROOM, 0);
			isFirstLevel = false;
		}
		else
			NewPlaceRoom(coord, RotateRoom(SHORTROOM, 0), 0);


		float startTime = Time.realtimeSinceStartup;
		byte[] roomTypes = new byte[14] { BIGROOM, MEDIUMROOM, SMALLROOM, LONGROOM, SHORTROOM, CROSSROOM, LCURVEDROOM, RCURVEDROOM,LRHOMBOIDROOM, RRHOMBOIDROOM,
		SEMIROOM, CRESCENTROOM, BONEROOM, PICKROOM };
		List<ValidRoom> validRooms = new List<ValidRoom>();
		for(int i = 0; i < cells.Length; i++)
		{
			validRooms.AddRange(FindValidMoves(cells[i].coordinates, roomTypes));
		}
		float endTime = Time.realtimeSinceStartup;
		Debug.Log($"{validRooms.Count} rooms valid out of {cells.Length} cells tested in {(endTime - startTime) * 1000} milliseconds");

		NewPlaceRoom(validRooms[rot].coords, validRooms[rot].roomType, 1, validRooms[rot].doorCell, validRooms[rot].doorDirection);
		FigureOutWalls();
		// put the player there
		player.MoveTo(roomdictionary[IndexFromCoordinates(coord)].transform.position + Vector3.up);
		levelName.NextLevel();
		gameManager.NextLevel();
	}

	protected void FigureOutWalls()
	{
		foreach(var room in roomdictionary.Values)
		{
			var neighbours = room.coordinates.GetNeighbours();
			for(int i = 0; i < 6; i++)
			{
				var neighbour = GetRoom(neighbours[i]);
				
				if(room.walls[i] == WallType.None)
				{
					if(neighbour == null || neighbour.roomId != room.roomId)
					{
						room.walls[i] = WallType.Wall;
					}
				}
			}
		}
	}
	protected override void Generate()
	{
		NewGenerate();
		return;

		// Set up object references
		if (levelName == null)
			levelName = GameObject.Find("Level Name").GetComponent<LevelName>();
		if(gameManager == null)
			gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

		// clear rooms
		rooms.Clear();

		// find player
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

		// place the first room in the centre
		var coord = HexCoordinates.FromOffsetCoordinates(width / 2, height / 2);
		if (isFirstLevel)
		{
			rooms.Add(PlaceRoom(coord, spawnRoomPrefab, true));
			isFirstLevel = false;
		}
		else
			rooms.Add(PlaceRoom(coord, null, true));

		// put the player there
		player.MoveTo(rooms[0].transform.position + Vector3.up);

		// generate paths from the start
		for(int i = 0; i < branchesPerPath; i++)
		{
			GeneratePathFrom(coord, roomsPerPath, pathDepth);
		}

		// find rooms with empty neighbours
		var perimiterRooms = rooms.Where(r => r.coordinates.GetNeighbours().Any(n => !rooms.Contains((HexRoom)GetCell(n)) && IndexFromCoordinates(n) <= width * height)).ToArray();
		if (perimiterRooms.Length == 0) {
			Debug.LogError("No rooms with empty neighbours, leave extra space for secret room & exit room");
			return;
		}

		// the last one is the boss room
		var bossRoom = perimiterRooms[perimiterRooms.Length - 1];

		var neighbours = bossRoom.coordinates.GetNeighbours();
		int d = UnityEngine.Random.Range(0, 6), s = d;

		// if it isn't empty, cycle through neighbours clockwise until we get one that is
		while (rooms.Contains((HexRoom)GetCell(neighbours[d])) || IndexFromCoordinates(neighbours[d]) > width * height)
		{
			d = (d + 1) % 6;
			if (d == s)
			{
				// if we can't find any empty neighbours, give up
				Debug.LogError("Should never hit this");
				return;
			}
		}

		// place the room
		var exitRoom = PlaceRoom(neighbours[d], null);

		// place a door between the two
		bossRoom.walls[d] = WallType.Door;
		exitRoom.walls[(d + 3) % 6] = WallType.Door;

		rooms.Add(exitRoom);

		// select one at random to be entrance to secret room
		var secretEntrance = perimiterRooms.Where(r => r.coordinates.GetNeighbours().Any(n => !rooms.Contains((HexRoom)GetCell(n)) && IndexFromCoordinates(n) <= width * height)).ToArray()[UnityEngine.Random.Range(0, perimiterRooms.Length - 1)];

		// select a random neighbour to be the secret room
		neighbours = secretEntrance.coordinates.GetNeighbours();
		d = UnityEngine.Random.Range(0, 6); s = d;

		// if it isn't empty, cycle through neighbours clockwise until we get one that is
		while (rooms.Contains((HexRoom)GetCell(neighbours[d])) || IndexFromCoordinates(neighbours[d]) > width * height)
		{
			d = (d + 1) % 6;
			if (d == s)
			{
				// if we can't find any empty neighbours, give up
				Debug.LogError("Should never hit this");
				return;
			}
		}
		// place the room
		var secretRoom = PlaceRoom(neighbours[d], null);

		// place an illusory wall between the two
		secretEntrance.walls[d] = WallType.Secret;
		secretRoom.walls[(d + 3) % 6] = WallType.Secret;

		rooms.Add(secretRoom);

		//var lastPrefab = generatedObjects[generatedObjects.Count - 1];
		//var newPrefab = Instantiate(exitRoomPrefab, lastPrefab.transform.position, Quaternion.identity);
		//Destroy(lastPrefab);
		//generatedObjects.Add(newPrefab);
		FillRooms(rooms[0].coordinates, secretRoom.coordinates, bossRoom.coordinates, exitRoom.coordinates);
		levelName.NextLevel();
		gameManager.NextLevel();
	}

	protected void FillRooms(HexCoordinates start, HexCoordinates secret, HexCoordinates boss, HexCoordinates exit)
	{
		//generatedObjects.Add(Instantiate(spawnRoomPrefab, GetPositionFromCoordinates(start), Quaternion.identity));
		generatedObjects.Add(Instantiate(secretRoomPrefabs[UnityEngine.Random.Range(0, secretRoomPrefabs.Count)], GetPositionFromCoordinates(secret), Quaternion.identity));
		generatedObjects.Add(Instantiate(bossRoomPrefabs[UnityEngine.Random.Range(0, bossRoomPrefabs.Count)], GetPositionFromCoordinates(boss), Quaternion.identity));
		generatedObjects.Add(Instantiate(exitRoomPrefab, GetPositionFromCoordinates(exit), Quaternion.identity));

		for(int i = 0; i < rooms.Count; i++)
		{
			if (rooms[i].coordinates != start && rooms[i].coordinates != secret && rooms[i].coordinates != boss && rooms[i].coordinates != exit)
			{
				generatedObjects.Add(Instantiate(roomPrefabs[UnityEngine.Random.Range(0, roomPrefabs.Count)], GetPositionFromCoordinates(rooms[i].coordinates), Quaternion.identity));
			}
		}
	}
	protected void GeneratePathFrom(HexCoordinates generateFrom, int numRooms, int depth)
	{
		int r = 0, s;
		var coord = generateFrom;

		// generate path
		for (int i = 0; i <= numRooms; i++)
		{
			// place room
			HexRoom room;
			room = PlaceRoom(coord, null);
			if(!rooms.Contains(room))
				rooms.Add(room);

			// open path to previous room
			if(i > 0)
			{
				room.walls[(r + 3) % 6] = WallType.Door;
			}

			// if this is the last room in the path..
			if (i == numRooms)
			{
				// if we want to generate more branches
				if(depth > 0)
				{
					// recursive call to generate further branches from here
					for (int j = 0; j < branchesPerPath; j++)
					{
						GeneratePathFrom(room.coordinates, numRooms - 1, depth - 1);
					}
				}
				// this branch is finished
				return;
			}
			// if it isn't, keep generating the path

			// get a random neighbour
			var neighbours = coord.GetNeighbours();
			r = s = UnityEngine.Random.Range(0, 6);

			// if it isn't empty, cycle through neighbours clockwise until we get one that is
			while(rooms.Contains((HexRoom)GetCell(neighbours[r])) || IndexFromCoordinates(neighbours[r]) > width * height)
			{
				r = (r + 1) % 6;
				if(r == s)
				{
					// if we can't find any empty neighbours, give up
					Debug.LogWarning("No available neighbours");
					return;
				}
			}

			// open up the door to the next room in the path
			room.walls[r] = WallType.Door;

			// generate the room in aforementioned neighbour in the next loop
			coord = neighbours[r];
		}
	}

	public HexRoom PlaceRoom(HexCoordinates coordinates, GameObject prefab, bool white = false)
	{
		int i = IndexFromCoordinates(coordinates);
		
		if (cells[i] != null)
		{
			if (rooms.Contains((HexRoom)cells[i]))
			{
				return (HexRoom)cells[i];
			}
			HexRoom room = (HexRoom)Instantiate<HexCell>(cellPrefab);
			room.walls = new WallType[6]; // dont fucking ask me
			room.transform.SetParent(transform, false);
			room.transform.localPosition = cells[i].transform.localPosition;
			room.coordinates = coordinates;
			room.color = white ? Color.white : UnityEngine.Random.ColorHSV();
			for(int j = 0; j < 6; j++)
			{
				room.walls[j] = WallType.Wall;
			}
			if(prefab != null)
				generatedObjects.Add(Instantiate(prefab, room.transform.position, Quaternion.identity));

			Destroy(cells[i].gameObject);
			cells[i] = room;
			return room;
		}
		return null;
	}

	private void CalculateWalls()
	{
		foreach(var room in rooms)
		{
			if (room == null)
				continue;
			var neighours = room.coordinates.GetNeighbours();
			for(int i = 0; i < 6; i++)
			{
				room.walls[i] = WallType.Door;
	//            if (GetRoom(neighours[i]) != null)
	//            {
	//                var neighour = GetRoom(neighours[i]);
	//                room.walls[i] = !(neighour.walls[(i + 3) % 6]);
	//            }
				//else
				//{
	//                room.walls[i] = true;
				//}
			}
		}
	}

	public HexRoom GetRoom(HexCoordinates coordinates)
	{
		int index = IndexFromCoordinates(coordinates);
		if (!roomdictionary.ContainsKey(index))
			return null;
		if (coordinates.X + coordinates.Z / 2 >= width)
			return null;
		if (coordinates.Y > coordinates.X)
			return null;
		if (coordinates.Z < 0 || coordinates.Z > height)
			return null;
		if (index < 0 || index >= height * width)
			return null;
		return roomdictionary[index];
	}
}
