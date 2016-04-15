using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Seeker))]
public class EnemyAI : AIPath
{
    public delegate void OnTargetReachedEvent(GameObject g);
    public event OnTargetReachedEvent OnTargetReach;

    public bool playerInSight;

    public float normalSpeed = 1f;
    public float chaseSpeed = 2f;
    public float playerChaseDistance = 2f;

    private GameObject player;

    public new void Start()
    {
        target = GameManager.gm.GetWaypoint();

        player = GameObject.FindGameObjectWithTag("Player");

        speed = normalSpeed;

        base.Start();
    }

    public override void OnTargetReached()
    {
        OnTargetReach(target.gameObject);

        if (target.tag != "Player")
        {
            target = GameManager.gm.GetWaypoint();
        }
    }

    protected new void Update()
    {
        RaycastHit hit;
        // Create a vector from the enemy to the player and store the angle between it and forward.
        Vector3 direction = player.transform.position - transform.position;

        // ... and if a raycast towards the player hits something...
        if (Physics.Raycast(transform.position, direction.normalized, out hit, playerChaseDistance))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
            // ... and if the raycast hits the player...
            if (hit.collider.gameObject == player)
            {
                // ... the player is in sight.
                playerInSight = true;

                speed = chaseSpeed;

                target = player.transform;
            }
            else if (playerInSight)
            {
                speed = normalSpeed;
                playerInSight = false;
                target = GameManager.gm.GetWaypoint();
            }
        }
        else if (playerInSight)
        {
            speed = normalSpeed;
            playerInSight = false;
            target = GameManager.gm.GetWaypoint();
        }


        base.Update();
    }
}
