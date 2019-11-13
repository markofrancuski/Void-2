using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlatform : MonoBehaviour
{

    public PlatformType GetPlatformType()
    {
        return platformType;
    }

    [SerializeField] private PlatformType platformType;

    public virtual void Interact(PlayerController controller)
    {

    }

    public void SetUp(PlatformType platform)
    {
        platformType = platform;
    }

    public void DestroyPlatform()
    {
        gameObject.SetActive(false);
    }
}
