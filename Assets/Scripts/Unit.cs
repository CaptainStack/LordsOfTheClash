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
    private float acquireTargetCooldown = .25f;

    // The current range at which the unit is searching for units
    private float currentSearchRange;


    // The number of requests to disable this unit's AI (for stun, freeze)
    private float disableAICount = 0f;

    // Keep track of squared measurements for faster vector sqrMagnitude comparisons (faster than regular magnitude)
    private float prevSpeed;
    private float sqrSpeed;
    private float prevRange;
    private float sqrRange;

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

        currentSearchRange = visionRange * .25f; // initial range for the unit to search for targets

        InitializeUnitFaction();
        InitializeUnitDepth();
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

    // Sets the z depth for this unit
    void InitializeUnitDepth()
    {
        if (isBuilding) // Buildings have foundations "below ground"
            transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
        else // Regular units walk on the ground, so 0f
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

	// Update is called once per frame
	void Update ()
    {
        // If dead, destroy self
        if (health <= 0f)
        {
            Destroy(this.gameObject);
        }

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
            movementTarget = Vector2.zero; // Stop moving once in range of the target
            FightTarget();
        }
        else
        {
            AcquireTarget();
        }
    }

    // FixedUpdate runs synchronized with Unity physics cycle
    void FixedUpdate()
    {
        // Check if Unit AI has been disabled
        if (disableAICount > 0)
            return;

        // Recompute sqrSpeed if unit speed changed
        if (speed != prevSpeed)
        {
            prevSpeed = speed;
            sqrSpeed = speed * speed;
        }

        // If hostile target is out of range, move toward them
        if (this.unitRigidBody.velocity.sqrMagnitude < sqrSpeed && !TargetInRange())
        {
            MoveToTarget();
        }
    }

    // Move towards current target
    Vector2 movementTarget = Vector2.zero;
    void MoveToTarget()
    {
        if (currentTarget != null)
            movementTarget = (Vector2)currentTarget.transform.position;
        
        if (!movementTarget.Equals(Vector2.zero))
        {

            // Accelerate in direction, up to max speed
            if (this.unitRigidBody.velocity.sqrMagnitude < sqrSpeed)
            {
                Vector3 movementDir = (movementTarget - (Vector2)this.transform.position).normalized;

                // Add a small amount of random side-to-side movement for better bunching (units form crowds instead of lines)
                // (this also makes unit movement feel more organic)
                Vector3 normal = Vector3.Cross(movementDir, Vector3.forward);
                movementDir += normal * Random.Range(-.33f, .33f);
                movementDir = movementDir.normalized;

                this.unitRigidBody.AddForce(movementDir * this.unitRigidBody.mass * .33f, ForceMode2D.Impulse);
            }
        }
    }

    // Checks if the current target is alive and in range
    bool TargetInRange()
    {
        if (currentTarget == null)
            return false;

        // Recompute sqrRange if attack range changed
        if (attackRange != prevRange)
        {
            prevRange = attackRange;
            sqrRange = attackRange * attackRange;
        }

        // Compute distance to target (squared), adjusting for unit radius and scale factor
        float sqrDistanceToTarget = ((Vector2)this.transform.position - (Vector2)currentTarget.transform.position).sqrMagnitude;
        float colliderRadiusAdjust = this.unitCollider.radius * this.transform.localScale.x;
        colliderRadiusAdjust += currentTarget.unitCollider.radius * currentTarget.transform.localScale.x;
        sqrDistanceToTarget -= colliderRadiusAdjust * colliderRadiusAdjust;

        return sqrRange >= sqrDistanceToTarget; // check if in range
    }

    // Acquires a target
    Collider2D[] visibleColliders = new Collider2D[1000]; // Used to store visible units' colliders when acquiring a target (avoid frequent alloc)
    void AcquireTarget()
    {
        acquireTargetTimer -= Time.deltaTime;

        if (acquireTargetTimer <= 0f)
        {
            // Next AcquireTarget time, plus a tiny amount of variance (distributes engine processing load)
            acquireTargetTimer = acquireTargetCooldown + Random.Range(-.25f, .25f);

            // Get the nearest target that doesn't match this unit's faction and set it as current target
            Unit closestTarget = null;
            float closestDistance = float.MaxValue;

            // bit mask that specifies all layers other than this faction's layer
            int targetLayer = ~(1 << this.gameObject.layer);

            // z depth of units to target (to exclude buildings or ground/flying units)
            float minDepth = float.MinValue;
            float maxDepth = onlyTargetBuildings ? -1f : float.MaxValue;

            // loops through all colliders in this unit's vision circle
            int numColliders = Physics2D.OverlapCircleNonAlloc(this.transform.position, Mathf.Min(currentSearchRange, visionRange), visibleColliders, targetLayer, minDepth, maxDepth);
            for (int i = 0; i < numColliders; i++)
            {
                Unit unit = visibleColliders[i].gameObject.GetComponent<Unit>();

                if (unit)
                {
                    float distance = (this.transform.position - unit.transform.position).sqrMagnitude;

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = unit;
                    }
                }
            }

            // Set currentTarget to closest target (if found), and adjust the vision search range
            if (closestTarget)
            {
                currentTarget = closestTarget;
                currentSearchRange = (closestTarget.transform.position - this.transform.position).magnitude;

                acquireTargetTimer *= 2; // Delay next target acquisition if this one was successful
            }
            else // If no target found in the search, search a little farther
            {
                // Increase search range faster at short range, slower at long (to keep search area deltas about the same)
                if (currentSearchRange < visionRange * .125f)
                    currentSearchRange += visionRange * .1f;
                if (currentSearchRange < visionRange * .25f)
                    currentSearchRange += visionRange * .05f;
                else if (currentSearchRange < visionRange * .75f)
                    currentSearchRange += visionRange * .025f;
                else
                    currentSearchRange += visionRange * .01f;
            }
        }
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
    bool spawnersEnabled = true;
    private void DisableSpawners()
    {
        if (spawnersEnabled)
        {
            Spawner[] spawners = this.gameObject.GetComponents<Spawner>();

            foreach (Spawner spawner in spawners)
            {
                spawner.Disable();
            }

            spawnersEnabled = false;
        }
    }

    // Enables all spawner components attached to this unit
    private void EnableSpawners()
    {
        if (!spawnersEnabled)
        {
            Spawner[] spawners = this.gameObject.GetComponents<Spawner>();

            foreach (Spawner spawner in spawners)
            {
                spawner.Enable();
            }
        }
    }
}
