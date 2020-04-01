using UnityEngine;

// A building is a special, stationary type of ranged unit
public class Obstacle : MonoBehaviour
{
    // For pathfinding
    public UnityEngine.AI.NavMeshObstacle navObstacle;
    public Collider2D obstacleCollider;

    void Start()
    {
        // Add pathfinding obstacle
        if (!navObstacle)
        {
            GameObject navObstacleGameObj = new GameObject();
            navObstacleGameObj.name = "Navigation Obstacle";
            navObstacleGameObj.transform.parent = this.transform;
            navObstacleGameObj.layer = LayerMask.NameToLayer("Obstacle");

            navObstacleGameObj.transform.position = new Vector3(transform.position.x, 0f, transform.position.y);
            navObstacleGameObj.transform.localScale = new Vector3(1f, 1f / transform.localScale.y, transform.localScale.y);

            navObstacle = navObstacleGameObj.AddComponent<UnityEngine.AI.NavMeshObstacle>();

            if (obstacleCollider is CircleCollider2D circleCollider)
            {
                navObstacle.shape = UnityEngine.AI.NavMeshObstacleShape.Capsule;
                navObstacle.radius = circleCollider.radius;
            }
            else if (obstacleCollider is BoxCollider2D boxCollider)
            {
                navObstacle.shape = UnityEngine.AI.NavMeshObstacleShape.Box;
                navObstacle.size = new Vector3(boxCollider.size.x, 10f, boxCollider.size.y) * 1.2f;
            }

            navObstacle.height = 10f;
            navObstacle.center = new Vector3 (0f, navObstacle.height, 0f);
            navObstacle.carving = true;
        }
    }

    void OnDestroy()
    {
        if (navObstacle)
            Destroy(navObstacle.gameObject);
    }
}