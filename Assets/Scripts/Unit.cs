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

    // Is this unit a building, and should it only target units that are buildings
    public bool isBuilding = false;
    public bool onlyTargetBuildings = false;

    // Range of a unit's vision, used to find enemies
    public float visionRange = 20f;

    // The current enemy being attacked
    protected Unit currentTarget;

    // Attack cooldown timer
    private float attackTimer;

    // Timer objects for acquiring a target, so we don't spam it (expensive computation)
    private float acquireTargetTimer = 0f;
    private float acquireTargetCooldown = .33f;

    // UnitEffects active on this unit (DoT, HoT, Freeze, Stun, etc)
    public List<UnitEffect> activeEffects;

    // The number of requests to disable this unit's AI (for stun, freeze)
    private float disableAICount = 0f;

    // Use this for initialization
    protected virtual void Start ()
    {
        // Add RigidBody2D
        if (!unitRigidBody)
        {
            unitRigidBody = this.gameObject.AddComponent<Rigidbody2D>();
            unitRigidBody.drag = 5f;
            unitRigidBody.gravityScale = 0.0f;
            unitRigidBody.freezeRotation = true;
        }

        // Add Collider
        if (!unitCollider)
        {
            unitCollider = this.gameObject.AddComponent<CircleCollider2D>();
            unitCollider.radius = 0.09f;
        }

        // Add Sprite
        if (!spriteRenderer)
            spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();

        InitializeUnitFaction();
    }

    // Sets the sprite color and layer mask for this unit's faction
    void InitializeUnitFaction()
    {
        switch (faction)
        {
            case Faction.Friendly:
                spriteRenderer.color = Color.green;
            break;
            case Faction.Neutral:
                spriteRenderer.color = Color.yellow;
            break;
            case Faction.Enemy:
                spriteRenderer.color = Color.red;
            break;
        }
        this.gameObject.layer = LayerMask.NameToLayer(faction.ToString());
    }

	// Update is called once per frame
	void Update ()
    {
        // If dead, destroy self
        if (health <= 0f)
        {
            Destroy(this.gameObject);
        }
    }

    // FixedUpdate runs synchronized with Unity physics cycle
    void FixedUpdate()
    {
        // Check if Unit AI has been disabled
        if (disableAICount > 0)
        {
            currentTarget = null;
            DisableSpawners();
            return;
        }
        EnableSpawners(); // Enable spawners, in case they were disabled above

        // If hostile target is in range, attack them
        if (TargetInRange())
        {
            FightTarget();
        }
        // Otherwise find a hostile target
        else         
        {
            AcquireTarget();
            MoveToTarget();
        }
    }

    // Move towards current target
    void MoveToTarget()
    {
        if (currentTarget != null)
        {
            Vector3 movementDir = (currentTarget.transform.position - this.transform.position).normalized;

            // Add a small amount of random side-to-side movement for better bunching (units form crowds instead of lines)
            // (this also makes unit movement feel more organic)
            Vector3 normal = Vector3.Cross(movementDir, Vector3.forward);
            movementDir += normal * Random.Range(-1f, 1f);
            movementDir = movementDir.normalized;

            // Accelerate in direction, up to max speed
            if (this.unitRigidBody.velocity.magnitude < speed)
                this.unitRigidBody.velocity += (Vector2)movementDir * .1f;
        }
    }

    // Checks if the current target is alive and in range
    bool TargetInRange()
    {
        if (currentTarget == null)
            return false;

        // Compute distance to target, adjusting for unit radius and scale factor
        float distanceToTarget = (this.transform.position - currentTarget.transform.position).magnitude;
        distanceToTarget -= this.unitCollider.radius * this.transform.localScale.x;
        distanceToTarget -= currentTarget.unitCollider.radius * currentTarget.transform.localScale.x;

        return attackRange >= distanceToTarget; // check if in range
    }

    // Acquires a target
    void AcquireTarget()
    {
        acquireTargetTimer -= Time.deltaTime;

        if (acquireTargetTimer <= 0f)
        {
            acquireTargetTimer = acquireTargetCooldown;

            // Get the nearest target that doesn't match this unit's faction and set it as current target
            float closestDistance = float.MaxValue;
            currentTarget = null;

            // bit mask that specifies all layers other than this faction's layer
            int targetLayer = ~(1 << this.gameObject.layer);

            // loops through all colliders in this unit's vision circle
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(this.transform.position, visionRange, targetLayer))
            {
                Unit unit = collider.gameObject.GetComponent<Unit>();

                if (IsValidTarget(unit))
                {
                    float distance = (this.transform.position - unit.transform.position).sqrMagnitude;

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        currentTarget = unit;
                    }
                }
            }
        }
    }

    // Check if the given unit is a valid target for this unit
    protected virtual bool IsValidTarget(Unit target)
    {
        if (!target)
            return false;
        
        if (onlyTargetBuildings && !target.isBuilding)
            return false;

        return true;
    }

    // Fight the current target
    void FightTarget()
    {
        // Attack with a cooldown timer
        if (attackCooldown > 0f)
        {
            if (Time.time > attackTimer)
            {
                attackTimer = Time.time + attackCooldown;
                Attack();
            }
        }
    }

    // Attack current target
    protected virtual void Attack()
    {
        // No-op, implement attack behavior in derived class
    }

    // Applies an active effect (buff or debuff) to this unit
    public void ApplyEffect(UnitEffect effect)
    {
        effect.unit = this;
        effect.transform.parent = this.transform;
        activeEffects.Append(effect);
    }

    // Disables unit AI until ResumeAI has been called the same number of times
    public void DisableAI()
    {
        disableAICount++;
    }

    // Resumes the unit AI, but must be called the same number of times as DisableAI
    public void ResumeAI()
    {
        if (disableAICount == 0)
        {
            Debug.Log("Error: ResumeAI called before DisableAI");
            return;
        }

        disableAICount--;
    }

    // Disables all spawner components attached to this unit
    private void DisableSpawners()
    {
        Spawner[] spawners = this.gameObject.GetComponents<Spawner>();

        foreach (Spawner spawner in spawners)
        {
            spawner.Disable();
        }
    }

    // Enables all spawner components attached to this unit
    private void EnableSpawners()
    {
        Spawner[] spawners = this.gameObject.GetComponents<Spawner>();

        foreach (Spawner spawner in spawners)
        {
            spawner.Enable();
        }
    }
}
