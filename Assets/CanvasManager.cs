using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class CanvasManager : Singleton<CanvasManager>
{
    public GameObject mainMenuPanel;
    public GameObject statsPanel;

    public void ShowMainMenu() { mainMenuPanel.SetActive(true); statsPanel.SetActive(false); }
    public void HideMainMenu() { mainMenuPanel.SetActive(false); statsPanel.SetActive(true); }
   
}
