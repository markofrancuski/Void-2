using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HayPlatform : BasePlatform
{

    public override void Interact(PlayerController controller)
    {
        Debug.Log("Landed on Hay Platform!");
    }
}
