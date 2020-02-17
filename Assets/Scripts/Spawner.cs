using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float respawnFrequency;
    public Unit unitToSpawn;

    private float timeUntilNextSpawn;

    // Start is called before the first frame update
    void Start()
    {
        timeUntilNextSpawn = respawnFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        timeUntilNextSpawn -= Time.deltaTime;

        if (timeUntilNextSpawn <= 0f) {
            Vector3 spawnPosition = this.transform.position;
            Instantiate(unitToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}