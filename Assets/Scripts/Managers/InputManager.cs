using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    #region INPUT
    [Header("INPUT")]
    private Vector2 startTouchPosition;
    private Vector3 swipeDirection;
    [SerializeField] private float swipeSensitivity;
    #endregion

    #region SWIPE EVENT
    public delegate void OnSwiped(string str);
    //Subscribe methods form other scripts to listen this event => When triggered pass calls this function and pass the argument => subscribed methods will be called with the argument
    public static event OnSwiped OnSwipedEvent;

    //Fetch the PlayerState for checking input
    public delegate PlayerState OnPlayerStateCheck();
    public static event OnPlayerStateCheck OnPlayerStateCheckEvent;

    #endregion

    // Update is called once per frame
    void Update() // Maybe fixedUpdate
    {
        if(OnPlayerStateCheckEvent.Invoke() != PlayerState.DEAD) CheckTouchInput();
    }

    void CheckTouchInput()
    {
        if (Input.touchCount > 0)
        {
            //First touch => First finger that touched the screen.
            Touch touch = Input.GetTouch(0);

            //User start pressing the screen => Retreive the start position.
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            //User is no longer touching the screen => Get the position => Check the direction, 'Swiping'.
            if (touch.phase == TouchPhase.Ended)
            {
                swipeDirection = (touch.position - startTouchPosition).normalized;

                if (swipeDirection.x >= swipeSensitivity)
                {
                    AddMovement("RIGHT");

                }

                else if (swipeDirection.x <= -swipeSensitivity)
                {
                    AddMovement("LEFT");

                }

                else if (swipeDirection.y >= swipeSensitivity)
                {
                    AddMovement("UP");

                }

                else if (swipeDirection.y <= -swipeSensitivity)
                {
                    AddMovement("DOWN");

                }

            }
        }
    }
 
    void AddMovement(string str)
    {
        // Alert subscribed methods if its not null
        OnSwipedEvent?.Invoke(str);
        //OnSwipedEvent?.Invoke(str);
    }

}
