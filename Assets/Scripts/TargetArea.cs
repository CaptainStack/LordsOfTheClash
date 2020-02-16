using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetArea : MonoBehaviour
{
    public CircleCollider2D targetCollider2D;
    public List<Character> targetList;
    public float range = 10f;

    void Start()
    {
        targetCollider2D = this.gameObject.AddComponent<CircleCollider2D>();
        targetCollider2D.isTrigger = true;

        targetList = new List<Character>();
        targetCollider2D.radius = range;
    }

    void Update()
    {
        // Update range in case it changed
        targetCollider2D.radius = range;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Add characters to targetList
        Character character = other.gameObject.GetComponent<Character>();
        if (character != null && !targetList.Contains(character))
        {
            targetList.Add(character);

            // Keep target list sorted by range
            var self = this;
            targetList.OrderBy(n => (self.transform.position - n.transform.position).magnitude);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Remove characters from targetList
        Character character = other.gameObject.GetComponent<Character>();
        targetList.Remove(character);
    }
}