using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // How long between respawns
    public float respawnCooldown;

    // Type of unit to spawn
    public Unit unitToSpawn;

    // Faction of spawned units
    public Faction faction;

    // Offset from spawner's position to place spawned unit
    public Vector3 spawnPositionOffset = new Vector3(0f, 0.1f);

    // Time remaining until next spawn
    private float timeUntilNextSpawn;

    // Start is called before the first frame update
    void Start()
    {
        timeUntilNextSpawn = respawnCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        timeUntilNextSpawn -= Time.deltaTime;

        if (timeUntilNextSpawn <= 0f)
        {
            timeUntilNextSpawn = respawnCooldown;

            Vector3 spawnPosition = this.transform.position + spawnPositionOffset;
            Unit newUnit = Instantiate(unitToSpawn, spawnPosition, Quaternion.identity);
            newUnit.faction = faction;
        }
    }
}