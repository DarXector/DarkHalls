using UnityEngine;
using System.Collections;

public class DistanceToPlayer : MonoBehaviour {

    public float showDistance = 2f;

    private GameObject player;

    public ShowHideEffect effect;

    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(!effect)
        {
            effect = gameObject.GetComponent<ShowHideEffect>();
        }

        if (effect)
        {
            effect.Hide();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!effect || !player)
        {
            return;
        }
        // Create a vector from the enemy to the player and store the angle between it and forward.
        float distance = Mathf.Abs(Vector3.Distance(player.transform.position, transform.position));

        RaycastHit hit;
        // Create a vector from the enemy to the player and store the angle between it and forward.
        Vector3 direction = player.transform.position - transform.position;

        if (Physics.Raycast(transform.position, direction.normalized, out hit, showDistance) && hit.collider.gameObject == player)
        {
            effect.Show();
        }
        else if (distance < 1f)
        {
            effect.Show();
        }
        else
        {
            effect.Hide();
        }
    }
}
