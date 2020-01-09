using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : Singleton<GameManager>
{

    #region DELEGATES/EVENTS


    public delegate bool OnPlayerCheckLifeSaver();
    public static event OnPlayerCheckLifeSaver CheckPlayerLifeSaverEvent;

    public delegate void OnReviveButtonClicked();
    public static event OnReviveButtonClicked ReviveButtonClickedEvent;

    public delegate void OnResetScene();
    public static event OnResetScene ResetSceneEvent;
   
    #endregion 

    [Header("UI REFERENCES")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject revivePanel;
    [SerializeField] private GameObject extraLifeImage;

    [SerializeField] private IntValue coinValue;
    [SerializeField] private TextMeshProUGUI coinValueText;

    private void OnEnable()
    {
        PlayerController.OnPlayerDieEvent += OnPlayerDeath;

    }
    private void OnDisable()
    {
        PlayerController.OnPlayerDieEvent -= OnPlayerDeath;
    }

    private void Start()
    {
        //coinValueText.SetText(coinValue.currentValue.ToString());

        //UpdateExtralifeImage();
    }

    void UpdateExtralifeImage()
    {
        if ((bool)!CheckPlayerLifeSaverEvent?.Invoke()) extraLifeImage.SetActive(false);
        else extraLifeImage.SetActive(true);
    }

    private void OnPlayerDeath()
    {
        if ((bool)CheckPlayerLifeSaverEvent?.Invoke()) Debug.Log("Player is dead => Revive player");//revivePanel.SetActive(true);
        else Debug.Log("Player is dead => gameOver");//gameOverPanel.SetActive(true);
    }

    public void OnReviveButtonClick()
    {
        revivePanel.SetActive(false);
        ReviveButtonClickedEvent?.Invoke();

        UpdateExtralifeImage();
        revivePanel.SetActive(false);
    }

    public void OnResetButtonClick()
    {
        ResetSceneEvent?.Invoke();
        SceneManager.LoadScene(0);
    }

    public void AddCoins(int amount)
    {
        coinValue.currentValue += amount;
        coinValueText.SetText(coinValue.currentValue.ToString());
    }


}
