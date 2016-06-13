using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class PositionJoystick : MonoBehaviour
{
    private RectTransform _joystickRect;

    void Awake()
    {
        _joystickRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        int nbTouches = Input.touchCount;

        if (nbTouches > 0)
        {
            Touch touch = Input.GetTouch(0);

            TouchPhase phase = touch.phase;

            switch (phase)
            {
                case TouchPhase.Began:
                    ShowJoystick(touch.position);
                    break;
                case TouchPhase.Ended:
                    HideJoystick();
                    break;
                case TouchPhase.Canceled:
                    HideJoystick();
                    break;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShowJoystick(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                HideJoystick();
            }

        }
    }

    public void ShowJoystick(Vector2 touchPos)
    {
        Debug.Log("touchPos " + touchPos);

        gameObject.SetActive(true);
        _joystickRect.position = new Vector3(touchPos.x, touchPos.y, 0);
    }

    public void HideJoystick()
    {
        gameObject.SetActive(false);
    }
}
