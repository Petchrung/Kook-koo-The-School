
using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{
    [Header("Items to Spawn")]
    public GameObject[] itemPrefabs;

    [Header("Spawn Locations")]
    public Transform[] spawnPoints;

    [Header("Settings")]
    public int maxSpawnCount = 5;

    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        if (itemPrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Please assign items and spawn points!");
            return;
        }

        int count = Mathf.Min(maxSpawnCount, spawnPoints.Length);

        // สุ่มตำแหน่งเกิด
        for (int i = 0; i < count; i++)
        {
            int randomPoint = Random.Range(i, spawnPoints.Length);

            Transform temp = spawnPoints[i];
            spawnPoints[i] = spawnPoints[randomPoint];
            spawnPoints[randomPoint] = temp;

            // สุ่มไอเทม
            int randomItem = Random.Range(0, itemPrefabs.Length);

            Instantiate(
                itemPrefabs[randomItem],
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );
        }
    }
}
