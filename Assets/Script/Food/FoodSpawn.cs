using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawn : MonoBehaviour {

    [SerializeField] Food foodPrefab;
    [SerializeField] float minFoodSpawnRate;
    [SerializeField, Range(0f, 100f)] float maxFoodSpawnRate;

    [SerializeField] BoxCollider2D levelBox;

    float timeToSpawn;
    float timer = 0f;

    private void Start()
    {
        timeToSpawn = (minFoodSpawnRate <= 0f) ? maxFoodSpawnRate : Random.Range(minFoodSpawnRate, maxFoodSpawnRate);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToSpawn)
        {
            SpawnFood();
            timer = 0f;

            timeToSpawn = (minFoodSpawnRate <= 0f) ? maxFoodSpawnRate : Random.Range(minFoodSpawnRate, maxFoodSpawnRate);
        }
    }

    void SpawnFood()
    {
        Vector3 attemptedSpawnLocation = new Vector3();
        while (!AttemptSpawnLocation(ref attemptedSpawnLocation))
        {
        }
        Instantiate(foodPrefab, attemptedSpawnLocation, Quaternion.identity);
    }

    bool AttemptSpawnLocation(ref Vector3 spawnLocation)
    {
        Bounds boxBounds = levelBox.bounds;
        Vector3 attemptedSpawnLocation = new Vector3(
            Random.Range(boxBounds.min.x, boxBounds.max.x),
            Random.Range(boxBounds.min.y, boxBounds.max.y),
            0f);

        RaycastHit2D[] hits = Physics2D.CircleCastAll(attemptedSpawnLocation, 1f, Vector2.zero);
        if (hits.Length > 1)
        {
            return false;
        }

        spawnLocation = attemptedSpawnLocation;
        return true;
    }
}