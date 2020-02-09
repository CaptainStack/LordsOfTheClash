using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArea : MonoBehaviour
{
    public CircleCollider2D targetCollider2D;
    public List<Character> targetList;

    void Start()
    {
        targetCollider2D = this.gameObject.AddComponent<CircleCollider2D>();
        targetCollider2D.isTrigger = true;

        targetList = new List<Character>();
    }

    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Add characters to targetList
        Character character = other.gameObject.GetComponent<Character>();
        if (character != null && !targetList.Contains(character))
            targetList.Add(character);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Remove characters from targetList
        Character character = other.gameObject.GetComponent<Character>();
        if (character != null)
            targetList.Remove(character);
    }
}