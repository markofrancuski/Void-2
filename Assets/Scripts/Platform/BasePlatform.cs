using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlatform : MonoBehaviour, IDestroyable
{

    public virtual void Interact(PlayerController controller)
    {
        if(controller.GetFallingFloorsHeight() >= 2)
        {
            controller.Death();
        }
    }

    public void DestroyObject()
    {
        gameObject.SetActive(false);
    }
}
