using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidePlatform : BasePlatform
{
    public override void Interact(PlayerController controller)
    {
        //Start sliding player
        int rnd = Random.Range(0, 2);

        // Move player right
        if(rnd == 0)
        {

        }
        //Move player left
        else
        {

        }


    }

    
}
