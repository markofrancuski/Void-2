using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HayPlatform : BasePlatform
{

    public override void Interact(Person controller)
    {
        //PrintObjectInteracting(controller, "Hay");

        if (controller.currentState != PersonState.STUNNED && controller.currentState != PersonState.DEAD) controller.currentState = PersonState.IDLE;
        controller.IsFreeFall = false;
        controller.isDeadFromFall = false;
    }
}
