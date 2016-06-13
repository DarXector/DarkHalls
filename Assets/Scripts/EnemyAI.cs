using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(EnemySoundControl))]
public class EnemyAI : AIPath
{
    public delegate void OnTargetReachedEvent();
    public event OnTargetReachedEvent OnPlayerCought;

    public bool playerInSight;

    public float normalSpeed = 1f;
    public float chaseSpeed = 2f;
    public float chaseDistance = 1.5f;

    private GameObject player;

    private EnemySoundControl _soundControl;

    public new void Start()
    {
        target = GameManager.gm.GetWaypoint();

        player = GameObject.FindGameObjectWithTag("Player");

        _soundControl = GetComponent<EnemySoundControl>();

        speed = normalSpeed;

        base.Start();
    }

    public override void OnTargetReached()
    {
        if (target.tag != "Player")
        {
            target = GameManager.gm.GetWaypoint();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.tag == "Player" && OnPlayerCought != null)
        {
            OnPlayerCought();
        }
    }

    protected new void Update()
    {
        if(!player)
        {
            return;
        }

        RaycastHit hit;
        // Create a vector from the enemy to the player and store the angle between it and forward.
        Vector3 direction = player.transform.position - transform.position;


        // ... and if a raycast towards the player hits something...
        if (Physics.Raycast(transform.position, direction.normalized, out hit, chaseDistance) && hit.collider.gameObject == player)
        {
            if (!playerInSight)
            {
                //Debug.Log("Seen player");
                playerInSight = true;
                speed = chaseSpeed;
                _soundControl.PlayRoar();
            }

            //Debug.Log("Chasing player");
            target = player.transform;

            base.Update();
            return;
        }

        if (playerInSight)
        {
            //Debug.Log("Lost sight of player");
            speed = normalSpeed;
            playerInSight = false;
            GetNewWaypoint();
        }
        

        base.Update();
    }

    void GetNewWaypoint()
    {
        target = GameManager.gm.GetWaypoint();
    }
}
