using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // How long between respawns
    public float respawnCooldown;

    // Type of unit to spawn
    public Unit unitToSpawn;

    // Offset from spawner's position to place spawned unit
    public Vector3 spawnPositionOffset = new Vector3(0f, 0.1f);

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
        if (isEnabled)
        {
            timeUntilNextSpawn -= Time.deltaTime;

            if (timeUntilNextSpawn <= 0f)
            {
                // Next spawn time, plus a tiny amount of variance (distributes engine processing load from pre-placed spawners)
                timeUntilNextSpawn = respawnCooldown + Random.Range(0f, .05f);

                Vector3 spawnPosition = this.transform.position + spawnPositionOffset;
                Unit newUnit = Instantiate(unitToSpawn, spawnPosition, Quaternion.identity);
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