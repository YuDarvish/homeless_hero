using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {

    [SerializeField] GameObject objectToSpawn;
    [SerializeField] float minSpawnRate;
    [SerializeField, Range(0f, 100f)] float maxSpawnRate;
    [SerializeField] bool canOnlyHaveOne;

    bool objectInScene = false;
    [SerializeField] List<BoxCollider2D> spawnBoxes;
    [SerializeField] List<Collider2D> ignoredBoxes;

    float timeToSpawn;
    float timer = 0f;

    private void Start()
    {
        timeToSpawn = (minSpawnRate <= 0f) ? maxSpawnRate : Random.Range(minSpawnRate, maxSpawnRate);
    }

    private void Update()
    {
        UpdateTimer();
    }

    void UpdateTimer()
    {
        if (canOnlyHaveOne && objectInScene) return;

        timer += Time.deltaTime;
        if (timer >= timeToSpawn)
        {
            Spawn();
            timer = 0f;

            timeToSpawn = (minSpawnRate <= 0f) ? maxSpawnRate : Random.Range(minSpawnRate, maxSpawnRate);
        }
    }

    void Spawn()
    {
        Vector3 attemptedSpawnLocation = new Vector3();
        while (!AttemptSpawnLocation(ref attemptedSpawnLocation))
        {
        }
        GameObject spawnedObject = Instantiate(objectToSpawn, attemptedSpawnLocation, Quaternion.identity);
        
        if (canOnlyHaveOne)
        {
            objectInScene = true;
            CollectableEventFirer eventFirer = spawnedObject.GetComponent<CollectableEventFirer>();
            if (eventFirer)
            {
                eventFirer.onCollection += OnObjectCollected;
            }
        }
    }

    bool AttemptSpawnLocation(ref Vector3 spawnLocation)
    {
        BoxCollider2D randomSpawnBox = spawnBoxes[Random.Range(0, spawnBoxes.Count)];
        Bounds boxBounds = randomSpawnBox.bounds;

        Vector3 attemptedSpawnLocation = new Vector3(
            Random.Range(boxBounds.min.x, boxBounds.max.x),
            Random.Range(boxBounds.min.y, boxBounds.max.y),
            0f);

        RaycastHit2D[] hits = Physics2D.CircleCastAll(attemptedSpawnLocation, 1f, Vector2.zero);
        foreach (RaycastHit2D hit in hits)
        {
            if (!spawnBoxes.Contains((BoxCollider2D)hit.collider) &&
                !ignoredBoxes.Contains(hit.collider))
            {
                return false;
            }
        }

        spawnLocation = attemptedSpawnLocation;
        return true;
    }

    void OnObjectCollected()
    {
        objectInScene = false;
    }
}