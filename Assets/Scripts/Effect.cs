using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Effect : MonoBehaviour
{
    public Unit summon;

    public AreaOfEffect areaOfEffectPrefab;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;   
    }

    void Update()
    {
        
    }

    public void Action(Vector2 position)
    {
        if (areaOfEffectPrefab != null) //Apply Area Of Effect at mouse position
        {
            AreaOfEffect newAreaOfEffect = Instantiate(areaOfEffectPrefab, position, Quaternion.identity);
            Mirror.NetworkServer.Spawn(newAreaOfEffect.gameObject);
        }
        if (summon != null) //summon unit at mouse position
        {
            Unit newUnit = Instantiate(summon, position, Quaternion.identity);
            Mirror.NetworkServer.Spawn(newUnit.gameObject);
        }
    }
}
