using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikePlatform : BasePlatform
{

    public override void Interact(Person controller)
    {
        //PrintObjectInteracting(controller, "Spike");
        controller.Death("Spike Platform");
    }
}
