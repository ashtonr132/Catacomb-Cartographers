using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Tilemaps;

//this class defines the level grid, initialises the mesh creation script.

public class LevelGen : MonoBehaviour
{
    private int MinMaxSize = 15, noiseRounds = 4, wallHeight = 1;
    internal static int levelSize = 25;
    private int? seed;
    internal bool[,] grid;
    private Quaternion CameraAngle = Quaternion.Euler(new Vector3(-45, 180, 0));
    
    [SerializeField]
    internal bool useRandomSeed;

    [Range(0, 100)]
    internal int FillPercent = 40;
    private System.Random rnd;

    [SerializeField]
    Sprite[] Sprites;

    [SerializeField]
    GameObject portal, player;

    internal void InitLevel(int Size, int? Seed, int difficulty = 1)
    {
        if (Size > 25)
        {
            levelSize = Size;
        }
        seed = Seed;
        BuildMap();
        LevelFill();
    }

    private void LevelFill()
    {
        Vector4 t = new Vector4();
        float min = float.MaxValue, max = float.MinValue;
        for (int i = 0; i < levelSize; i++)
        {
            for (int j = 0; j < levelSize; j++)
            {
                if (grid[i, j] == true)
                {
                    if (i + j < min)
                    {
                        min = i + j;
                        t.x = i;
                        t.y = j;
                    }
                    else if (i + j > max)
                    {
                        max = i + j;
                        t.w = i;
                        t.z = j;
                    }
                }
            }
        }
        GameObject portal1 = Instantiate(portal, new Vector3(t.x - (levelSize / 2), 0.5f, t.y - (levelSize / 2)) + (Vector3.right + Vector3.forward), Quaternion.identity, transform);
        GameObject portal2 = Instantiate(portal, new Vector3(t.w - (levelSize / 2), 0.5f, t.z - (levelSize / 2)) - (Vector3.right + Vector3.forward), Quaternion.identity, transform);
        portal1.transform.localScale /= 4;
        portal2.transform.localScale /= 4;
        GameObject Player = Instantiate(player, portal1.transform.position - Vector3.forward, Quaternion.identity, transform);
        portal2.transform.rotation = Quaternion.Euler(90, 0, 0);
        portal1.transform.rotation = Quaternion.Euler(90, 0, 0);
        Player.transform.rotation = Quaternion.Euler(90, 0, 0);
        Player.name = "Player";
    }
    
    private void BuildMap()
    {
        useRandomSeed = seed == null ? true : false; //is this script accessed for a random level or a seeded one?
        GenMap();
        RemoveMinor(GetAreas(true), MinMaxSize);
        AddPaths(GetAreas(false), MinMaxSize);
        if (grid != null)
        {
            gameObject.AddComponent<MeshCollider>().sharedMesh = GetComponent<MeshGen>().GenerateMesh(grid, 1f);
            GetComponent<MeshFilter>().mesh = GetComponent<MeshCollider>().sharedMesh;
            transform.GetChild(0).gameObject.AddComponent<MeshCollider>().sharedMesh = GetComponent<MeshGen>().CreateWallMesh(wallHeight);
            transform.GetChild(0).GetComponent<MeshFilter>().mesh = transform.GetChild(0).GetComponent<MeshCollider>().sharedMesh;
            bool[,] b = grid;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    b[i, j] = grid[i, j] == false ? true:false;
                }
            }
            transform.GetChild(1).gameObject.AddComponent<MeshCollider>().sharedMesh = GetComponent<MeshGen>().GenerateMesh(b, 1f);
            transform.GetChild(1).GetComponent<MeshFilter>().mesh = transform.GetChild(1).gameObject.GetComponent<MeshCollider>().sharedMesh;
            transform.GetChild(1).position += Vector3.up * wallHeight;
        }
    }

    private void GenMap()
    {
        grid = new bool[levelSize, levelSize];
        FillMap();

        for (int i = 0; i < noiseRounds; i++)
        {
            ReduceNoise(i, 4, 4);
        }
    }

    private void RemoveMinor(List<Area> areas, int minWalls) //removes small pillar areas to leave more open larger areas
    {
        for (int i = 0; i < areas.Count; i++)
        {
            if (areas[i].Tiles.Count < minWalls)
            {
                for (int j = 0; j < areas[i].Tiles.Count; j++)
                {
                    grid[Convert.ToInt32(areas[i].Tiles[j].x), Convert.ToInt32(areas[i].Tiles[j].y)] = false;
                }
            }
        }
    }

    private void AddPaths(List<Area> areas, int minArea) //connect all unconnected areas in the map.
    {
        for (int a1 = 1; a1 < areas.Count; a1++) //foreach area
        {
            TilePair t = ClosestTiles(areas[a1].EdgeTiles, areas[0].EdgeTiles); //the two edge tiles that are the least distance apart
            if (t.tilepair != null && areas[a1].Tiles.Count > minArea)
            {
                ConnectAreas(t.tilepair);
            }
            else
            {
                var a = areas[a1];
                for (int i = 0; i < areas[a1].Tiles.Count; i++) //fill this area it cannot be connected
                {
                    grid[Convert.ToInt32(a.Tiles[i].x), Convert.ToInt32(a.Tiles[i].y)] = true;
                }
            }
        }
    }

    private void ConnectAreas(Vector4? vec2)
    {
        Vector4 vec = new Vector4(vec2.Value.x, vec2.Value.y, vec2.Value.z, vec2.Value.w); //vec? to vec
        float j = vec.x > vec.y ? vec.y : vec.x, max = j == vec.x ? vec.y : vec.x; //is x || y is the same ?

        for (float i = j; i < max; i++)
        {
            if (vec.z == 0) //x are the same
            {
                grid[Convert.ToInt32(vec.w), Convert.ToInt32(i)] = false;
            }
            else //y are the same
            {
                grid[Convert.ToInt32(i), Convert.ToInt32(vec.w)] = false;
            }
        }
    }

    private TilePair ClosestTiles(List<Vector2> V1, List<Vector2> V2)
    {
        int minDist = int.MaxValue;
        Vector4? tilePair = null;

        for (int t1 = 0; t1 < V1.Count; t1++) //foreach edgetile in V1
        {
            for (int t2 = 0; t2 < V2.Count; t2++) //foreach same y axis or same x axis tile
            {
                if (V1[t1].x == V2[t2].x || V1[t1].y == V2[t2].y)
                {
                    Vector2 tileA = V1[t1], tileB = V2[t2];
                    var dist = Convert.ToInt32(Mathf.Pow(tileA.x - tileB.x, 2) + Mathf.Pow(tileA.y - tileB.y, 2)); //dist between tiles
                    if (dist < minDist) //new smaller dist
                    {
                        minDist = dist;
                        tilePair = new Vector4(tileA.x, tileA.y, tileB.x, tileB.y); //new best tilepair
                    }
                }
            }
        }

        if (tilePair != null && tilePair.Value.x == tilePair.Value.z) //x axis same, vertical join
        {
            return new TilePair(minDist, new Vector4(tilePair.Value.y, tilePair.Value.w, 0, tilePair.Value.x));
        }
        else if (tilePair != null) // y axis same horizontal join
        {
            return new TilePair(minDist, new Vector4(tilePair.Value.x, tilePair.Value.z, 1, tilePair.Value.y));
        }
        else
        {
            return new TilePair(minDist, null);
        }
    }

    private List<Area> GetAreas(bool tileType)
    {
        List<Area> areas = new List<Area>();
        bool[,] checkedTiles = new bool[levelSize, levelSize];

        for (int x = 0; x < levelSize; x++)
        {
            for (int y = 0; y < levelSize; y++)
            {
                if (checkedTiles[x, y] == false && grid[x, y] == tileType) //if tile is unassigned start new area with tile
                {
                    Area newArea = new Area
                    {
                        Tiles = GetAreaTiles(x, y),
                        EdgeTiles = new List<Vector2>(),
                    };
                    newArea.area = newArea.Tiles.Count;
                    foreach (Vector2 tile in newArea.Tiles)
                    {
                        checkedTiles[Convert.ToInt32(tile.x), Convert.ToInt32(tile.y)] = true;
                        if (NeighbourCount(V: tile, adjacentOnly: true).Count > 0)
                        {
                            newArea.EdgeTiles.Add(tile);
                        }
                    }
                    areas.Add(newArea);
                }
            }
        }
        return areas.OrderByDescending(o => o.area).ToList(); //sort by area size
    }

    private List<Vector2> GetAreaTiles(int startX, int startY)
    {
        List<Vector2> tiles = new List<Vector2>();
        bool[,] checkedTiles = new bool[levelSize, levelSize];
        bool tileType = grid[startX, startY];

        Queue<Vector2> queue = new Queue<Vector2>();
        queue.Enqueue(new Vector2(startX, startY));
        checkedTiles[startX, startY] = true;

        while (queue.Count > 0)
        {
            Vector2 tile = queue.Dequeue();
            tiles.Add(tile);
            for (int x = Convert.ToInt32(tile.x) - 1; x <= tile.x + 1; x++)
            {
                for (int y = Convert.ToInt32(tile.y) - 1; y <= tile.y + 1; y++)
                {
                    if (Convert.ToBoolean(TileInMap(x: x, y: y)) && (y == tile.y || x == tile.x))
                    {
                        if (checkedTiles[x, y] == false && grid[x, y] == tileType)
                        {
                            checkedTiles[x, y] = true;
                            queue.Enqueue(new Vector2(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    private bool? TileInMap(Vector2? tile = null, int? x = null, int? y = null)
    {
        if (tile != null)
        {
            return (tile.Value.x >= 0 && tile.Value.y >= 0 && tile.Value.x < levelSize && tile.Value.y < levelSize);
        }
        else if (x != null && y != null)
        {
            return (x >= 0 && y >= 0 && x < levelSize && y < levelSize);
        }
        else
        {
            return null;
        }
    }
    
    private void FillMap()
    {
        if (useRandomSeed)
        {
            rnd = new System.Random();
            seed = rnd.Next(0, int.MaxValue);
        }
        else
        {
            rnd = new System.Random(Convert.ToInt32(seed));
        }
        for (int x = 0; x < levelSize; x++)
        {
            for (int y = 0; y < levelSize; y++)
            {
                grid[x, y] = (rnd.Next(0, 100) < FillPercent); //randomly turn a tile into a wall at fill%.
            }
        }
    }

    private void ReduceNoise(int i, int maxCut, int minCut) //cellular automata rules to generate map, altered to suit my ideal map.
    {
        for (int x = 0; x < levelSize; x++)
        {
            for (int y = 0; y < levelSize; y++)
            {
                int wallTiles = NeighbourCount(x, y).Count;
                if (wallTiles > maxCut || //cellular automata rule for turn to wall.
                   (x == 0 || x == levelSize - 1 || y == 0 || y == levelSize - 1) || //is border tile.
                   ((x == 1 || x == levelSize - 2 || y == 1 || y == levelSize - 2) && (rnd.Next(0, 100) < FillPercent * 2 && i < noiseRounds - 1))) //is next to border tile chance to be a wall is higher, this is used to generate a more natual border.
                {
                    grid[x, y] = true; //the tile is now a wall
                }
                else if (wallTiles < minCut)//cellular automata rule for turn to space.
                {
                    grid[x, y] = false; //the tile is now empty
                }//else leave this tile as it is
            }
        }
    }

    private List<Vector2> NeighbourCount(int? x1 = 0, int? y1 = 0, bool returnWalls = true, bool adjacentOnly = false, int radius = 1, Vector2? V = null) //how many of the surrounding radius tiles are tile typ input (walls)
    {
        int x, y; //convert nullables to non-nullables.
        if (V != null)
        {
            x = Convert.ToInt32(V.Value.x);
            y = Convert.ToInt32(V.Value.y);
        }
        else if (x1 != null && y1 != null)
        {
            x = Convert.ToInt32(x1);
            y = Convert.ToInt32(y1);
        }
        else
        {
            throw new ArgumentException("All input parameters Null", "NeighbourCount : int x, int y, Vector2(x,y)");
        }

        List<Vector2> Neighbours = new List<Vector2>();
        for (int i = x - radius; i <= x + radius; i++) //check 3 wide.
        {
            for (int j = y - radius; j <= y + radius; j++) //check 3 high.
            {
                if (Convert.ToBoolean(TileInMap(x: i, y: j))) //tile is not on the map border.
                {
                    if ((adjacentOnly && (i == x || j == y)) || !adjacentOnly)
                    {
                        if (returnWalls == grid[i, j])
                        {
                            Neighbours.Add(new Vector2(i, j));
                        }
                    }

                }
            }
        }
        return Neighbours; //returns list of chosen tile type around input tile.
    }
}

internal struct Area
{
    internal List<Vector2> Tiles { get; set; }
    internal List<Vector2> EdgeTiles { get; set; }
    internal int area { get; set; }
}

internal struct TilePair
{
    internal int minDist { get; set; }
    internal Vector4? tilepair { get; set; }

    internal TilePair(int maxValue, Vector4? tilePair) : this()
    {
        minDist = maxValue;
        tilepair = tilePair;
    }
}
