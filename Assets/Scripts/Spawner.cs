using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Mirror.NetworkBehaviour
{
    // How long between respawns
    public float respawnCooldown;

    // Type of unit to spawn
    public Unit unitToSpawn;

    // Faction fo units to spawn
    public Faction faction;

    // Offset from spawner's position to place spawned unit
    private  Vector3 spawnPositionOffset = new Vector3(0f, 0.3f);

    // Time remaining until next spawn
    private float timeUntilNextSpawn;
    private bool isEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        timeUntilNextSpawn = respawnCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer && isEnabled)
        {
            timeUntilNextSpawn -= Time.deltaTime;

            if (timeUntilNextSpawn <= 0f)
            {
                // Next spawn time, plus a tiny amount of variance (distributes engine processing load from pre-placed spawners)
                timeUntilNextSpawn = respawnCooldown + Random.Range(-.05f, .05f);

                // Slightly offset spawn position to prevent units from "stacking"
                Vector3 randomOffset = new Vector3(Random.Range(-.2f, .2f), Random.Range(-.2f, .2f), 0f);

                Vector3 spawnPosition = this.transform.position + spawnPositionOffset;
                Unit newUnit = Instantiate(unitToSpawn, spawnPosition, Quaternion.identity);
                newUnit.faction = faction;

                Mirror.NetworkServer.Spawn(newUnit.gameObject);
            }
        }
    }

    public void Enable()
    {
        isEnabled = true;
    }

    public void Disable()
    {
        isEnabled = false;
    }
}