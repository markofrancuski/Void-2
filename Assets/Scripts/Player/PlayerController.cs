using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using MEC;

//State Machine
public enum PlayerState { IDLE, MOVING, DEAD, INTERACTING, STUNNED};

public class PlayerController : MonoBehaviour, IDestroyable
{
    public PlayerState currentPlayerState;

    #region EVENT/DELEGATE DECLARATION
    public delegate void OnBoundarySet(float x, float y);
    public static event OnBoundarySet onBoundarySet;

    public delegate void OnPlayerDie();
    public static event OnPlayerDie OnPlayerDieEvent;

    #endregion

    #region MOVEMENT VARIABLES
    //public List<string> movementString;
    [SerializeField] public LinkedList<string> movementList;

    //Boundary
    [SerializeField] private float horizontalBoundary;
    [SerializeField] private float verticalBoundary;

    //Movement Vector
    [SerializeField] private Vector3 nextPosition;
    private Vector3 upVector;
    private Vector3 downVector;
    private Vector3 rightVector;
    private Vector3 leftVector;

    [SerializeField] private float tweenDuration;
 
    #endregion

    #region UNITY FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        movementList = new LinkedList<string>();

        currentPlayerState = PlayerState.IDLE;
        StartCoroutine(_MovePlayerCoroutine());
        //Timing.RunCoroutine(_MovePlayerCoroutine());

        //Script execution order => Get Position after level script executes
        Invoke("RetrieveMovePosition", .2f);

    }

    private void Update()
    {
        //if (currentPlayerState == PlayerState.IDLE && !IsFreeFall)
        //{
        //    if(!CheckPlatformUnderneath())
        //    {
        //        IsFreeFall = true;
        //    }
        //}
    }

    private void OnEnable()
    {
        SlidePlatform.OnSlidePlatformInteractEvent += AddFirstMove;
        InputManager.OnSwipedEvent += AddMove;
        InputManager.OnPlayerStateCheckEvent += GetPlayerState;
        InteractableManager.OnShieldActiveCheckEvent += () => IsProtected;
        InteractableManager.OnShieldActivateEvent += ActivateShield;
        InteractableManager.OnWeaponBoostActivateCheckEvent += () => IsWeaponBoostActive;
    }

    private void OnDisable()
    {
        SlidePlatform.OnSlidePlatformInteractEvent -= AddFirstMove;
        InputManager.OnSwipedEvent -= AddMove;
        InputManager.OnPlayerStateCheckEvent -= GetPlayerState;
        InteractableManager.OnShieldActiveCheckEvent -= () => IsProtected;
        InteractableManager.OnShieldActivateEvent -= ActivateShield;
        InteractableManager.OnWeaponBoostActivateCheckEvent -= () => IsWeaponBoostActive;
    }

    private void OnBecameInvisible()
    {
        Death();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile")) Stun();
    }

    #endregion

    #region CHARACTER INTERACTION VARIABLES

    [SerializeField] private GameObject shieldBarrierGO;

    public bool IsWeaponBoostActive;

    [SerializeField] private bool isProtected;
    public bool IsProtected
    {
        get { return isProtected; }
        set { isProtected = value; }
    }

    private bool isMovingDown;
    public bool isDeadFromFall;

    [SerializeField] private bool isFreeFall;
    public bool IsFreeFall 
    {
        get { return isFreeFall; }
        set 
        {
            isFreeFall = value;

            if (value)
            {
                //Cast ray cast down
                //Check Distance between current position and the first platform below if its more then 1. => Death();

                RaycastHit2D[] hits;

                hits = Physics2D.RaycastAll(transform.position, Vector2.down, 20f, 1 << 8);

                CheckPlayerFalling(hits);
            }
        }
    }
   
    void CheckPlayerFalling(RaycastHit2D[] hits)
    {
        
        //Is there platforms under the player
        if (hits.Length > 0)
        {
            Debug.Log("Globals.Instance.movePaceHorizontal * 2: " + Globals.Instance.movePaceHorizontal * 2);

            //If there is more then one platform => raycast will hit the direct platform that is player moving from => get platform in grid under
            if (hits.Length > 1 && isMovingDown)
            {
                Debug.Log("hits[1].distance: " + hits[1].distance);
                if (hits[1].distance >= Globals.Instance.movePaceHorizontal * 2) isDeadFromFall = true;
                else isDeadFromFall = false;
                isMovingDown = false;
            }
            //There is only one platform under => Jumped up or on sides check first platform under player
            else
            {
                Debug.Log("hits[0].distance: " + hits[0].distance);
                if (hits[0].distance >= Globals.Instance.movePaceHorizontal * 2) isDeadFromFall = true;
                else isDeadFromFall = false;
            }

        }
        else
        {
            //No platform under => player will die upon landing
            isDeadFromFall = true;
        }
    }
    #endregion

    #region COROUTINES

    IEnumerator _MovePlayerCoroutine()
    {
        while (true)
        {
            if (movementList.Count != 0 && currentPlayerState == PlayerState.IDLE && !IsFreeFall) // currentPlayerState != PlayerState.INTERACING
            {
             
                nextPosition = GetMovement(movementList.First);

                if (ValidateBoundary())
                {
                    MovePlayer();
                    //Wait Tween duration
                    yield return new WaitForSeconds(tweenDuration);
                }
                else
                {
                    yield return new WaitForSeconds(tweenDuration); // or wait one frame 
                    HandleTweenFinished();
                }
                              
            }
            yield return new WaitUntil(() => currentPlayerState == PlayerState.IDLE);
        }
    }

    IEnumerator _ActivateShieldCoroutine()
    {
        isProtected = true;
        shieldBarrierGO.SetActive(true);
        yield return new WaitForSeconds(3);
        isProtected = false;
        shieldBarrierGO.SetActive(false);
    }
    
    IEnumerator _InteractableCoroutine(float time)
    {
        currentPlayerState = PlayerState.INTERACTING;
        yield return new WaitForSeconds(time);
        currentPlayerState = PlayerState.IDLE;
    }
    
    #endregion

    #region EVENT/DELEGATE FUNCTIONS

    public void AddMove(string movement)
    {
        movementList.AddLast(movement);
        //movementString.Add(movement);
    }

    public void AddFirstMove(string movement)
    {
        movementList.AddFirst(movement);
    }

    public PlayerState GetPlayerState()
    {
        return currentPlayerState;
    }

    public void EnterInFreeFall()
    {
        IsFreeFall = true;
    }
    #endregion

    #region HELPER FUNCTIONS

    void MovePlayer()
    {
        currentPlayerState = PlayerState.MOVING;
        switch (movementList.First.Value)
        {
            case "UP": Tween.Position(gameObject.transform, nextPosition, tweenDuration, 0, Tween.EaseInOutStrong, Tween.LoopType.None, HandleTweenStarted, HandleTweenFinished); break;
            case "DOWN":
                Vector2 nextPos = gameObject.transform.position + new Vector3(0, -1f, 0);
                HandleTweenMovingDownStarted();
                //Tween.Position(gameObject.transform, nextPos, tweenDuration/2, 0, Tween.EaseInOutStrong, Tween.LoopType.None);
                Invoke("HandleTweenMovingDownFinished", .5f);
                break;
            case "RIGHT": Tween.Position(gameObject.transform, nextPosition, tweenDuration, 0, Tween.EaseOut, Tween.LoopType.None, HandleTweenStarted, HandleTweenFinished); break;
            case "LEFT": Tween.Position(gameObject.transform, nextPosition, tweenDuration, 0, Tween.EaseOut, Tween.LoopType.None, HandleTweenStarted, HandleTweenFinished); break;

            default:
                break;
        }
    }

    [SerializeField] private BoxCollider2D boxCollider;

    void HandleTweenMovingDownStarted()
    {
        IsFreeFall = true;
        boxCollider.enabled = false;
    }
    void HandleTweenMovingDownFinished()
    {
        boxCollider.enabled = true;
        if (movementList.Count > 0) movementList.RemoveFirst();
        
    }
    //Change later the name of the method
    void HandleTweenStarted()
    {
        currentPlayerState = PlayerState.MOVING;
    }

    //Called when one move is done
    void HandleTweenFinished()
    {
        //Remove The move
        if (movementList.Count > 0) movementList.RemoveFirst();

        if (!CheckPlatformUnderneath())
        {
            IsFreeFall = true;
        }

        currentPlayerState = PlayerState.IDLE;
     
    }
    //Get Position where to move 
    private Vector3 GetMovement(LinkedListNode<string> str)
    {
        switch (str.Value)
        {
            case "UP": isMovingDown = false; return gameObject.transform.position + upVector; 
            case "DOWN": isMovingDown = true; return gameObject.transform.position + downVector; 
            case "RIGHT": isMovingDown = false; return gameObject.transform.position + rightVector; 
            case "LEFT": isMovingDown = false; return gameObject.transform.position + leftVector;

            default:
                isMovingDown = false; return  gameObject.transform.position;
        }
    }

    private void RetrieveMovePosition()
    {
        upVector = new Vector3(0, Globals.Instance.movePaceVertical, 0);
        downVector = new Vector3(0, -Globals.Instance.movePaceVertical, 0);
        rightVector = new Vector3(Globals.Instance.movePaceHorizontal, 0, 0);
        leftVector = new Vector3(-Globals.Instance.movePaceHorizontal, 0, 0);

        //Set the boundary size
        horizontalBoundary = (Globals.Instance.movePaceHorizontal * 5f)/2;
        verticalBoundary = (Globals.Instance.movePaceVertical * 5f)/2;
    }

    private bool CheckPlatformUnderneath()
    {    
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y/2 + +0.1f, 1 << 8);
        Debug.DrawLine(transform.position, transform.position - new Vector3(0,  transform.localScale.y / 2 + +0.1f, 0) , Color.red);
        if (hit) return true;
        return false;
    }

    private bool ValidateBoundary()
    {
        //First check 
        switch (movementList.First.Value)
        {
            /*case "UP": // Raise Death flag 
                if (GetMovement("UP").y > verticalBoundary) return false;
                return true;*/
            /*case "DOWN": // Raise Death flag 
                if (GetMovement("DOWN").y < -verticalBoundary) return false;
                return true;*/
            case "RIGHT":
                if (nextPosition.x > horizontalBoundary) return false;
                return true;
            case "LEFT":
                if (nextPosition.x < -horizontalBoundary) return false;
                return true;

            default: return true;
        }
    }

    private bool ValidateVerticalBoundary(Vector2 movement)
    {
        if (movement.y >= verticalBoundary || movement.y <= -verticalBoundary) return true;
        return false;
    }
    
    private BasePlatform GetPlatformUnderneath()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + +0.1f, 1 << 8);
        if (hit)
        {
            BasePlatform platform = hit.collider.gameObject.GetComponent<BasePlatform>();
            return platform;
        }

        return null;
        
    }
    
    #endregion

    #region INTERFACE IMPLEMENTATION

    public void DestroyObject()
    {
        if(!isProtected) Death();
    }

    #endregion

    #region CHARACTER INTERACTIONS

    void ActivateShield()
    {
        StartCoroutine(_ActivateShieldCoroutine());
    }

    public void Death()
    {
        StopAllCoroutines();
        currentPlayerState = PlayerState.DEAD;

        OnPlayerDieEvent?.Invoke();
        print("You have died");
    }

    private void Stun()
    {
        currentPlayerState = PlayerState.STUNNED;

    }

    #endregion

}
