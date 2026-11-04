using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float playerRadius;

    [SerializeField] private GameObject coinPrefab;

    [SerializeField] private int maxCoins;
    [SerializeField] private int areaX;
    [SerializeField] private int areaY;
    [SerializeField] private float interval;

    private float _duration;
    private int _totalSpawnedCoins;

    private void Start()
    {
        _duration = 0;
        _totalSpawnedCoins = 0;
    }

    private void Update()
    {
        if (transform.childCount >= maxCoins) return;

        _duration += Time.deltaTime;

        if(_duration >= interval)
        {
            PeriodicCoinsSpawn();
            _duration = 0;
        }
    }

    private void PeriodicCoinsSpawn()
    {
        Spawn();
        _totalSpawnedCoins++;
    }

    private void Spawn(GameObject specificCoin = null)
    {
        Vector3 spawnPos = Vector3.zero;
        GameObject coinToSpawn = specificCoin == null ? coinPrefab : specificCoin;

        while (true)
        {
            float x = Random.Range(-areaX, areaX + 1);
            float z = Random.Range(-areaY, areaY + 1);
            spawnPos = new Vector3(x, 0.5f, z);

            if (Vector3.Distance(spawnPos, player.position) >= playerRadius)
                break;
        }

        Instantiate(coinToSpawn, spawnPos, Quaternion.identity, transform);
    }
}
