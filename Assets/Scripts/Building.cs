using UnityEngine;

// A building is a special, stationary type of ranged unit
public class Building : RangedUnit
{
    // For pathfinding
    public Obstacle navigationObstacle;

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
}