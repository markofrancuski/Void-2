using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlatform : MonoBehaviour, IDestroyable
{

    public PlatformType GetPlatformType()
    {
        return platformType;
    }

    [SerializeField] private PlatformType platformType;

    public virtual void Interact(PlayerController controller)
    {
        if(controller.GetFallingFloorsHeight() > 0)
        {
            controller.Death();
        }
    }

    public void SetUp(PlatformType platform)
    {
        platformType = platform;
    }

    public void DestroyObject()
    {
        gameObject.SetActive(false);
    }
}
