using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelGrid : HexGrid
{
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
	protected List<GameObject> generatedObjects = new List<GameObject>();
	protected Player player;
	protected LevelName levelName;
	protected GameManager gameManager;
	protected bool isFirstLevel = true;
	// Start is called before the first frame update
	protected override void Start()
	{
		hexMesh.Triangulate(cells);
		wallMesh = GetComponentInChildren<WallMesh>();
		//CalculateWalls();
		wallMesh.Triangulate(rooms.ToArray());
		levelName = GameObject.Find("Level Name").GetComponent<LevelName>();
	}
	private void Update()
	{
		if (Input.GetKeyDown("space"))
		{
			Regenerate();
		}
	}
	public void Regenerate()
	{
		foreach (var cell in cells)
		{
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
		Generate();
		hexMesh.Triangulate(cells);
		wallMesh.Triangulate(rooms.ToArray());
		GameObject.Find("Fog Grid").GetComponent<FogGrid>().RegenerateFog();
	}
	protected override void Generate()
	{

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
		if (coordinates.X + coordinates.Z / 2 >= width)
			return null;
		if (coordinates.Y > coordinates.X)
			return null;
		if (coordinates.Z < 0 || coordinates.Z > height)
			return null;
		if (index < 0 || index >= height * width)
			return null;
		return rooms[index];
	}
}
