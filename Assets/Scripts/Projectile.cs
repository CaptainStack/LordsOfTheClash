using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 1f;

    // Explosion triggered when target is reached
    public Explosion explosionPrefab;

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
        if (collider.OverlapPoint(target))
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
        Explosion newExplosion = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        newExplosion.faction = this.faction;

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