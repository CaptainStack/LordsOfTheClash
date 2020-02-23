using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Effect : MonoBehaviour
{
    public Unit summon;

    public float spellDamage;
    public float healPower;
    public float radius;
    public float impactForce;

    public Explosion explosionPrefab;

    private Camera cam;
    
    void Start()
    {
        cam = Camera.main;   
    }

    void Update()
    {
        
    }

   public void Action()
    {
        Vector3 point = new Vector3();
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        point = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cam.nearClipPlane));
        point.z = -2.668513f;

        if (explosionPrefab != null) //Do damage and apply force to enemies at mouse position
        {
            explosionPrefab.damage = spellDamage;
            explosionPrefab.radius = radius;
            explosionPrefab.faction = Faction.Friendly;
            explosionPrefab.impactForce = impactForce;
            Explosion newExplosion = Instantiate(explosionPrefab, point, Quaternion.identity);
        }
        if (summon != null) //summon unit at mouse position
        {
            Unit newUnit = Instantiate(summon, point, Quaternion.identity);
        }
    }
}
