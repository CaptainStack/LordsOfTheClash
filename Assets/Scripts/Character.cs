using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Faction of the characters
public enum Faction { Neutral, Friendly, Enemy }

public class Character : MonoBehaviour
{
    public Rigidbody2D characterRigidBody;
    public CircleCollider2D characterCollider;
    public SpriteRenderer spriteRenderer;

    // Character stats
    public float health = 1;
    public float speed = 1f;
    public float range = 1f;
    public float attackSpeed = 1f;
    public Faction faction = Faction.Neutral;

    // Attack cooldown timer
    public float attackTimer = 1f;

    // Direction character is facing
    protected Vector3 lookDirection;

    // Character's vision circle, used to find enemies
    public TargetArea visionArea;
    public GameObject visionAreaGameObject;

    // The current enemy being attacked
    protected Character currentTarget;

    // Use this for initialization
    protected virtual void Start ()
    {
        // Add RigidBody2D
        if (!characterRigidBody)
        {
            characterRigidBody = this.gameObject.AddComponent<Rigidbody2D>();
            characterRigidBody.drag = 0.5f;
            characterRigidBody.gravityScale = 0.0f;
            characterRigidBody.freezeRotation = true;
        }

        // Add Collider
        if (!characterCollider)
        {
            characterCollider = this.gameObject.AddComponent<CircleCollider2D>();
            characterCollider.radius = 0.1f;
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
        // If hostile target is in range, attack them
        if (TargetInRange())
        {
            AttackTarget();
        }
        else // Otherwise move to nearest hostile
        {
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
        // Get the nearest target that doesn't match this character's faction and set it as current target
        currentTarget = visionArea.targetList.Find(x => x.faction != this.faction);
    }

    void AttackTarget()
    {
        // Stop moving
        this.characterRigidBody.velocity = Vector2.zero;

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
        Debug.Log("Character attacking target " + currentTarget);
    }

    // Move towards current target
    void MoveToTarget()
    {
        if (currentTarget != null)
        {
            Vector3 movementDir = (currentTarget.transform.position - this.transform.position).normalized;
            this.characterRigidBody.velocity = movementDir * speed;
            this.lookDirection = movementDir;
        }
    }
}
