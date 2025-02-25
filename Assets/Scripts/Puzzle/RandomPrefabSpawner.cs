using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPrefabSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn; 
    [SerializeField] private List<Transform> spawnLocations; 
    [SerializeField] private int spawnCount = 5;

    void Start()
    {
        SpawnPrefabs();
    }

    void SpawnPrefabs()
    {
        if (spawnLocations.Count == 0 || prefabToSpawn == null)
        {
            Debug.LogWarning("No spawn locations or prefab assigned!");
            return;
        }

        List<Transform> availableLocations = new List<Transform>(spawnLocations);

        for (int i = 0; i < spawnCount && availableLocations.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableLocations.Count);
            Transform spawnPoint = availableLocations[randomIndex];

            Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

            availableLocations.RemoveAt(randomIndex);
        }
    }
}
