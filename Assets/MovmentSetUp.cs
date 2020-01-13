using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.SceneManagement;
using TMPro;

public class MovmentSetUp : MonoBehaviour
{
    [SerializeField] private Animator canvasAnimatior;
    [SerializeField] private TextMeshProUGUI chapterTitle;
    // Start is called before the first frame update
    void Start()
    {
        float worldSpriteWidth = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        //Get the screen height & width in world space units
        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = (worldScreenHeight / Screen.height) * Screen.width;

        //Initialize new scale to the current scale
        Vector3 newScale = transform.localScale;

        //Divide screen width by sprite width, set to X axis scale
        newScale.x = worldScreenWidth / worldSpriteWidth * 0.2f;
        newScale.y = worldScreenHeight / 1 * 0.2f;
        newScale.y = newScale.y - 0.4f;

        //0.2f  = 5 grid space
        Debug.Log("Move Pace Setup!");
        Globals.Instance.movePaceHorizontal = newScale.x;
        Globals.Instance.movePaceVertical = newScale.y;
              
        Globals.Instance.upVector = new Vector3(0, newScale.y, 0);
        Globals.Instance.downVector = new Vector3(0, -newScale.y, 0);
        Globals.Instance.rightVector = new Vector3(newScale.x, 0, 0);
        Globals.Instance.leftVector = new Vector3(-newScale.x, 0, 0);

        //Set the boundary size
        Globals.Instance.horizontalBoundary = (newScale.x * 5f) / 2;
        Globals.Instance.verticalBoundary = (newScale.y * 5f) / 2;

    }

    public void OnChapterClicked(int chapterNumber)
    {
        Globals.Instance.currentChapter = chapterNumber;
        chapterTitle.SetText("Chapter " + chapterNumber);
        SetUpButtons();
        levelSelection.SetActive(true);
    }

    private bool isClicked = false;

    public void OnLevelClicked(int levelNumber)
    {
        if (isClicked) return;

        isClicked = true;
        Globals.Instance.currentLevel = levelNumber;
        Timing.RunCoroutine(_LoadLevel().CancelWith(this.gameObject));
        //Play the switching animation and load the scene async
    }

    public void OnBackButtonClicked()
    {
        Globals.Instance.currentChapter = -1;
        Globals.Instance.currentLevel = -1;
        levelSelection.SetActive(false);
    }

    private IEnumerator<float> _LoadLevel()
    {
        Debug.Log("Loading Level");
        canvasAnimatior.SetBool("Load", true);
        AsyncOperation operation = SceneManager.LoadSceneAsync("Level_"+ Globals.Instance.currentChapter + "_" + Globals.Instance.currentLevel);
        operation.allowSceneActivation = false;

        while(!operation.isDone)
        {
            if (operation.progress >= .9f) break;
            yield return Timing.WaitForOneFrame;
        }

        yield return Timing.WaitForSeconds(2.3f);
        operation.allowSceneActivation = true;                 
    }

    [SerializeField] private GameObject[] buttons;
    [SerializeField] private GameObject levelSelection;

    void SetUpButtons()
    {
        int val = Globals.Instance.GetNumberOfLevelsInCurrentChapter();
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i + 1 <= val) buttons[i].SetActive(true);
            else buttons[i].SetActive(false);
        }
    }
}
