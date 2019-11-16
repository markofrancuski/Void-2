using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] private GameObject GameOverPanel;

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
       coinValueText.SetText(coinValue.currentValue.ToString());
     
    }

    private void OnPlayerDeath()
    {
        GameOverPanel.SetActive(true);
    }

    public void OnResetButtonClick()
    {
        SceneManager.LoadScene(0);
    }

    public void AddCoins(int amount)
    {
        coinValue.currentValue += amount;
        coinValueText.SetText(coinValue.currentValue.ToString());
    }


}
