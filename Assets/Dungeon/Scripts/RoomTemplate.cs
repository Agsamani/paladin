using UnityEngine;

public class RoomTemplate : MonoBehaviour
{
    [SerializeField]
    private WallTemplate[] _wallTemplates = new WallTemplate[4];

    public void Generate(RoomDefinition def)
    {
        for(int i = 0; i < 4; i++)
        {
            _wallTemplates[i].Generate(def.walls[i]);
        }
    }

}
