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
    public float range = 1f;
    public float attackSpeed = 1f;
    public Faction faction = Faction.Neutral;

    // Attack cooldown timer
    public float attackTimer = 1f;

    // Direction unit is facing
    protected Vector3 lookDirection;

    // Unit's vision circle, used to find enemies
    public TargetArea visionArea;
    public GameObject visionAreaGameObject;

    // The current enemy being attacked
    protected Unit currentTarget;

    // Use this for initialization
    protected virtual void Start ()
    {
        // Add RigidBody2D
        if (!unitRigidBody)
        {
            unitRigidBody = this.gameObject.AddComponent<Rigidbody2D>();
            unitRigidBody.drag = 0.5f;
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

        // Add Aggro target area
        if (!visionAreaGameObject)
        {
            visionAreaGameObject = new GameObject("visionArea");
            visionArea = visionAreaGameObject.AddComponent<TargetArea>();

            // Set gameobject parent to this object
            visionAreaGameObject.transform.parent = this.gameObject.transform;
        }
    }

	// Update is called once per frame
	protected virtual void Update ()
    {
        // If dead, destroy self
        if (health <= 0f)
        {
            Destroy(this);
        }
        // Else if hostile target is in range, attack them
        else if (TargetInRange())
        {
            AttackTarget();
        }
        // Otherwise move to nearest hostile
        else         {
            AcquireTarget();
            MoveToTarget();
        }
    }

    // Checks if the current target is alive and in range
    bool TargetInRange()
    {
        return currentTarget != null
            && currentTarget.health >= 0f
            && range >= (this.transform.position - currentTarget.transform.position).magnitude; // check if in range
    }

    // Acquires a target
    void AcquireTarget()
    {
        // Get the nearest target that doesn't match this unit's faction and set it as current target
        currentTarget = visionArea.targetList.Find(x => x.faction != this.faction);
    }

    void AttackTarget()
    {
        // Stop moving
        this.unitRigidBody.velocity = Vector2.zero;

        // Turn toward target
        this.lookDirection = (this.transform.position - currentTarget.transform.position).normalized;

        // Attack target
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0.0f)
        {
            attackTimer = attackSpeed;
            Attack();
        }
    }

    protected virtual void Attack()
    {
        Debug.Log("Unit attacking target " + currentTarget);
    }

    // Move towards current target
    void MoveToTarget()
    {
        if (currentTarget != null)
        {
            Vector3 movementDir = (currentTarget.transform.position - this.transform.position).normalized;
            this.unitRigidBody.velocity = movementDir * speed;
            this.lookDirection = movementDir;
        }
    }
}
