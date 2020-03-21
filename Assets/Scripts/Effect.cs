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

   public void Action()
    {
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector3 point = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cam.nearClipPlane));
        point.z = 0f;

        if (areaOfEffectPrefab != null) //Apply Area Of Effect at mouse position
        {
            AreaOfEffect newAreaOfEffect = Instantiate(areaOfEffectPrefab, point, Quaternion.identity);
        }
        if (summon != null) //summon unit at mouse position
        {
            Unit newUnit = Instantiate(summon, point, Quaternion.identity);
        }
    }
}
