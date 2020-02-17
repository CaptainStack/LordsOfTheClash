using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float respawnFrequency;
    public Unit unitToSpawn;
    public Faction faction;
    public Vector3 spawnPositionOffset = new Vector3(0f, 0.1f);

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

        if (timeUntilNextSpawn <= 0f)
        {
            timeUntilNextSpawn = respawnFrequency;

            Vector3 spawnPosition = this.transform.position + spawnPositionOffset;
            Unit newUnit = Instantiate(unitToSpawn, spawnPosition, Quaternion.identity);
            newUnit.faction = faction;
        }
    }
}