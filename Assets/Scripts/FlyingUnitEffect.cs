using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Flying buff for a unit
public class FlyingUnitEffect : UnitEffect
{
    // What to do at the start of the effect
    protected override void OnEffectStart()
    {
        // Only one flying effect at a time
        FlyingUnitEffect[] flyingUnitEffects = unit.GetComponents<FlyingUnitEffect>();
        foreach (FlyingUnitEffect flyingEffect in flyingUnitEffects)
        {
            if (flyingEffect == this)
                continue;
            else if (permanent || expirationTime > flyingEffect.expirationTime)
                flyingEffect.EndEffect();
            else
                Destroy(this);
        }

        // Apply flying to target by setting its z-depth to -1 and moving it to the "Flying" layer
        unit.transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y, -1f);
        unit.gameObject.layer = LayerMask.NameToLayer("Flying");
    }

    // What to do at the end of the effect
    protected override void OnEffectEnd()
    {
        // Remove flying from target by setting its z-depth to 0 and resetting its faction layer
        unit.transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y, 0f);
        unit.gameObject.layer = unit.GetFactionLayer();
    }
}
