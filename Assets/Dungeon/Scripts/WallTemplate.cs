using UnityEngine;

public class WallTemplate : MonoBehaviour
{
    public GameObject[] TemplateWallTypes = new GameObject[4];

    public void Generate(WallState state)
    {
        for(int i = 0; i < TemplateWallTypes.Length; i++)
        {
            TemplateWallTypes[i]?.SetActive(i == (int)state);
        }

    }
}
