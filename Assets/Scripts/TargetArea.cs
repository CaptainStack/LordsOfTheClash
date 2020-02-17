using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetArea : MonoBehaviour
{
    public CircleCollider2D targetCollider2D;
    public List<Unit> targetList;
    public float range = 10f;

    void Start()
    {
        targetCollider2D = this.gameObject.AddComponent<CircleCollider2D>();
        targetCollider2D.isTrigger = true;

        targetList = new List<Unit>();
        targetCollider2D.radius = range;
    }

    void Update()
    {
        // Update range in case it changed
        targetCollider2D.radius = range;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Add units to targetList
        Unit unit = other.gameObject.GetComponent<Unit>();
        if (unit != null && !targetList.Contains(unit))
        {
            targetList.Add(unit);

            // Keep target list sorted by range
            var self = this;
            targetList.OrderBy(n => (self.transform.position - n.transform.position).magnitude);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Remove units from targetList
        Unit unit = other.gameObject.GetComponent<Unit>();
        targetList.Remove(unit);
    }
}