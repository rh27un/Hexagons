using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGrid : HexGrid
{
    [SerializeField]
    protected int roomsToGenerate;
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
    protected GameObject exitRoomPrefab;

    protected WallMesh wallMesh;
    protected List<HexRoom> rooms = new List<HexRoom>();
    protected List<GameObject> generatedObjects = new List<GameObject>();
    protected Player player;
    protected LevelName levelName;
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
        if (levelName == null)
            levelName = GameObject.Find("Level Name").GetComponent<LevelName>();

        rooms.Clear();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        var coord = HexCoordinates.FromOffsetCoordinates(width / 2, height / 2);
        if (isFirstLevel)
        {
            rooms.Add(PlaceRoom(coord, spawnRoomPrefab, true));
            isFirstLevel = false;
        }
        else
            rooms.Add(PlaceRoom(coord, null, true));
        player.MoveTo(rooms[0].transform.position + Vector3.up);
        for(int i = 0; i < branchesPerPath; i++)
		{
            GeneratePathFrom(coord, roomsPerPath, pathDepth);
		}
        var lastPrefab = generatedObjects[generatedObjects.Count - 1];
        var newPrefab = Instantiate(exitRoomPrefab, lastPrefab.transform.position, Quaternion.identity);
        Destroy(lastPrefab);
        generatedObjects.Add(newPrefab);
        levelName.NextLevel();
    }
	protected void GeneratePathFrom(HexCoordinates generateFrom, int numRooms, int depth)
    {
        int r = 0, s;
        var coord = generateFrom;
        for (int i = 0; i <= numRooms; i++)
        {
            HexRoom room;
            room = PlaceRoom(coord, roomPrefabs[UnityEngine.Random.Range(0, roomPrefabs.Count)]);
            if(!rooms.Contains(room))
                rooms.Add(room);
            if(i > 0)
			{
                room.walls[(r + 3) % 6] = true;
			}
            if (i == numRooms)
            {
                if(depth > 0)
				{
                    for (int j = 0; j < branchesPerPath; j++)
                    {
                        GeneratePathFrom(room.coordinates, numRooms - 1, depth - 1);
                    }
				}
                return;
            }

            var neighbours = coord.GetNeighbours();
            r = s = UnityEngine.Random.Range(0, 6);
            while(rooms.Contains((HexRoom)GetCell(neighbours[r])) || IndexFromCoordinates(neighbours[r]) > width * height)
			{
                r = (r + 1) % 6;
                if(r == s)
				{
                    Debug.LogWarning("No available neighbours");
                    return;
				}
			}
            room.walls[r] = true;
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
            room.transform.SetParent(transform, false);
            room.transform.localPosition = cells[i].transform.localPosition;
            room.coordinates = coordinates;
            room.color = white ? Color.white : UnityEngine.Random.ColorHSV();
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
                room.walls[i] = true;
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
