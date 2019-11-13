using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikePlatform : BasePlatform
{

    public override void Interact(PlayerController controller)
    {
        base.Interact(controller);
        controller.Death();
    }
}
