using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovmentSetUp : MonoBehaviour
{
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
        Globals.Instance.movePaceHorizontal = newScale.x;
        Globals.Instance.movePaceVertical = newScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
