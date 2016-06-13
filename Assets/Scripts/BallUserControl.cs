using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

class BallUserControl : MonoBehaviour
{
    private Ball ball; // Reference to the ball controller.

    private Vector3 move;
    // the world-relative desired move direction, calculated from the camForward and user input.

    private Transform cam; // A reference to the main camera in the scenes transform
    private Vector3 camForward; // The current forward direction of the camera
    private bool jump; // whether the jump button is currently pressed
    private Vector2 touchOrigin;
    private bool _mouseDown;

    private void Awake()
    {
        // Set up the reference.
        ball = GetComponent<Ball>();


        // get the transform of the main camera
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Ball needs a Camera tagged \"MainCamera\", for camera-relative controls.");
            // we use world-relative controls in this case, which may not be what the user wants, but hey, we warned them!
        }
    }


    private void Update()
    {
        // Get the axis and jump input.

        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");

        if (Input.touchCount > 0)
        {
            //Store the first touch detected.
            Touch myTouch = Input.touches[0];

            //Check if the phase of that touch equals Began
            if (myTouch.phase == TouchPhase.Began)
            {
                //If so, set touchOrigin to the position of that touch
                touchOrigin = myTouch.position;
                _mouseDown = true;
            }

            //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
            else if (myTouch.phase == TouchPhase.Ended)
            {
                _mouseDown = false;
            }

            if (_mouseDown)
            {
                //Set touchEnd to equal the position of this touch
                Vector2 touchEnd = myTouch.position;

                var move = GetRelativePosition(touchEnd);

                v = move.v;
                h = move.h;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchOrigin = Input.mousePosition;
                _mouseDown = true;

            }

            if (Input.GetMouseButtonUp(0))
            {
                _mouseDown = false;
            }

            if(_mouseDown)
            {
                //Set touchEnd to equal the position of this touch
                Vector2 touchEnd = Input.mousePosition;

                var move = GetRelativePosition(touchEnd);

                v = move.v;
                h = move.h;
            }

        }

        // calculate move direction
        if (cam != null)
        {
            // calculate camera relative direction to move:
            camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            move = (v*camForward + h*cam.right).normalized;
        }
        else
        {
            // we use world-relative directions in the case of no main camera
            move = (v*Vector3.forward + h*Vector3.right).normalized;
        }
    }

    Move GetRelativePosition(Vector2 touchEnd)
    {
        float h = 0f;
        float v = 0f;

        float x = touchEnd.x - touchOrigin.x;

        //Calculate the difference between the beginning and end of the touch on the y axis.
        float y = touchEnd.y - touchOrigin.y;

        //Check if the difference along the x axis is greater than the difference along the y axis.
        h = x;
        v = y;

        return new Move(h, v);
    }


    private void FixedUpdate()
    {
        // Call the Move function of the ball controller
        ball.Move(move, jump);
        jump = false;
    }
}

class Move
{
    public Move(float ph, float pv)
    {
        h = ph;
        v = pv;
    }

    public float h;
    public float v;
}
