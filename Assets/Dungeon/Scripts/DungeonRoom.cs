using UnityEngine;

[System.Flags]
public enum Walls { None = 0, North = 1, East = 2, South = 4, West = 8 }

public abstract class DungeonRoom : MonoBehaviour
{
    [SerializeField]
    protected RoomDefinition _roomDef;

    [SerializeField]
    protected RoomTemplate _roomTemplate;


    public Color color = new Color();
    
    protected static readonly Vector2Int[] Offsets =
        { new(1,0), new(0,1), new(-1,0), new(0,-1) };

    protected float dim = 1.0f;

    public void InitWalls(RoomTemplate room, RoomDefinition def, float dim=1.0f)
    {
        _roomTemplate = Instantiate(room, gameObject.transform);
        _roomTemplate.transform.SetParent(gameObject.transform);
        _roomTemplate.Generate(def);
        _roomDef = def;
        this.dim = dim;
        GenerateRoom();
    }
    protected abstract void GenerateRoom();

    private void OnDrawGizmos()
    {
        Gizmos.color = this.color;
        Vector3 pos = gameObject.transform.position + new Vector3(2.0f, 2.0f, 2.0f);
        Gizmos.DrawWireCube(pos, new Vector3(4f, 4f, 4f));

        for (int j = 0; j < 4; j++)
        {
            if (_roomDef.walls[j] == WallState.Empty) 
                Gizmos.DrawWireSphere(new Vector3(pos.x + Offsets[j].x * 1f, 0, pos.z + Offsets[j].y * 1f), 1f);
        }


    }
}
