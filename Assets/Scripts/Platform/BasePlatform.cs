using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlatform : MonoBehaviour, IDestroyable
{
    public delegate void FreeFallPlayerEvent();
    private event FreeFallPlayerEvent OnFreeFallPlayerHandler;

    public delegate void RecalculateDistanceEvent();
    private event RecalculateDistanceEvent OnRecalculateDistanceEvent;

    public virtual void Interact(PlayerController controller)
    {
        Debug.Log("Landed on Normal platform");
        if (controller.IsFreeFall && controller.isDeadFromFall)
        {
            controller.Death();
        }
        controller.currentPlayerState = PlayerState.IDLE;
        controller.IsFreeFall = false;
    }

    #region UNITY FUNCTIONS

    PlayerController controller;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            controller = collision.collider.GetComponent<PlayerController>();
            Interact(controller);
            OnFreeFallPlayerHandler += controller.EnterInFreeFall;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerController controller = collision.collider.GetComponent<PlayerController>();
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
        if(controller != null)
        {
            controller.IsFreeFall
        }
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