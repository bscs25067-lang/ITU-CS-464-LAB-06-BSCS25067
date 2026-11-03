using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float playerRadius;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject redCoinPrefab;
    [SerializeField] private GameObject blueCoinPrefab;

    [SerializeField] private int maxCoins;
    [SerializeField] private int area;
    [SerializeField] private float interval;

    public int CurrentSpawnedCoins {
        get
        {
            int count = 0;
            foreach(Transform t in transform)
            {
                if (t.GetComponent<Coin>().Type == Coin.CoinType.Default)
                    count++;
            }

            return count;
        }
    }

    private float _duration;
    private int _totalSpawnedCoins;
    private int _redCoinSpawnTime;

    private void Start()
    {
        _duration = 0;
        _totalSpawnedCoins = 0;

        _redCoinSpawnTime = Random.Range(2, 11);
    }

    private void Update()
    {
        if (CurrentSpawnedCoins >= maxCoins) return;

        _duration += Time.deltaTime;

        if(_duration >= interval)
        {
            PeriodicCoinsSpawn();
            _duration = 0;
        }
    }

    public void SpawnBlueCoins(int count)
    {
        for (int i = 0; i < count; i++)
            Spawn(blueCoinPrefab);
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
            float x = Random.Range(-area, area + 1);
            float z = Random.Range(-area, area + 1);
            spawnPos = new Vector3(x, 0.5f, z);

            if (_totalSpawnedCoins == _redCoinSpawnTime)
                coinToSpawn = redCoinPrefab;

            if (Vector3.Distance(spawnPos, player.position) >= playerRadius)
                break;
        }

        Instantiate(coinToSpawn, spawnPos, Quaternion.identity, transform);
    }
}
