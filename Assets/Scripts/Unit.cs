using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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

        currentSearchRange = visionRange * .2f; // initial range for the unit to search for targets

        InitializeUnitFaction();
        InitializeUnitDepth();
    }

    // Sets the sprite color and layer mask for this unit's faction
    public void InitializeUnitFaction()
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

        // Set faction layer, unless this is a flying unit (they have their own layer)
        if (this.gameObject.layer != LayerMask.NameToLayer("Flying"))
            this.gameObject.layer = GetFactionLayer();
    }

    public int GetFactionLayer()
    {
        return LayerMask.NameToLayer(faction.ToString());
    }

    // Sets the z depth for this unit
    protected virtual void InitializeUnitDepth()
    {
        if (this.GetComponent<FlyingUnitEffect>()) // Flying units are above ground / outward from screen, so they have negative z value
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
        if (currentTarget && currentTarget.faction != this.faction && TargetInRange())
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

        // If hostile target is out of range, move toward them (unless we're at max speed already)
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
                bool isFlying = transform.position.z == -1f; // Flying units should move directly toward target
                Vector3 movementDir = ((isFlying ? movementTarget : ComputeNextMoveStep()) - (Vector2)transform.position).normalized;

                // Add a small amount of random side-to-side movement for better bunching (units form crowds instead of lines)
                // (this also makes unit movement feel more organic)
                Vector3 cross = Vector3.Cross(movementDir, Vector3.forward);
                movementDir += cross * Random.Range(-.1f, .1f);
                movementDir = movementDir.normalized;

                this.unitRigidBody.AddForce(movementDir * this.unitRigidBody.mass * .5f, ForceMode2D.Impulse);
            }
        }
    }

    bool pathFresh = false; // If path needs recalculating
    NavMeshPath movePath; // Cached movement path
    int currentCorner = 0; // Current corner in movement path
    Vector2 prevMovementTarget; // Previous movementTarget used for pathfinding
    float nextPathfindingUpdate = 0f;
    float pathfindingUpdateFrequency = 1f;
    // Computes the position of the next movement step this unit should take
    Vector2 ComputeNextMoveStep()
    {
        // If close to target, just go toward it
        if (((Vector2)transform.position - movementTarget).sqrMagnitude < 1f)
            return movementTarget;

        if (movePath == null)
            movePath = new NavMeshPath();
        
        // Mark path dirty if the movement target has changed substantially
        if ((movementTarget - prevMovementTarget).sqrMagnitude < 2f)
        {
            pathFresh = false;
            prevMovementTarget = movementTarget;
        }

        // Update path if necessary
        if (!pathFresh || Time.time > nextPathfindingUpdate)
        {
            nextPathfindingUpdate = Time.time + pathfindingUpdateFrequency;

            NavMeshHit navMeshHit = new NavMeshHit();

            // Use SamplePosition to find exact start/end points on the navmesh

            Vector3 startPos = new Vector3(transform.position.x, 0, transform.position.y);
            if(NavMesh.SamplePosition(startPos, out navMeshHit, 1f, NavMesh.AllAreas))
                startPos = navMeshHit.position;

            Vector3 endPos = new Vector3(movementTarget.x, 0, movementTarget.y);
            if(NavMesh.SamplePosition(endPos, out navMeshHit, 1f, NavMesh.AllAreas))
                endPos = navMeshHit.position;

            pathFresh = NavMesh.CalculatePath(
                startPos,
                endPos, 
                NavMesh.AllAreas,
                movePath);
            
            if (pathFresh)
                currentCorner = 0; // If path updated, start at first corner for direction
        }

        if (currentCorner < movePath.corners.Count())
        {
            // If close to the current corner, advance to the next corner
            if (((Vector2)transform.position - new Vector2(movePath.corners[currentCorner].x, movePath.corners[currentCorner].z)).sqrMagnitude < .1f)
                currentCorner++;
        }

        if (currentCorner < movePath.corners.Count())
        {
            // Return the next corner position 
            return new Vector2(movePath.corners[currentCorner].x, movePath.corners[currentCorner].z);
        }
        else 
        {
            // Otherwise don't move
            return (Vector2)transform.position;
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
            int targetLayer = ~(1 << GetFactionLayer());

            // z depth of units to target (to exclude buildings or ground/flying units)
            float minDepth = OnlyTargetBuildings() ? 1f : (CanTargetFlying() ? -1f : 0f);
            float maxDepth = float.MaxValue;

            // loops through all colliders in this unit's vision circle
            int numColliders = Physics2D.OverlapCircleNonAlloc(this.transform.position, Mathf.Min(currentSearchRange, visionRange), visibleColliders, targetLayer, minDepth, maxDepth);
            for (int i = 0; i < numColliders; i++)
            {
                Unit unit = visibleColliders[i].gameObject.GetComponent<Unit>();

                if (unit && unit.faction != faction)
                {
                    float distance = ((Vector2)this.transform.position - (Vector2)unit.transform.position).sqrMagnitude;

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
            else if (currentSearchRange >= visionRange)
            {
                acquireTargetTimer *= 4; // Delay the next search if no target found in max vision range
            }
            else // If no target found in the search, search a little farther
            {
                currentSearchRange += visionRange * .1f;
            }
        }
    }

    // Whether this unit can target flying units or not
    protected virtual bool CanTargetFlying()
    {
        return false;
    }

    // Returns true if this unit can only target buildings
    protected virtual bool OnlyTargetBuildings()
    {
        return false;
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
            foreach (Spawner spawner in this.gameObject.GetComponents<Spawner>())
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
            foreach (Spawner spawner in this.gameObject.GetComponents<Spawner>())
            {
                spawner.Enable();
            }

            spawnersEnabled = true;
        }
    }
}
