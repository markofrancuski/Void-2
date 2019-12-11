using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePlatform : BasePlatform
{

    [SerializeField] private BasePlatform firstPlatform;
    [SerializeField] private BasePlatform secondPlatform;

    private void Start()
    {
        //firstPlatform = gameObject.transform.GetChild(0).GetComponent<BasePlatform>();
        //secondPlatform = gameObject.transform.GetChild(1).GetComponent<BasePlatform>();

        firstPlatform.gameObject.SetActive(true);
        secondPlatform.gameObject.SetActive(false);
    }

    public override void Interact(Person controller)
    {
        
        if (firstPlatform.gameObject.activeInHierarchy)
        {
            firstPlatform.Interact(controller);
            secondPlatform.gameObject.SetActive(true);
            firstPlatform.gameObject.SetActive(false);
        }
        else
        {
            secondPlatform.Interact(controller);
            secondPlatform.gameObject.SetActive(false);
            firstPlatform.gameObject.SetActive(true);
        }
    }
}
