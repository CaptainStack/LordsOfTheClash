using UnityEngine;

// A building is a special, stationary type of ranged unit
public class Building : RangedUnit
{
    // For pathfinding
    public Obstacle navigationObstacle;
    public GameObject summonArea;

    public override void OnStartServer()
    {
        base.OnStartServer();

        // Buildings can't move
        this.speed = 0f;
        this.unitRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;

        // Add pathfinding obstacle
        if (!navigationObstacle)
        {
            navigationObstacle = gameObject.AddComponent<Obstacle>();
            navigationObstacle.obstacleCollider = unitCollider;
        }
    }

    override protected void InitializeUnitDepth()
    {
        // Buildings have foundations "below ground", so give them a positive z depth (inward towards screen)
        transform.position = new Vector3(transform.position.x, transform.position.y, 1f);
    }

    private void Update() //check to see if building destroyed to active neutral summoning area
    {
        if (this.health <= 0)
        {
            if (summonArea != null)
            {
                summonArea.SetActive(true);
            }
            Destroy(this.gameObject);
        }
    }
}