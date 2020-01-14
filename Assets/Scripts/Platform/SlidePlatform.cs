using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidePlatform : BasePlatform
{
    public delegate void OnSlidePlatformInteract(string str);
    public static event OnSlidePlatformInteract OnSlidePlatformInteractEvent;

    int rnd;
    public override void Interact(Person controller)
    {
        //PrintObjectInteracting(controller, "Slide");
        if (controller.IsFreeFall && distanceFromUpperPlatform +0.4f >= Globals.Instance.movePaceHorizontal)
        {

        }
        //{
            //Start sliding player
        int rnd = Random.Range(0, 2);

        if (rnd == 0)
        {
            //Move Right
            OnSlidePlatformInteractEvent?.Invoke("RIGHT");
        }
        else
        {
            //Move Left
            OnSlidePlatformInteractEvent?.Invoke("LEFT");
        }
        controller.IsFreeFall = false;
        controller.currentState = PersonState.IDLE;
        //}
        //else controller.Death();
    }



}
