using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Faction of the characters
public enum Faction { Friendly, Neutral, Enemy }

public abstract class Character : MonoBehaviour
{
    public Rigidbody2D characterRigidBody;
    public CircleCollider2D characterCollider;
    public SpriteRenderer spriteRenderer;

    // Player stats
    public int hp;
    public float speed;
    public Faction faction;

    // Direction character is facing
    protected Vector3 lookDirection;

    // Character's aggro area, used to find enemies
    public TargetArea aggroArea;

    // Character's attack area, used to find enemies in attack range
    public TargetArea attackArea;

    // The current enemy being attacked
    protected Character currentTarget;

    // Abstract functions
    protected abstract void UpdateImpl ();
    protected abstract void StartImpl();

    // Use this for initialization
    void Start ()
    {
        // Add RigidBody2D
        characterRigidBody = this.gameObject.AddComponent<Rigidbody2D>();
        characterRigidBody.drag = 0.5f;
        characterRigidBody.gravityScale = 0.0f;
        characterRigidBody.freezeRotation = true;

        // Add Collider
        characterCollider = this.gameObject.AddComponent<CircleCollider2D>();
        characterCollider.radius = 0.1f;

        // Add Sprite
        spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();

        // Add Aggro and Attack areas
        aggroArea = this.gameObject.AddComponent<TargetArea>();
        attackArea = this.gameObject.AddComponent<TargetArea>();

        // Call into derived class start
        StartImpl();
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Find a target to attack
        FindTarget();

        // If currently targetting hostile, attack them. Otherwise move toward nearest hostile target
        if (currentTarget)
            AttackTarget();
        else
            MoveToHostile();

        // Call into derived class update
        UpdateImpl();
    }

    // Find a target to attack
    void FindTarget()
    {
        // If current target is still alive, nothing to do
        if (currentTarget && currentTarget.health >= 0f)
            return;

        currentTarget = null;

        // If in range of a hostile target, set them as current target
        List<Character> targetList = attackArea.targetList;
        Character target = targetList.Find(x => x.faction != this.faction);
        if (target != null)
            currentTarget = target;
    }

    // Run towards nearest hostile target
    void MoveToHostile()
    {
        targetList = aggroArea.targetList;
        target = targetList.Find(x => x.faction != this.faction);
        if (target != null)
        {
            Vector3 movementDir = (enemyTarget.gameObject.transform.position - this.gameObject.transform.position).normalized;
            this.characterRigidBody.velocity = movementDir * speed;
            this.lookDirection = movementDir;
        }
    }
}
