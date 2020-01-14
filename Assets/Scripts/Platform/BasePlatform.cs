using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlatform : MonoBehaviour, IDestroyable
{
    public delegate void FreeFallPlayerEvent();
    private event FreeFallPlayerEvent OnFreeFallPlayerHandler;

    public delegate void RecalculateDistanceEvent();
    private event RecalculateDistanceEvent OnRecalculateDistanceEvent;

    public virtual void Interact(Person controller)
    {
        //PrintObjectInteracting(controller, "Normal");

        if (controller.IsFreeFall && controller.isDeadFromFall)
        {
            controller.Death("Height");
        }
        if (controller.currentState != PersonState.STUNNED && controller.currentState != PersonState.DEAD) controller.currentState = PersonState.IDLE;
        controller.IsFreeFall = false;
    }

    #region UNITY FUNCTIONS

    /// <summary>
    /// Prints out the interaction when something touches the platform.
    /// </summary>
    /// <param name="controller"> Name of the player(Tim, Annie, AI, PVP Player).</param>
    /// <param name="platformName"> Name of the platform Player touched. </param>
    public void PrintObjectInteracting(Person controller, string platformName)
    {
        Debug.Log($"Object: {controller.gameObject.name} is interacting with {platformName} platform!");
    }

    Person controller;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") || collision.collider.CompareTag("AI"))
        {
            controller = collision.collider.GetComponent<Person>();
            Interact(controller);
            OnFreeFallPlayerHandler += controller.EnterInFreeFall;
        }

    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("AI"))
        {
            Person controller = collision.collider.GetComponent<Person>();
            OnFreeFallPlayerHandler -= controller.EnterInFreeFall;
        }
    }

    public float distanceFromUpperPlatform;
    public float distanceFromUnderneathPlatfrom;

    private void OnEnable()
    {
        GetVerticalPlatformDistance();
        SubscribeSurroundingPlatforms();
    }

    private void OnDisable()
    {
        UnsubscribeSurroundingPlatforms();
        //If player is on platform and platform gets destroyed => change to true
        if(controller != null) controller.IsFreeFall = true;
    }

    #endregion

    void SubscribeSurroundingPlatforms()
    {
        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position + new Vector3(0, 0.2f, 0), Vector2.up, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent += hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;

        hit = Physics2D.Raycast(transform.position + new Vector3(0, -0.2f, 0), Vector2.down, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent += hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;

        hit = Physics2D.Raycast(transform.position + new Vector3 (0.2f, 0, 0), Vector2.right, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent += hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;

        hit = Physics2D.Raycast(transform.position + new Vector3(0.2f, 0, 0), Vector2.left, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent += hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;
    }

    void UnsubscribeSurroundingPlatforms()
    {
        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position + new Vector3(0, 0.2f, 0), Vector2.up, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent -= hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;

        hit = Physics2D.Raycast(transform.position + new Vector3(0, -0.2f, 0), Vector2.down, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent -= hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;

        hit = Physics2D.Raycast(transform.position + new Vector3(0.2f, 0, 0), Vector2.right, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent -= hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;

        hit = Physics2D.Raycast(transform.position + new Vector3(0.2f, 0, 0), Vector2.left, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent -= hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;

    }

    void GetVerticalPlatformDistance()
    {
        RaycastHit2D hit;

        //Up
        hit = Physics2D.Raycast(transform.position + new Vector3(0, 0.2f, 0), Vector2.up, 20f, 1 << 8);
        if (hit) distanceFromUpperPlatform = hit.distance;
        else distanceFromUpperPlatform = 20;

        //Down
        hit = Physics2D.Raycast(transform.position + new Vector3(0, -0.2f, 0), Vector2.down, 20f, 1 << 8);
        if (hit) distanceFromUnderneathPlatfrom = hit.distance;
        else distanceFromUnderneathPlatfrom = 20;
    }
    
    //If Player is on this platform and gets destroyed => enter free fall
    public void DestroyObject()
    {
        //If this platform is destroyed send notification to surrounding subscribed platform to recalculate the distance 
        OnRecalculateDistanceEvent?.Invoke();
        OnFreeFallPlayerHandler?.Invoke();
        gameObject.SetActive(false);
    }

}