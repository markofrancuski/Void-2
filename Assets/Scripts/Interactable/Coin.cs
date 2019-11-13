using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Interactable
{
    public override void Interact()
    {
        GameManager.Instance.AddCoins(1);
        base.Interact();
    }
}
