using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Faction of the units
public enum Faction { Neutral, Friendly, Enemy }

public class Unit : MonoBehaviour
{
    public Rigidbody2D unitRigidBody;
    public CircleCollider2D unitCollider;
    public SpriteRenderer spriteRenderer;

    // Unit stats
    public float health = 1;
    public float speed = 1f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public Faction faction = Faction.Neutral;

    // Attack cooldown timer
    private float attackTimer;

    // Range of a unit's vision, used to find enemies
    public float visionRange = 20f;

    // The current enemy being attacked
    protected Unit currentTarget;

    // Timer objects for acquiring a target, so we don't spam it (expensive computation)
    private float acquireTargetTimer = 0f;
    private float acquireTargetCooldown = .5f;

    // Use this for initialization
    protected virtual void Start ()
    {
        // Add RigidBody2D
        if (!unitRigidBody)
        {
            unitRigidBody = this.gameObject.AddComponent<Rigidbody2D>();
            unitRigidBody.drag = 3f;
            unitRigidBody.gravityScale = 0.0f;
            unitRigidBody.freezeRotation = true;
        }

        // Add Collider
        if (!unitCollider)
        {
            unitCollider = this.gameObject.AddComponent<CircleCollider2D>();
            unitCollider.radius = 0.1f;
        }

        // Add Sprite
        if (!spriteRenderer)
            spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
    }

	// Update is called once per frame
	void Update ()
    {
        // If dead, destroy self
        if (health <= 0f)
        {
            Destroy(this.gameObject);
        }
        // Else if hostile target is in range, attack them
        else if (TargetInRange())
        {
            FightTarget();
        }
        // Otherwise find a hostile target
        else         
        {
            AcquireTarget();
        }
    }

    // FixedUpdate runs synchronized with Unity physics cycle
    void FixedUpdate()
    {
        // If we have a target, but they are out of range, move towards them
        if (currentTarget && !TargetInRange())
            MoveToTarget();
    }

    // Move towards current target
    void MoveToTarget()
    {
        if (currentTarget != null)
        {
            Vector3 movementDir = (currentTarget.transform.position - this.transform.position).normalized;

            // Accelerate in direction, up to max speed
            if (this.unitRigidBody.velocity.magnitude < speed)
                this.unitRigidBody.velocity += (Vector2)movementDir * .1f;
        }
    }

    // Checks if the current target is alive and in range
    bool TargetInRange()
    {
        return currentTarget != null
            && currentTarget.health >= 0f
            && attackRange >= (this.transform.position - currentTarget.transform.position).magnitude; // check if in range
    }

    // Acquires a target
    void AcquireTarget()
    {
        acquireTargetTimer -= Time.deltaTime;

        if (acquireTargetTimer <= 0f)
        {
            acquireTargetTimer = acquireTargetCooldown;

            // Get the nearest target that doesn't match this unit's faction and set it as current target
            float closestDistance = currentTarget ? (this.transform.position - currentTarget.transform.position).sqrMagnitude : float.MaxValue;

            // loops through all colliders in this unit's vision circle
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(this.transform.position, visionRange))
            {
                Unit unit = collider.gameObject.GetComponent<Unit>();

                if (unit && unit.faction != this.faction)
                {
                    float distance = (this.transform.position - unit.transform.position).sqrMagnitude;

                    if (distance < closestDistance)
                    {
                        distance = closestDistance;
                        currentTarget = unit;
                    }
                }
            }
        }
    }

    // Fight the current target
    void FightTarget()
    {
        // Attack with a cooldown timer
        if (attackCooldown > 0f)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0.0f)
            {
                attackTimer = attackCooldown;
                Attack();
            }
        }
    }

    // Attack current target
    protected virtual void Attack()
    {
        Debug.Log("Unit attacking target " + currentTarget);
    }
}
