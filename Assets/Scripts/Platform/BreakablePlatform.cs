using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : BasePlatform
{
    private void EnablePlatform()
    {
        gameObject.SetActive(true);
    }

    public override void Interact(PlayerController controller)
    {
        base.Interact(controller);
        if (controller.GetFreeze())
        {
            gameObject.SetActive(false);
            Invoke("EnablePlatform", 2f);
        }
        
        //base.Interact(controller);
    }
 
}
