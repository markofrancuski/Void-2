﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using MEC;

//State Machine
public enum PlayerState { IDLE, MOVING, DEAD };

public class PlayerController : MonoBehaviour
{
    public PlayerState currentPlayerState;

    #region EVENT/DELEGATE DECLARATION
    public delegate void OnBoundarySet(float x, float y);
    public static event OnBoundarySet onBoundarySet;

    public delegate void OnPlayerDie();
    public static event OnPlayerDie OnPlayerDieEvent;

    #endregion

    #region MOVEMENT VARIABLES
    public List<string> movementString;
    [SerializeField] private Transform TerrainObject;
    private float movePaceHorizontal;
    private float movePaceVertical;
    //Boundary
    [SerializeField] private float horizontalBoundary;
    [SerializeField] private float verticalBoundary;

    //Movement Vector
    private Vector3 upVector;
    private Vector3 downVector;
    private Vector3 rightVector;
    private Vector3 leftVector;

    [SerializeField] private float tweenDuration;
    [SerializeField] private bool freezeMovement;

    #endregion

    #region UNITY FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        currentPlayerState = PlayerState.IDLE;
        StartCoroutine(_MovePlayerCoroutine());

        OnPlayerDieEvent += Death;

        //Timing.RunCoroutine(_MovePlayerCoroutine());

        //Script execution order => Get Position after level script executes
        Invoke("RetrieveMovePosition", .5f);

    }

    private void OnEnable()
    {
        InputManager.OnSwipedEvent += AddMove;
        InputManager.OnPlayerStateCheckEvent += GetPlayerState;
    }

    private void OnDisable()
    {
        InputManager.OnSwipedEvent -= AddMove;
        InputManager.OnPlayerStateCheckEvent -= GetPlayerState;
    }

    #endregion

    IEnumerator _MovePlayerCoroutine()
    {
        while (true)
        {
            if (movementString.Count != 0 && currentPlayerState == PlayerState.IDLE)
            {
                if (!ValidateVerticalBoundary(GetMovement(movementString[0])))
                {
                    //Check Boundaries
                    if (ValidateBoundary())
                    {
                        Tween.Position(gameObject.transform, GetMovement(movementString[0]), tweenDuration, 0, Tween.EaseOutBack, Tween.LoopType.None, HandleTweenStarted, HandleTweenFinished);
                        //Wait Tween duration
                        yield return new WaitForSeconds(tweenDuration);
                        //Check If it should stop executing the queued swipes.(freezeMovement is changed in the HandleTweenFinished() )
                        if (freezeMovement) yield return StartCoroutine(_MovePlayerDownCoroutine());
                    }
                    else
                    {
                        yield return new WaitForSeconds(tweenDuration); // or wait one frame 
                        HandleTweenFinished();
                    }
                }             
            }
            //yield return Timing.WaitUntilDone(currentPlayerState == PlayerState.IDLE);
            yield return new WaitUntil( () => currentPlayerState == PlayerState.IDLE);
        }
    }
       
    /// <summary>
    /// 
    /// Move player one grid cell down
    /// if there is platform underneath the player stop moving down 'falling down' => break;
    /// else continue moving down until you hit the platform or boundary(Die).
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator _MovePlayerDownCoroutine()
    {
        while(true)
        {
            //Move player Down one grid
            Tween.Position(gameObject.transform, GetMovement("DOWN"), tweenDuration, 0, Tween.EaseOutBack, Tween.LoopType.None);
            yield return new WaitForSeconds(tweenDuration);
            //Check if there is platfom underneath
            if(CheckPlatformUnderneath())
            {
                break;
            }
            else
            {
                ValidateVerticalBoundary(GetMovement("DOWN"));
                StopCoroutine(_MovePlayerDownCoroutine());
            }
        }
    }

    #region EVENT/DELEGATE FUNCTIONS

    public void AddMove(string movement)
    {
        movementString.Add(movement);
    }

    public PlayerState GetPlayerState()
    {
        return currentPlayerState;
    }
    #endregion

    #region HELPER FUNCTIONS
    //Change later the name of the method
    void HandleTweenStarted()
    {
        currentPlayerState = PlayerState.MOVING;
    }

    void HandleTweenFinished()
    {

        if (!CheckPlatformUnderneath()) freezeMovement = true;
        else freezeMovement = false;
        currentPlayerState = PlayerState.IDLE;
        //Remove last movement from the list
        if (movementString.Count > 0) movementString.RemoveAt(0);
    }

    //Get Position where to move 
    private Vector2 GetMovement(string str)
    {
        switch (str)
        {
            case "UP": return gameObject.transform.position + upVector;
            case "DOWN": return gameObject.transform.position + downVector;
            case "RIGHT": return gameObject.transform.position + rightVector;
            case "LEFT": return gameObject.transform.position + leftVector;

            default:
                return gameObject.transform.position;
        }
    }

    private void RetrieveMovePosition()
    {
        Level levelScript = TerrainObject.GetChild(0).GetComponent<Level>();
        movePaceHorizontal = levelScript.moveX;
        movePaceVertical = levelScript.moveY;

        upVector = new Vector3(0, movePaceVertical, 0);
        downVector = new Vector3(0, -movePaceVertical, 0);
        rightVector = new Vector3(movePaceHorizontal, 0, 0);
        leftVector = new Vector3(-movePaceHorizontal, 0, 0);

        //Set the boundary size
        horizontalBoundary = (movePaceHorizontal * 5f)/2;
        verticalBoundary = ( movePaceVertical * 5f)/2;
    }

    private bool CheckPlatformUnderneath()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y/2 + +0.1f, 1 << 8);
        if (hit) return true;
        else return false;
    }

    private bool ValidateBoundary()
    {
        //First check 
        switch (movementString[0])
        {
            /*case "UP": // Raise Death flag 
                if (GetMovement("UP").y > verticalBoundary) return false;
                return true;*/
            /*case "DOWN": // Raise Death flag 
                if (GetMovement("DOWN").y < -verticalBoundary) return false;
                return true;*/
            case "RIGHT":
                if (GetMovement("RIGHT").x > horizontalBoundary) return false;
                return true;
            case "LEFT":
                if (GetMovement("LEFT").x < -horizontalBoundary) return false;
                return true;

            default: return true;
        }
    }

    private bool ValidateVerticalBoundary(Vector2 movement)
    {
        if (movement.y >= verticalBoundary || movement.y <= -verticalBoundary) { OnPlayerDieEvent?.Invoke(); return true; }

        return false;
    }
    #endregion

    private void Death()
    {
        currentPlayerState = PlayerState.DEAD;

        print("You have died");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(0, - (transform.localScale.y/2 + 0.1f), 0) );
    }
}