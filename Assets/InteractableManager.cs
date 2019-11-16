using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using TMPro;

public class InteractableManager : Singleton<InteractableManager>
{

    public delegate bool OnShieldActiveCheck();
    public static event OnShieldActiveCheck OnShieldActiveCheckEvent;

    public delegate void OnShieldActivate();
    public static event OnShieldActivate OnShieldActivateEvent;

    [SerializeField] private Transform playerTransform;
    private void Start()
    {
        bombText.SetText(bomb.ToString());
        stunText.SetText(stun.ToString());
        shieldText.SetText(shield.ToString());
    }

    [SerializeField] private int bomb;
    public int Bomb
    {
        get { return bomb; }
        set { bomb = value; bombText.SetText(bomb.ToString()); }
    }

    [SerializeField] private int stun;
    public int Stun
    {
        get { return stun; }
        set { stun = value; stunText.SetText(stun.ToString()); }
    }

    [SerializeField] private int shield;
    public int Shield
    {
        get { return shield; }
        set { shield = value; shieldText.SetText(shield.ToString()); }
    }

    [SerializeField] private TextMeshProUGUI bombText;
    [SerializeField] private TextMeshProUGUI stunText;
    [SerializeField] private TextMeshProUGUI shieldText;

    [SerializeField] private GameObject bombPrefab;

    public void AddInteractable(int index, int amount)
    {
        //If there player has Weapon boost => Recieves 2 random weapon. If player has weapon boost and its active reveice all three weapons
        switch (index)
        {
            case 0:
                Bomb++;
                Debug.Log("Got bomb!");
                break;
            case 1:
                Stun++;
                Debug.Log("Got Stun Projectile!");
                break;
            case 2:
                Shield++;
                Debug.Log("Got Shield!");
                break;
            default:
                break;
        }
    }

    public void OnInteractableItemClicked(int index)
    {
        switch (index)
        {
            case 0:
                if (Bomb > 0)
                {
                    //Retrieve bomb from pooler
                    Bomb--;
                    GameObject go = Instantiate(bombPrefab, playerTransform.position, Quaternion.identity);
                    go.SetActive(true);
                }
                break;
            case 1:
                if (Stun > 0)
                {
                    Stun--;
                }
                break;
            case 2:
                if (Shield > 0)
                {
                    if (OnShieldActiveCheckEvent != null)
                    {
                        if (!OnShieldActiveCheckEvent.Invoke())
                        {
                            OnShieldActivateEvent?.Invoke();
                            Shield--;
                        }
                    }

                    //Shield--;
                }
                break;
            default:
                break;
        }
    }
}
