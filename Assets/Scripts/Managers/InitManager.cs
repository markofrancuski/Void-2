using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MEC;

public class InitManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Timing.RunCoroutine(_LoadMainMenu().CancelWith(this.gameObject));
    }
    [SerializeField] private GameObject splashBackground;

    private IEnumerator<float> _LoadMainMenu()
    {
        yield return Timing.WaitForSeconds(4.2f);

        SceneManager.sceneLoaded += OnSceneLoaded;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return Timing.WaitForOneFrame;
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        splashBackground.SetActive(false);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
