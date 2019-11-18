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

    [SerializeField] private Transform TerrainObject;
    private float movePaceHorizontal;
    private float movePaceVertical;
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
    [SerializeField] private bool freezeMovement;

    #endregion

    #region UNITY FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        movementList = new LinkedList<string>();

        currentPlayerState = PlayerState.IDLE;
        StartCoroutine(_MovePlayerCoroutine());
        //StartCoroutine(_MovePlayerDownCoroutine());
        //Timing.RunCoroutine(_MovePlayerCoroutine());

        //Script execution order => Get Position after level script executes
        Invoke("RetrieveMovePosition", .5f);

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
    int fallingFloorNumber;
    [SerializeField] private GameObject shieldBarrierGO;

    public bool IsWeaponBoostActive;

    [SerializeField] private bool isProtected;
    public bool IsProtected
    {
        get { return isProtected; }
        set { isProtected = value; }
    }

    #endregion

    #region COROUTINES

    IEnumerator _MovePlayerCoroutine()
    {
        while (true)
        {
            if (movementList.Count != 0 && currentPlayerState == PlayerState.IDLE) // currentPlayerState != PlayerState.INTERACING
            {
             
                GetMovement(movementList.First);

                if (ValidateBoundary())
                {
                    Tween.Position(gameObject.transform, nextPosition, tweenDuration, 0, Tween.EaseOutBack, Tween.LoopType.None, HandleTweenStarted, HandleTweenFinished);
                    //Wait Tween duration
                    yield return new WaitForSeconds(tweenDuration);
                }
                else
                {
                    yield return new WaitForSeconds(tweenDuration); // or wait one frame 
                    HandleTweenFinished();
                }
                  
                //yield return Timing.WaitUntilDone(currentPlayerState == PlayerState.IDLE);               
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

    #endregion

    #region HELPER FUNCTIONS
    public int GetFallingFloorsHeight() => fallingFloorNumber;

    //Change later the name of the method
    void HandleTweenStarted()
    {
        currentPlayerState = PlayerState.MOVING;
    }
    public bool isFreeFall;

    //Called when one move is done
    void HandleTweenFinished()
    {
        //Remove The move
        if (movementList.Count > 0) movementList.RemoveFirst();
        
        //Check States
        if (!CheckPlatformUnderneath())
        {
            fallingFloorNumber++;
            AddMove("DOWN");
            //Raycast don check distance
        }
        else
        {
            
            gameObject.transform.position = nextPosition;
            nextPosition = gameObject.transform.position;
          
            BasePlatform platform = GetPlatformUnderneath();
            if(platform != null)
            {             
                platform.Interact(this);
                fallingFloorNumber = 0;
            }
        }
        currentPlayerState = PlayerState.IDLE;
    }
    //Get Position where to move 
    private void GetMovement(LinkedListNode<string> str)
    {
        switch (str.Value)
        {
            case "UP": nextPosition = gameObject.transform.position + upVector; break;
            case "DOWN": nextPosition = gameObject.transform.position + downVector; break;
            case "RIGHT": nextPosition = gameObject.transform.position + rightVector; break;
            case "LEFT": nextPosition = gameObject.transform.position + leftVector; break;

            default:
                nextPosition = gameObject.transform.position; break;
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
        else
        {
            return null;
        }

    }
    
    #endregion

    #region INTERFACE IMPLEMENTATION

    public void DestroyObject()
    {
        Death();
    }

    #endregion

    #region CHARACTER INTERACTIONS

    public bool GetFreeze()
    {
        return freezeMovement;
    }

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
