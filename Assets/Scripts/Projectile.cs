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

    public SpriteRenderer spriteRenderer;
    public Sound sound;
    private AudioSource audioSource;

    private float detonationTimer = 0f;
    private bool detonated = false;
    
    void Start()
    {
        // Add Sprite
        if (!spriteRenderer)
            spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();

        if (sound.clip)
        {
            if (!audioSource)
                audioSource = this.gameObject.AddComponent<AudioSource>();

            audioSource.spatialize = true;
            audioSource.spatialBlend = .5f;
            sound.SetSource(audioSource);
            sound.PlaySound();
        }

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
        if (detonated)
        {
            // Hide projectile until audio is finished playing, then destroy it
            this.spriteRenderer.enabled = false;

            if (!audioSource || !audioSource.isPlaying)
                Destroy(this.gameObject);
        }
        // Explode if projectile has reached target
        else if (Time.time > detonationTimer)
        {
            Explode();
        }
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
        detonated = true;
    }

    // Move towards target
    void MoveToTarget()
    {
        Vector3 movementVector = target - this.transform.position; 
        Vector3 direction = (movementVector).normalized;
        this.transform.position += direction * speed * Time.deltaTime;
    }
}