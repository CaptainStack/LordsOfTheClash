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

    public Rigidbody2D rigidbody2d;
    public SpriteRenderer spriteRenderer;

    private float detonationTimer = 0f;
    
    void Start()
    {
        // Add RigidBody2D
        if (!rigidbody2d)
        {
            rigidbody2d = this.gameObject.AddComponent<Rigidbody2D>();
            rigidbody2d.drag = 0.0f;
            rigidbody2d.gravityScale = 0.0f;
            rigidbody2d.freezeRotation = true;
            rigidbody2d.isKinematic = true;
        }

        // Add Sprite
        if (!spriteRenderer)
            spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();

        MoveToTarget();
    }

    void Update()
    {
        // Explode if projectile has reached target
        if (Time.time > detonationTimer)
        {
            Explode();
        }
    }

    // FixedUpdate runs synchronized with Unity physics cycle
    void FixedUpdate()
    {
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
        Vector3 movementVector = target - this.transform.position; 
        Vector3 direction = (movementVector).normalized;
        rigidbody2d.velocity = direction * speed;

        detonationTimer = Time.time + movementVector.magnitude / speed;
    }
}