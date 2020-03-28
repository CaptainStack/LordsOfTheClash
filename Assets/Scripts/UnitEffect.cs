using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An active effect on a unit (buff or debuff)
public class UnitEffect : MonoBehaviour
{

    // Duration of the effect. 0 for instant
    public float duration = 0;

    // If true, the effect does not expire (this overrides duration)
    public bool permanent = false;

    // Frequency of the effect. 0 or less disables periodic updates
    public float frequency = 0;
    
    // Target Unit the effect acts upon
    protected Unit unit;

    // Timers for effect expiration, and periodic updates
    protected float expirationTime = 0;
    private float nextPeriodicUpdate = 0;

    void Start()
    {
        unit = gameObject.GetComponent<Unit>();
        if (!unit)
        {
            Debug.Log("Error: UnitEffect component not attached to a unit");
            Destroy(this.gameObject);
        }

        // Schedule first periodic update
        if (frequency > 0)
        {
            nextPeriodicUpdate = Time.time + frequency;
        }

        expirationTime = Time.time + duration;
        OnEffectStart();
    }

    void Update()
    {
        // End expiring effects
        if (!permanent && Time.time > expirationTime)
        {
            EndEffect();
        }

        // Periodic effects
        if (frequency > 0 && Time.time >= nextPeriodicUpdate)
        {
            nextPeriodicUpdate = Time.time + frequency;
            OnEffectUpdate();
        }
    }

    // Removes the effect from the target unit and destroys the effect
    public void EndEffect()
    {
        OnEffectEnd();
        Destroy(this);
    }

    // What to do at the start of the effect
    protected virtual void OnEffectStart()
    {
        // No-op. Implement in derived class
    }

    // Periodically gets called to update the effect (like for a DoT)
    protected virtual void OnEffectUpdate()
    {
        // No-op. Implement in derived class
    }

    // What to do at the end of the effect
    protected virtual void OnEffectEnd()
    {
        // No-op. Implement in derived class
    }
}
