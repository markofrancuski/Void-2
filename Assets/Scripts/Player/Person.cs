using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//State Machine
public enum PersonState { IDLE, MOVING, DEAD, INTERACTING, STUNNED };

public class Person : MonoBehaviour
{

    #region COMPONENTS
    [Header("SUPER-CLASS")]
    [Header("COMPONENTS")]
    public BoxCollider2D boxCollider;

    #endregion

    #region PERSON VARIABLES
    public int Team;

    public bool IsStunned;

    #endregion

    #region MOVEMENT VARAIBLES
    [Header("MOVEMENT VARAIBLES")]
    public  Vector3 initialPosition;
    public PersonState currentState;

    public bool isDeadFromFall;
    public bool isMovingDown;

    [SerializeField] private bool isFreeFall;
    public bool IsFreeFall {
        get { return isFreeFall; }
        set {
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

    //Movement 
    public Vector3 nextPosition;

    #endregion

    #region MOVEMENT FUNCTIONS

    void CheckPlayerFalling(RaycastHit2D[] hits)
    {

        //Is there platforms under the player
        if (hits.Length > 0)
        {
            //Debug.Log("Globals.Instance.movePaceHorizontal * 2: " + Globals.Instance.movePaceHorizontal * 2);

            //If there is more then one platform => raycast will hit the direct platform that is player moving from => get platform in grid under
            if (hits.Length > 1 && isMovingDown)
            {
                //Debug.Log("hits[1].distance: " + hits[1].distance);
                if (hits[1].distance >= Globals.Instance.movePaceHorizontal * 2) isDeadFromFall = true;
                else isDeadFromFall = false;
                isMovingDown = false;
            }
            //There is only one platform under => Jumped up or on sides check first platform under player
            else
            {
                //Debug.Log("hits[0].distance: " + hits[0].distance);
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

    public void EnterInFreeFall()
    {
        IsFreeFall = true;
    }

    #endregion

    #region UNITY FUNCTIONS
    // Start is called before the first frame update
    public virtual void Start()
    {
        //Debug.Log($"Person start! {gameObject.name} ");
        initialPosition = gameObject.transform.position;
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            int team = other.GetComponent<Projectile>().Team;
            if (team != Team)
            {
                Debug.Log($"GO from team: {team} has stunned GO from team: {Team}");
                Stun();
            } 
            else  Debug.Log($"Cannot stun yourself!");
        }
    }

    #endregion
    
    public virtual void Death(string txt)
    {
        Debug.Log("Death from: " + txt);
        currentState = PersonState.DEAD;
    }

    protected virtual void HandleTweenStarted()
    {
        currentState = PersonState.MOVING;
    }

    protected virtual void HandleTweenFinished()
    {

    }

    protected virtual void HandleTweenMovingDownStarted()
    {
        IsFreeFall = true;
        boxCollider.enabled = false;
    }

    protected virtual void HandleTweenMovingDownFinished()
    {
        boxCollider.enabled = true;

    }

    protected virtual void Stun()
    {        
        if(!IsStunned) StartCoroutine(_StunPersonCoroutine());
    }

    #region HELPER FUNCTIONS

    #endregion

    #region COROUTINES

    IEnumerator _StunPersonCoroutine()
    {
        IsStunned = true;
        currentState = PersonState.STUNNED;

        yield return new WaitForSeconds(3f);

        currentState = PersonState.IDLE;
        IsStunned = false;

    }

    #endregion
}
