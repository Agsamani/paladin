using NUnit.Framework.Constraints;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject PlayerPrefab;

    private GameObject _player;

    private void Awake()
    {
        _player = Instantiate(PlayerPrefab, transform);
        _player.transform.parent = null;
    }

    private void Start()
    {
         PlayerManager.instance.RegisterPlayer(_player);
    }

    void Update()
    {
        
    }
}
