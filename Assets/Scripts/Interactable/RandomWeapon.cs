using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWeapon : Interactable
{

    public override void Interact()
    {
        GetRandomWeapon();
        base.Interact();
    }

    private void GetRandomWeapon()
    {
        int rnd = Random.Range(0, 3);
        InteractableManager.Instance.AddInteractable(rnd, 1);
    }
}
