using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using MEC;
using UnityEngine.Events;

public class PlayerController : Person, IDestroyable
{

    #region EVENT/DELEGATE DECLARATION

    public delegate void OnBoundarySet(float x, float y);
    public static event OnBoundarySet onBoundarySet;

    //public delegate void OnPlayerDie();
    //public static event OnPlayerDie OnPlayerDieEvent;

    #endregion

    #region MOVEMENT VARIABLES
    //public List<string> movementString;
    
    [SerializeField] public LinkedList<string> movementList;
 
    #endregion

    #region UNITY FUNCTIONS
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        movementList = new LinkedList<string>();

        currentState = PersonState.IDLE;
        StartCoroutine(_MovePlayerCoroutine());
        //Timing.RunCoroutine(_MovePlayerCoroutine());

    }

   /* private void Update()
    {
        //if (currentPlayerState == PlayerState.IDLE && !IsFreeFall)
        //{
        //    if(!CheckPlatformUnderneath())
        //    {
        //        IsFreeFall = true;
        //    }
        //}
    }*/

    private void OnEnable()
    {
       
        currentState = PersonState.IDLE;
        SlidePlatform.OnSlidePlatformInteractEvent += AddFirstMove;

        LevelManager.ResetLevel += () => Destroy(gameObject);

        InputManager.OnSwipedEvent += AddMove;
        InputManager.OnPlayerStateCheckEvent += GetPlayerState;
        InputManager.OnUnControlPlayerEvent += ClearList;

        InteractableManager.OnShieldActiveCheckEvent += () => IsProtected;
        InteractableManager.OnShieldActivateEvent += ActivateShield;
        InteractableManager.OnWeaponBoostActivateCheckEvent += () => IsWeaponBoostActive.currentValue;
        InteractableManager.GetPlayerTeamEvent += () => Team;

        GameManager.CheckPlayerLifeSaverEvent += GetLifeSaver;
        GameManager.ReviveButtonClickedEvent += ChangePlayerExtraLife;
        GameManager.ReviveButtonClickedEvent += OnReviveClick;

        //Test
        GameManager.ResetSceneEvent += () => { IsExtraLifeActive.currentValue = true; };
        //OnPlayerDieEvent += LevelManager.Instance.ShowDeathPanel;
    }

    private void OnDisable()
    {
       
        SlidePlatform.OnSlidePlatformInteractEvent -= AddFirstMove;

        LevelManager.ResetLevel -= () => Destroy(gameObject);

        InputManager.OnSwipedEvent -= AddMove;
        InputManager.OnPlayerStateCheckEvent -= GetPlayerState;
        InputManager.OnUnControlPlayerEvent -= ClearList;

        InteractableManager.OnShieldActiveCheckEvent -= () => IsProtected;
        InteractableManager.OnShieldActivateEvent -= ActivateShield;
        InteractableManager.OnWeaponBoostActivateCheckEvent -= () => IsWeaponBoostActive.currentValue;
        InteractableManager.GetPlayerTeamEvent -= () => Team;

        GameManager.CheckPlayerLifeSaverEvent -= GetLifeSaver;
        GameManager.ReviveButtonClickedEvent -= ChangePlayerExtraLife;
        GameManager.ReviveButtonClickedEvent -= OnReviveClick;
        //Test
        GameManager.ResetSceneEvent -= () => { IsExtraLifeActive.currentValue = true; };
        //OnPlayerDieEvent -= LevelManager.Instance.ShowDeathPanel;
    }

    private void OnBecameInvisible()
    {
       if (Globals.Instance.isSceneReady) Death("Invisible");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Boundary"))
        {
            Death("Falling of the world");
        }
    }
    
    #endregion

    #region CHARACTER INTERACTION VARIABLES

    [Header("CHARACTER INTERACTION VARIABLES")]
    [SerializeField] private GameObject shieldBarrierGO;

    [SerializeField] private BoolValue IsWeaponBoostActive;
    [SerializeField] private BoolValue IsExtraLifeActive;


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
           
            if (movementList.Count != 0 && currentState == PersonState.IDLE && !IsFreeFall)
            {              
                //if (InputManager.Instance.isControllable)
                //{
                    nextPosition = GetMovement(movementList.First);

                    if (ValidateBoundary())
                    {
                        MovePlayer();
                        //Wait Tween duration
                        yield return new WaitForSeconds(Globals.Instance.tweenDuration);
                    }
                    else
                    {
                        yield return new WaitForSeconds(Globals.Instance.tweenDuration); // or wait one frame 
                        HandleTweenFinished();
                    }
                //}
                ////Cancel the chain movement
                //else movementList.Clear();
                                           
            }
            yield return new WaitUntil(() => currentState == PersonState.IDLE);
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
    
    IEnumerator _InteractableCoroutine(float time, UnityAction interactMethod)
    {
        currentState = PersonState.INTERACTING;
        // InteractEvent += interactMethod;
        // InteractEvent?.Invoke();
        yield return new WaitForSeconds(time);
        currentState = PersonState.IDLE;
        // InteractEvent -= interactMethod;
    }

    #endregion

    #region EVENT/DELEGATE FUNCTIONS

    public void ChangePlayerExtraLife()
    {
        IsExtraLifeActive.currentValue = !IsExtraLifeActive.currentValue;
    }

    public bool GetLifeSaver() => IsExtraLifeActive.currentValue;

    public void AddMove(string movement)
    {
        movementList.AddLast(movement);
        //movementString.Add(movement);
    }

    public void AddFirstMove(string movement)
    {
        movementList.AddFirst(movement);
    }

    public PersonState GetPlayerState()
    {
        return currentState;
    }


    #endregion

    #region HELPER FUNCTIONS

    void MovePlayer()
    {
        currentState = PersonState.MOVING;
        switch (movementList.First.Value)
        {
            case "UP": Tween.Position(gameObject.transform, nextPosition, Globals.Instance.tweenDuration, 0, Tween.EaseInOutStrong, Tween.LoopType.None, HandleTweenStarted, HandleTweenFinished); break;
            case "DOWN":
                Vector2 nextPos = gameObject.transform.position + new Vector3(0, -1f, 0);
                HandleTweenMovingDownStarted();
                //Tween.Position(gameObject.transform, nextPos, tweenDuration/2, 0, Tween.EaseInOutStrong, Tween.LoopType.None);
                Invoke("HandleTweenMovingDownFinished", .5f);
                break;
            case "RIGHT": Tween.Position(gameObject.transform, nextPosition, Globals.Instance.tweenDuration, 0, Tween.EaseOut, Tween.LoopType.None, HandleTweenStarted, HandleTweenFinished); break;
            case "LEFT": Tween.Position(gameObject.transform, nextPosition, Globals.Instance.tweenDuration, 0, Tween.EaseOut, Tween.LoopType.None, HandleTweenStarted, HandleTweenFinished); break;

            default:
                break;
        }
    }

    protected override void HandleTweenMovingDownStarted()
    {
        IsFreeFall = true;
        boxCollider.enabled = false;
    }

    protected override void HandleTweenMovingDownFinished()
    {
        boxCollider.enabled = true;
        if (movementList.Count > 0) movementList.RemoveFirst();
        
    }

    //Change later the name of the method
    protected override void HandleTweenStarted()
    {
        currentState = PersonState.MOVING;
    }

    //Called when one move is done
    protected override void HandleTweenFinished()
    {
        //Remove The move
        if (movementList.Count > 0) movementList.RemoveFirst();

        if (!CheckPlatformUnderneath())
        {
            IsFreeFall = true;
        }

        currentState = PersonState.IDLE;
     
    }
    
    //Get Position where to move 
    private Vector3 GetMovement(LinkedListNode<string> str)
    {
        switch (str.Value)
        {
            case "UP": isMovingDown = false; return gameObject.transform.position + Globals.Instance.upVector; 
            case "DOWN": isMovingDown = true; return gameObject.transform.position + Globals.Instance.downVector; 
            case "RIGHT": isMovingDown = false; return gameObject.transform.position + Globals.Instance.rightVector; 
            case "LEFT": isMovingDown = false; return gameObject.transform.position + Globals.Instance.leftVector;

            default:
                isMovingDown = false; return  gameObject.transform.position;
        }
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
                if (nextPosition.x > Globals.Instance.horizontalBoundary) return false;
                return true;
            case "LEFT":
                if (nextPosition.x < -Globals.Instance.horizontalBoundary) return false;
                return true;

            default: return true;
        }
    }

    private bool ValidateVerticalBoundary(Vector2 movement)
    {
        if (movement.y >= Globals.Instance.verticalBoundary || movement.y <= -Globals.Instance.verticalBoundary) return true;
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

    private void ClearList() => movementList.Clear();
    #endregion

    #region INTERFACE IMPLEMENTATION

    public void DestroyObject()
    {
        if(!isProtected) Death("DestroyObject");
    }

    #endregion

    #region CHARACTER INTERACTIONS

    void ActivateShield()
    {
        StartCoroutine(_ActivateShieldCoroutine());
    }

    void OnReviveClick()
    {
        gameObject.transform.position = initialPosition; //Vector2.zero;
        currentState = PersonState.IDLE;
        isDeadFromFall = false;
    }
    
    public override void Death(string txt)
    {
        LevelManager.Instance.ShowDefeatPanel(txt);

        base.Death(txt);

        //OnPlayerDieEvent?.Invoke();

        if ( !IsExtraLifeActive )
        {
            StopAllCoroutines();
            
            print("You have died");
        }

    }

    #endregion

}
