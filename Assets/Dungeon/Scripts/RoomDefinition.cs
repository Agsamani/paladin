public enum WallState : byte
{
    Empty = 0,  
    Solid = 1, 
    Door = 2, 
    Rail = 3 
}

public enum WallIndex : int
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}

[System.Serializable]
public class RoomDefinition
{
    //[North, East, South, West]
    public WallState[] walls = new WallState[4] { WallState.Solid, WallState.Solid, WallState.Solid, WallState.Solid };

    public RoomDefinition() { }

    public RoomDefinition(WallState north, WallState east, WallState south, WallState west)
    {
        walls[(int)WallIndex.North] = north;
        walls[(int)WallIndex.East] = east;
        walls[(int)WallIndex.South] = south;
        walls[(int)WallIndex.West] = west;
    }

    public WallState GetWall(WallIndex i) => walls[(int)i];
    public void SetWall(WallIndex i, WallState s) => walls[(int)i] = s;

    public bool IsOpen(WallIndex i) => walls[(int)i] == WallState.Empty || walls[(int)i] == WallState.Door;
}
