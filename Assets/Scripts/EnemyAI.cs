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
    public float showDistance = 2f;
    public float chaseDistance = 1.5f;

    private GameObject player;
    private ShowHideEffect effect;

    public new void Start()
    {
        target = GameManager.gm.GetWaypoint();

        player = GameObject.FindGameObjectWithTag("Player");
        effect = gameObject.transform.GetChild(0).gameObject.GetComponent<ShowHideEffect>();
        if(effect)
        {
            effect.Hide();
        }

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
        if(!effect || !player)
        {
            return;
        }

        RaycastHit hit;
        // Create a vector from the enemy to the player and store the angle between it and forward.
        Vector3 direction = player.transform.position - transform.position;
        float distance = Mathf.Abs(Vector3.Distance(player.transform.position, transform.position));

        if (distance < showDistance)
        {
            effect.Show();
        }
        else
        {
            effect.Hide();
        }

        // ... and if a raycast towards the player hits something...
        if (Physics.Raycast(transform.position, direction.normalized, out hit, chaseDistance))
        {
            //Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
            // ... and if the raycast hits the player...

            if (hit.collider.gameObject == player)
            {
                if(!playerInSight)
                {
                    playerInSight = true;
                    speed = chaseSpeed;
                    target = player.transform;
                }

                base.Update();
                return;
            }
        }

        if(playerInSight)
        {
            speed = normalSpeed;
            playerInSight = false;
            target = GameManager.gm.GetWaypoint();
        }
        

        base.Update();
    }
}
