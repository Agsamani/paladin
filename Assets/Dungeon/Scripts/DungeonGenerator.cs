using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.AI.Navigation;

public class DungeonGenerator : MonoBehaviour
{
    public enum RoomType { Void=0, Start=1, Normal=2, End=3 };

    public int MaxX = 10;
    public int MaxZ = 10;

    public int RoomCount = 7; // TODO: check for validity: can cause problem if greater than free space

    private int[] _grid;
    private WallState[] _wallGridH;
    private WallState[] _wallGridV;
    private List<Vector2Int> _roomCandidates;

    private static readonly Vector2Int[] Offsets =
        { new(1,0), new(0,-1), new(-1,0), new(0,1) };

    private Vector2Int _startingPos;

    public DungeonRoom[] Rooms;
    public float RoomOffset = 1.0f;

    [Range(0f, 1.0f)]
    public float BigRoomProbability = 0.5f;

    public RoomTemplate RoomTemp;


    void Awake()
    {
        _grid = new int[MaxX * MaxZ];
        _wallGridH = new WallState[(MaxX) * (MaxZ+1)];
        _wallGridV = new WallState[(MaxX+1) * (MaxZ)];
        _roomCandidates = new List<Vector2Int>();
    }

    void Start()
    {
        GenerateStartingRoom();
        GenerateDungeon();
        GenerateEndRoom();
        GenerateWalls();
        SpawnMeshes();

        GetComponent<NavMeshSurface>().BuildNavMesh();
        GetComponent<EnemySpawnManager>()?.SetGridData(MaxX, MaxZ, _grid, RoomOffset);
    }

    void GenerateStartingRoom()
    {
        int x = 0;// Random.Range(0, MaxX);
        int z = 0;// Random.Range(0, MaxZ);

        _grid[z + MaxZ * x] = (int)RoomType.Start;

        _startingPos = new Vector2Int(x, z);

        for (int i = 0; i < 4; i++)
        {
            Vector2Int p = new Vector2Int(x, z) + Offsets[i];
            if (IsCellValid(p.x, p.y)) _roomCandidates.Add(p);
        }
    }

    void GenerateDungeon()
    {
        for(int i = 1; i < RoomCount;)
        {
            if(_roomCandidates.Count == 0)
            {
                Debug.Log("No more candidates");
                break;
            }
            int rndIdx = Random.Range(0, _roomCandidates.Count);
            Vector2Int p = _roomCandidates[rndIdx];

            // TODO: change this

            bool f = false;
            for (int j = 0; j < 4; j++)
            {
                Vector2Int c = new Vector2Int(p.x, p.y) + Offsets[j];
                int nCount = GetNeighbourCount(c.x, c.y);
                if (nCount >= 2) f = true;
            }
            if (f && Random.value > BigRoomProbability) continue;


            _roomCandidates.RemoveAt(rndIdx);

            if (!IsCellValid(p.x, p.y)) continue;

            _grid[p.y + MaxZ * p.x] = (int)RoomType.Normal;

            for (int j = 0; j < 4; j++)
            {
                Vector2Int c = new Vector2Int(p.x, p.y) + Offsets[j];
                if (IsCellValid(c.x, c.y)) _roomCandidates.Add(c);
            }

            i++;
        }
    }

    void GenerateEndRoom()
    {
        Vector2Int sel = _startingPos;
        float dist = 0;
        for(int x = 0; x < MaxX; x++)
        {
            for (int z = 0; z < MaxZ; z++)
            {
                if (_grid[z + MaxZ * x] == (int)RoomType.Normal)
                {
                    Vector2Int distVec = new Vector2Int(x, z) - _startingPos;
                    if(distVec.magnitude > dist)
                    {
                        sel = new Vector2Int(x, z);
                        dist = distVec.magnitude;
                    }
                }
            }
        }

        _grid[sel.y + MaxZ * sel.x] = (int)RoomType.End;
    }

    int GetNeighbourCount(int x, int z)
    {
        int c = 0;
        for (int j = 0; j < 4; j++)
        {
            Vector2Int p = new Vector2Int(x, z) + Offsets[j];
            if (p.x >= 0 && p.x < MaxX && 
                p.y >= 0 && p.y < MaxZ && 
                _grid[p.y + MaxZ * p.x] != 0) c++;
        }
        return c;
    }

    bool IsCellValid(int x, int z)
    {
        if (x < 0 || x >= MaxX) return false;
        if (z < 0 || z >= MaxZ) return false;
        if (_grid[z + MaxZ * x] != 0) return false;
        return true;
    }

    void GenerateWalls()
    {
        for(int x = 0; x < MaxX; x++)
        {
            for (int z = 0; z < MaxZ; z++)
            {
                if (_grid[z + MaxZ * x] == 0) continue;

                WallState solid = GetNeighbourCount(x, z) == 1 ? WallState.Rail : WallState.Solid;
                WallState empty = GetNeighbourCount(x, z) == 2 ? WallState.Door : WallState.Empty;

                Vector2Int c = new Vector2Int(x, z) + new Vector2Int(-1, 0);
                if (c.x >= 0 && c.x < MaxX &&
                    _grid[c.y + MaxZ * c.x] != 0)
                    _wallGridV[z + MaxZ * x] = empty;
                else
                    _wallGridV[z + MaxZ * x] = solid;


                c = new Vector2Int(x, z) + new Vector2Int(1, 0);
                if (c.x >= 0 && c.x < MaxX &&
                    _grid[c.y + MaxZ * c.x] != 0)
                    _wallGridV[z + MaxZ * (x+1)] = empty;
                else
                    _wallGridV[z + MaxZ * (x + 1)] = solid;
                
                c = new Vector2Int(x, z) + new Vector2Int(0, -1);
                if (c.y >= 0 && c.y < MaxX &&
                    _grid[c.y + MaxZ * c.x] != 0)
                    _wallGridH[z + (MaxZ + 1) * x] = empty;
                else
                    _wallGridH[z + (MaxZ + 1) * x] = solid;

                c = new Vector2Int(x, z) + new Vector2Int(0, 1);
                if (c.y >= 0 && c.y < MaxZ &&
                    _grid[c.y + MaxZ * c.x] != 0)
                    _wallGridH[z + 1 + (MaxZ+1) * x] = empty;
                else
                    _wallGridH[z + 1 + (MaxZ + 1) * x] = solid;


            }
        }

    }

    void SpawnMeshes()
    {
        for(int x = 0; x < MaxX; x++)
        {
            for (int z = 0; z < MaxZ; z++)
            {
                int roomIdx = _grid[z + MaxZ * x];
                if (roomIdx != (int)RoomType.Void)
                {
                    DungeonRoom obj = Instantiate(Rooms[roomIdx], new Vector3(x * RoomOffset, 0, z * RoomOffset), Quaternion.identity);
                    RoomDefinition def = new RoomDefinition();

                    def.SetWall(WallIndex.South, _wallGridH[z + (MaxZ + 1) * x]);
                    def.SetWall(WallIndex.North, _wallGridH[z + 1 + (MaxZ + 1) * x]);
                    def.SetWall(WallIndex.West, _wallGridV[z + (MaxZ) * x]);
                    def.SetWall(WallIndex.East, _wallGridV[z + (MaxZ) * (x+1)]);

                    obj.InitWalls(RoomTemp, def);



                    if (_grid[z + MaxZ * x] == (int)RoomType.Start) 
                        obj.color = new Color(0.72f, 0.09f, 0.88f);
                    if (_grid[z + MaxZ * x] == (int)RoomType.Normal) 
                        obj.color = new Color(0.89f, 0.89f, 0.88f);
                    if (_grid[z + MaxZ * x] == (int)RoomType.End) 
                        obj.color = new Color(0.08f, 0.89f, 0.67f);

                    obj.transform.SetParent(gameObject.transform);
                }
            }
        }

    }
}
