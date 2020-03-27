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

    //public Rigidbody2D rigidbody2d;
    public SpriteRenderer spriteRenderer;

    private float detonationTimer = 0f;
    
    void Start()
    {
        AudioManager.GetInstance().PlaySound("Shoot");
        // Add Sprite
        if (!spriteRenderer)
            spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();

        ComputeDetonationTimer();
    }

    // Set detonation timer for when the projectile will arrive
    void ComputeDetonationTimer()
    {
        Vector3 movementVector = target - this.transform.position; 
        detonationTimer = Time.time + movementVector.magnitude / speed;
    }

    void Update()
    {
    }

    // FixedUpdate runs synchronized with Unity physics cycle
    void FixedUpdate()
    {
        // Explode if projectile has reached target
        if (Time.time > detonationTimer)
            Explode();
        else
            MoveToTarget();
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
        this.transform.position += direction * speed * Time.deltaTime;
    }
}