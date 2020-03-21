using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaOfEffect : MonoBehaviour
{
    public float radius = .1f;
    public Faction faction = Faction.Neutral;


    // Start is called before the first frame update
    void Start()
    {
        // Apply area of effect, then destroy this gameobject
        AreaOfEffectAction();
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Action for this area of effect
    protected abstract void AreaOfEffectAction();

    // Computes a list of friendly targets hit
    protected List<Unit> ComputeFriendlyTargets()
    {
        // bit mask that specifies this faction's layer
        int targetLayer = 1 << LayerMask.NameToLayer(faction.ToString());

        Collider2D[] collidersHit = Physics2D.OverlapCircleAll(this.transform.position, radius, targetLayer);

        return CollidersToUnits(collidersHit);
    }

    // Computes a list of enemy targets hit
    protected List<Unit> ComputeEnemyTargets()
    {
        // bit mask that specifies all layers other than this faction's layer
        int targetLayer = ~(1 << LayerMask.NameToLayer(faction.ToString()));

        Collider2D[] collidersHit = Physics2D.OverlapCircleAll(this.transform.position, radius, targetLayer);

        return CollidersToUnits(collidersHit);
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
