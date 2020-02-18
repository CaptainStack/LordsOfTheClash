using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float damage = 1f;
    public float radius = .1f;
    public float impactForce = 0f;
    public Faction faction = Faction.Neutral;

    // Start is called before the first frame update
    void Start()
    {
        Explode();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Explode()
    {
        // Find targets in AOE
        Collider2D[] collidersHit = Physics2D.OverlapCircleAll(this.transform.position, radius);

        foreach (Collider2D collider in collidersHit)
        {
            // Apply damage and impact force to units
            Unit unit = collider.gameObject.GetComponent<Unit>();
            if (unit && unit.faction != this.faction)
            {
                unit.health -= damage;

                if (impactForce > 0f)
                {
                    Vector3 impactForceDirection = (unit.transform.position - this.transform.position).normalized;
                    unit.unitRigidBody.AddForce(impactForce * impactForceDirection);
                }
            }
        }

        // Destroy projectile
        Destroy(this.gameObject);
    }
}
