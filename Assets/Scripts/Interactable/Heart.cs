using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Interactable
{
    public override void Interact()
    {
        LevelManager.Instance.HeartCollected();
        base.Interact();
    }
}
