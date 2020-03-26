using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Flying buff for a unit
public class FlyingUnitEffect : UnitEffect
{
    // What to do at the start of the effect
    protected override void OnEffectStart()
    {
        Debug.Log("flying effect start");
        // Flying can only be applied to ground units, not flying or building units
        if (unit.transform.position.z < 0 || unit.transform.position.z > 0)
        {
            Destroy(this);
        }

        // Apply flying to target by setting its z-depth to -1
        unit.transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y, -1f);
    }

    // What to do at the end of the effect
    protected override void OnEffectEnd()
    {
        // Remove flying from target by setting its z-depth to 0
        unit.transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y, 0f);
    }
}
