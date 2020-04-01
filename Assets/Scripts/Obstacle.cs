using UnityEngine;
using UnityEngine.AI;

// A building is a special, stationary type of ranged unit
public class Obstacle : MonoBehaviour
{
    // For pathfinding
    public NavMeshObstacle navObstacle;
    public Collider2D obstacleCollider;

    void Start()
    {
        // Add pathfinding obstacle
        if (!navObstacle)
        {
            // Unity has 3d pathfinding. Need to use a separate gameobject for the 3d pathfinding plane
            GameObject navObstacleGameObj = new GameObject();
            navObstacleGameObj.name = "Navigation Obstacle";
            navObstacleGameObj.transform.parent = this.transform;
            navObstacleGameObj.layer = LayerMask.NameToLayer("Obstacle");

            // 3d plane uses x,z instead of x,y
            navObstacleGameObj.transform.position = new Vector3(transform.position.x, 0f, transform.position.y);
            navObstacleGameObj.transform.localScale = new Vector3(1f, 1f / transform.localScale.y, transform.localScale.y);

            navObstacle = navObstacleGameObj.AddComponent<NavMeshObstacle>();

            if (obstacleCollider is CircleCollider2D circleCollider)
            {
                navObstacle.shape = NavMeshObstacleShape.Capsule;
                navObstacle.radius = circleCollider.radius;
            }
            else if (obstacleCollider is BoxCollider2D boxCollider)
            {
                navObstacle.shape = NavMeshObstacleShape.Box;
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