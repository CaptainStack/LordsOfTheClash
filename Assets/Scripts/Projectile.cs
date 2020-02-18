using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 1f;
    public float speed = 1f;

    // Projectile AOE radius
    public float radius = .1f;
    // Faction of the projectile to prevent friendly-fire
    public Faction faction;

    // Position projectile is targetting
    public Vector3 target;

    public Rigidbody2D rigidbody;
    public CircleCollider2D collider;
    public SpriteRenderer spriteRenderer;
    
    void Start()
    {
        // Add RigidBody2D
        if (!rigidbody)
        {
            rigidbody = this.gameObject.AddComponent<Rigidbody2D>();
            rigidbody.drag = 0.0f;
            rigidbody.gravityScale = 0.0f;
            rigidbody.freezeRotation = true;
        }

        // Add Collider
        if (!collider)
        {
            collider = this.gameObject.AddComponent<CircleCollider2D>();
            collider.radius = 0.3f;
            collider.isTrigger = true;
        }

        // Add Sprite
        if (!spriteRenderer)
            spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Explode if projectile has reached target
        if (rigidbody.OverlapPoint(target))
        {
            Explode();
        }
        // Otherwise, move towards target
        else
        {
            MoveToTarget();
        }
    }

    void Explode()
    {
        // Find targets in AOE
        Collider2D[] collidersHit = Physics2D.OverlapCircleAll(this.transform.position, radius);

        foreach (Collider2D collider in collidersHit)
        {
            // Apply damage to units
            Unit unit = collider.gameObject.GetComponent<Unit>();
            if (unit && unit.faction != this.faction)
            {
                unit.health -= damage;
            }
        }

        // Destroy projectile
        Destroy(this.gameObject);
    }

    // Move towards target
    void MoveToTarget()
    {
        Vector3 movementDir = (target - this.transform.position).normalized;
        rigidbody.velocity = movementDir * speed;
    }
}