using UnityEngine;
using System.Collections;

public class DistanceToPlayer : MonoBehaviour {

    public float showDistance = 2f;

    private GameObject player;
    private ShowHideEffect effect;

    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        effect = gameObject.transform.GetChild(0).gameObject.GetComponent<ShowHideEffect>();
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

        if (distance < showDistance)
        {
            effect.Show();
        }
        else
        {
            effect.Hide();
        }
    }
}
