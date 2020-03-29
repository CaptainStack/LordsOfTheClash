using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaOfEffect : MonoBehaviour
{
    public float radius = 1f;
    public Faction faction = Faction.Neutral;

    // Duration of the area of effect. 0 for instant
    public float aoeDuration = 0;
    // Frequency of the AOE action. 0 or less disables periodic updates
    public float aoeFrequency = 0;

    public Sound sound;
    private AudioSource audioSource;
    
    // Timers for effect expiration, and periodic updates
    private float expirationTime = 0;
    private float nextPeriodicUpdate = 0;

    // Start is called before the first frame update
    void Start()
    {

        if (sound.clip)
        {
            if (!audioSource)
                audioSource = this.gameObject.AddComponent<AudioSource>();

            audioSource.spatialize = true;
            audioSource.spatialBlend = .5f;
            sound.SetSource(audioSource);
            sound.PlaySound();
        }

        // Schedule first periodic update
        if (aoeFrequency > 0)
        {
            nextPeriodicUpdate = Time.time + aoeFrequency;
        }

        expirationTime = Time.time + aoeDuration;
        AreaOfEffectAction();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time > expirationTime)
        {
            if (!audioSource || !audioSource.isPlaying)
                Destroy(this.gameObject);
        }
        // Run action periodically, if enabled
        else if (aoeFrequency > 0 && Time.time >= nextPeriodicUpdate)
        {
            nextPeriodicUpdate = Time.time + aoeFrequency;
            AreaOfEffectAction();
        }
    }

    // Action for this area of effect
    protected abstract void AreaOfEffectAction();

    // Computes a list of friendly targets hit
    protected List<Unit> ComputeFriendlyTargets()
    {
        // bit mask that specifies this faction's layer
        int targetLayer = 1 << LayerMask.NameToLayer(faction.ToString());

        Collider2D[] collidersHit = radius > 0 ?
            Physics2D.OverlapCircleAll(this.transform.position, radius, targetLayer) :
            Physics2D.OverlapPointAll(this.transform.position, targetLayer);

        List<Unit> units = CollidersToUnits(collidersHit);

        // Filter out enemy targets missed via layer
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].faction != faction)
            {
                units.Remove(units[i]);
                i--;
            }
        }

        return units;
    }

    // Computes a list of enemy targets hit
    protected List<Unit> ComputeEnemyTargets()
    {
        // bit mask that specifies all layers other than this faction's layer
        int targetLayer = ~(1 << LayerMask.NameToLayer(faction.ToString()));

        Collider2D[] collidersHit = radius > 0 ?
            Physics2D.OverlapCircleAll(this.transform.position, radius, targetLayer) :
            Physics2D.OverlapPointAll(this.transform.position, targetLayer);

        List<Unit> units = CollidersToUnits(collidersHit);

        // Filter out friendly targets missed via layer
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].faction == faction)
            {
                units.Remove(units[i]);
                i--;
            }
        }

        return units;
    }

    // Turns a collection of colliders into a list of units attached to those colliders
    private List<Unit> CollidersToUnits(Collider2D[] colliders)
    {
        List<Unit> units = new List<Unit>();

        foreach (Collider2D collider in colliders)
        {
            Unit unit = collider.gameObject.GetComponent<Unit>();

            if (unit)
                units.Add(unit);
        }

        return units;
    }
}
