using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class Level : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects;
    private float tempX;
    private float tempY;

    public float moveX;
    public float moveY;

    private Vector3 nextPos = new Vector3(0, 0, 0);
    [SerializeField] private float speed;
    private float disablePoint;

    private void Start() 
    {
        //Calculates the position Y so that terrain can disable itself when it reaches the position Y
        disablePoint = -(Camera.main.orthographicSize * 2);
        StartLevel();
    }

    private void FixedUpdate() 
    {
        /* if(TerrainManager.instance.isStarted) 
        {
            nextPos = transform.position + new Vector3(0, movePosY, 0);  
            Tween.Position(transform , transform.position, nextPos, speed, 0 );

            if(transform.position.y <= disablePoint) 
            {
                 gameObject.SetActive(false);
                 //TerrainManager.instance.spawnedLevels.Remove(gameObject);
                 //Destroy(gameObject);
            }    
        }
        */
    }

    public void AddList(List<GameObject> newList)
    {
        objects = newList;
    }



    public void StartLevel()
    {     
        //Get the sprite width in world space units
        float worldSpriteWidth = objects[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        //Get the screen height & width in world space units
        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = (worldScreenHeight / Screen.height) * Screen.width;

        //Initialize new scale to the current scale
        Vector3 newScale = transform.localScale;
        
        //Divide screen width by sprite width, set to X axis scale
        newScale.x = worldScreenWidth / worldSpriteWidth * 0.2f;
        newScale.y = worldScreenHeight / 1 * 0.2f;
        //0.2f  = 5 grid space
        moveX = newScale.x;
        moveY = newScale.y;

        //Position of the starting grid cell
        float posX = - (newScale.x * 2);
        float posY = newScale.y * 2;

        tempX = posX;
        tempY = posY;
         
        int gridCell = 5;
        //First Cells platform and pickable
        GameObject pickableGO = objects[0].transform.GetChild(0).transform.gameObject;
        GameObject platformGO = objects[0].transform.GetChild(1).transform.gameObject;
        
        #region Same Size Code
        if(pickableGO.activeInHierarchy)
        {
            pickableGO.transform.parent = null;
        }
        
        if(platformGO.activeInHierarchy)
        {
            platformGO.transform.parent = null;
        }
        
        #endregion

        //spritePickable.Scale.x *= scale.y;
        //Scale Sprites 
        #region Scaling Sprites Code

        /*if(pickableGO.activeInHierarchy)
        {
            Vector3 pickableScale = pickableGO.transform.localScale;
            pickableScale.x *= (newScale.y/2);
            pickableScale.y *= (newScale.x/2);
            pickableGO.transform.localScale = pickableScale;
        }
        
        
        if(platformGO.activeInHierarchy)
        {
            Vector3 platformScale = platformGO.transform.localScale;
            //platformScale.x *= newScale.x;
            platformScale.y *= (newScale.y/2);
            platformGO.transform.localScale = platformScale;
        }
        */

        #endregion
        
        //Setting the first cells position and scale
        objects[0].transform.localScale = newScale;
        objects[0].transform.localPosition = new Vector3(tempX, tempY, 1);
        
        #region Same Size Code
        
        if(pickableGO.activeInHierarchy)
        {
            pickableGO.transform.SetParent(objects[0].transform);
            pickableGO.transform.localPosition = new Vector3(0, 0.2f, 0);
        }
              
        if(platformGO.activeInHierarchy)
        {
            platformGO.transform.SetParent(objects[0].transform);
            platformGO.transform.localPosition = new Vector3(0, -0.2f, 0);
            Vector3 childScale = platformGO.transform.localScale;
            childScale.x = 0.75f;
            platformGO.transform.localScale = childScale; // Set the platform Scale.X = 1 (width of the parent). Scale.Y will be scaled based on the parent 
        }

        #endregion
        
        tempX += newScale.x;

        for (int i = 1; i < 25; i++)
        {     
            //Getting the childs of the Grid => Pickable, Platform
            pickableGO = objects[i].transform.GetChild(0).transform.gameObject;         
            platformGO = objects[i].transform.GetChild(1).transform.gameObject;
            
            #region Same Size Code
            if(pickableGO.activeInHierarchy) 
            {
                pickableGO.transform.parent = null;                    
            }
            
            if(platformGO.activeInHierarchy)
            {
                platformGO.transform.parent = null;
            }
            #endregion
      
            //Setting the position of a grid cell and scaling it even;
            objects[i].transform.localScale = newScale;
            objects[i].transform.localPosition = new Vector3(tempX, tempY,1); 

            #region Scaling Sprites Code
            // Scale Pickable for the half the scale parent
            /*if(pickableGO.activeInHierarchy)
            {
                Vector3 pickableScale = pickableGO.transform.localScale;
                pickableScale.x *= (newScale.y);
                pickableScale.y *= (newScale.x);
                pickableGO.transform.localScale = pickableScale;
            }
            */
            #endregion

            #region Same Size Code
            if(pickableGO.activeInHierarchy)
            {
                pickableGO.transform.SetParent(objects[i].transform);
                pickableGO.transform.localPosition = new Vector3(0, 0.2f, 0);
            }
            
            if(platformGO.activeInHierarchy)
            {
                platformGO.transform.SetParent(objects[i].transform);
                platformGO.transform.localPosition = new Vector3(0, -0.2f, 0);
                Vector3 childScale = platformGO.transform.localScale;
                childScale.x = 0.75f;
                platformGO.transform.localScale = childScale; // If the parent new scale.x is < 1f => Set the child to the witdh of the parent(x = 1); 
            }
            #endregion

            if(i+1 == gridCell )
            {            
                tempX = posX;              
                tempY -= newScale.y;    
                gridCell += 5;                                                  
            }
            else
            {
                
                tempX += newScale.x;          
            }
        }
    }

    private void OnDisable() 
    {       
        TerrainManager.instance.spawnedLevels.Remove(gameObject);
    }
}
