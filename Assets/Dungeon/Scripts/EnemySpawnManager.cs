using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawnManager : MonoBehaviour
{
    private int _maxX;
    private int _maxZ;
    private int[] _grid;
    private float _roomOffset;

    private GameObject _player;

    public int MaxEnemyCount = 5;
    public float SpawnInterval = 10.0f;
    public float PlayerDistanceThresh = 20.0f;
    public float EnemyDistanceThresh = 2.0f;
    public float EnemyY = 0.0f;

    private float _t = 0.0f;

    [SerializeField]
    private SkellyMain _skellyPrefab;
    private ObjectPool<SkellyMain> _pool;
    private List<SkellyMain> _enemies;


    private Vector3 _nextPos;
    private Quaternion _nextRot;
    
    //Todo: put a try frame limit 

    private void Awake()
    {
        _pool = new ObjectPool<SkellyMain>(CreateEnemy, OnEnemyGet, OnEnemyRelease);
        _enemies = new List<SkellyMain>();
    }

    private void OnEnemyGet(SkellyMain s)
    {
        s.gameObject.SetActive(true);
        s.transform.position = _nextPos;
        s.transform.rotation = _nextRot;
    }
    private void OnEnemyRelease(SkellyMain s)
    {

        s.gameObject.SetActive(false);
    }

    private SkellyMain CreateEnemy()
    {
        SkellyMain e = Instantiate(_skellyPrefab);
        e.SetPool(_pool);
        _enemies.Add(e);
        return e;
    }

    public void SetGridData(int x, int z, int[] grid, float roomOffset)
    {
        _maxX = x;
        _maxZ = z;
        _grid = grid;
        _roomOffset = roomOffset;
    }

    bool IsValidForSpawn(int x, int z)
    {
        if (_grid[z + _maxZ * x] != (int)DungeonGenerator.RoomType.Normal) return false;

        if ((IndexToPos(x, z) - _player.transform.position).sqrMagnitude <= PlayerDistanceThresh * PlayerDistanceThresh) return false;

        if (_pool.CountActive >= MaxEnemyCount) return false;

        foreach(SkellyMain s in _enemies)
        {
            if (!s.IsDead() &&
                (IndexToPos(x, z) - s.gameObject.transform.position).sqrMagnitude <= EnemyDistanceThresh * EnemyDistanceThresh) return false; 
        }

        return true;

    }

    bool TryToSpawn()
    {
        int x = Random.Range(0, _maxX);
        int z = Random.Range(0, _maxZ);
        if(IsValidForSpawn(x, z))
        {
            _nextPos = IndexToPos(x, z);
            float pitch = Random.Range(0, 360); // X
            float yaw = Random.Range(0, 360);   // Y
            _nextRot = Quaternion.Euler(pitch, yaw, 0f);

            _pool.Get();
            return true;
        } else
        {
            return false;
        }
    }


    Vector3 IndexToPos(int x, int z)
    {
        return new Vector3((x + 0.5f) * _roomOffset, EnemyY, (z + 0.5f) * _roomOffset);
    }
    void Start()
    {
        _player = PlayerManager.instance.Player;
    }

    void Update()
    {
        if (_t >= SpawnInterval)
        {
            if(TryToSpawn())
            {
                _t = 0.0f;
            }
        }
        _t += Time.deltaTime;
    }


}
