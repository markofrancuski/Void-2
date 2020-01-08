using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditor : EditorWindow 
{
    public static int levelNumber; 
    private int gridSize;
    private string saveLevelPath = "Assets/Prefabs/Level/";
    private Vector3 spaceBetweenPlatforms;
    private GameObject parentObject;
    private GameObject testObject;

    #region Resources
    [Header("Sprites")]
    [SerializeField] private Sprite platformNormalSprite;
    [SerializeField] private Sprite platformBreakableSprite;
    [SerializeField] private Sprite platformHaySprite;
    [SerializeField] private Sprite platformSpikeSprite;
    [SerializeField] private Sprite platformSlideSprite;
    [SerializeField] private Sprite platformDoubleSprite;

    [SerializeField] private Sprite coinSprite;
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Sprite weaponSprite;

    [Header("Textures")]
    [SerializeField] private Texture platformNormalTexture;
    [SerializeField] private Texture platformBreakableTexture;
    [SerializeField] private Texture platformHayTexture;
    [SerializeField] private Texture platformSpikeTexture;
    [SerializeField] private Texture platformSlideTexture;
    [SerializeField] private Texture platformDoubleTexture;

    [SerializeField] private Texture coinTexture;
    [SerializeField] private Texture heartTexture;
    [SerializeField] private Texture weaponTexture;
    [SerializeField] private Texture defaultTexture;
    #endregion

    #region Drop Down Variables
    /* private bool isPlatformClick = false;
     private bool isPickableClick = false;
     private int selectedPlatformIndex;*/
    private PlatformType selectedPlatformType;
    private PickableType selectedPickableType;

    #endregion

    private Dictionary<int, PlatformInfo> dictionary = new Dictionary<int, PlatformInfo>();

    private void Awake() 
    {
        Debug.Log("Loading Assets");

        #region Loading Assets
        
        testObject = Resources.Load<GameObject>("Test Sprite");
        //Sprites
        platformNormalSprite = Resources.Load<Sprite>("Sprites/Platforms/NormalPlatform");
        platformBreakableSprite = Resources.Load<Sprite>("Sprites/Platforms/BreakablePlatform");
        platformHaySprite = Resources.Load<Sprite>("Sprites/Platforms/HayPlatform");
        platformSpikeSprite = Resources.Load<Sprite>("Sprites/Platforms/SpikePlatform");
        platformSlideSprite = Resources.Load<Sprite>("Sprites/Platforms/SlidePlatform");

        coinSprite = Resources.Load<Sprite>("Sprites/Pickables/Coin");
        heartSprite = Resources.Load<Sprite>("Sprites/Pickables/Heart 1");
        weaponSprite = Resources.Load<Sprite>("Sprites/Pickables/RandomBox");

        //Texture
        platformNormalTexture = Resources.Load<Texture>("Sprites/Platforms/NormalPlatform");
        platformBreakableTexture = Resources.Load<Texture>("Sprites/Platforms/BreakablePlatform");
        platformHayTexture = Resources.Load<Texture>("Sprites/Platforms/HayPlatform");
        platformSpikeTexture = Resources.Load<Texture>("Sprites/Platforms/SpikePlatform");
        platformSlideTexture = Resources.Load<Texture>("Sprites/Platforms/SlidePlatform");
        platformDoubleTexture = Resources.Load<Texture>("Sprites/Platforms/DoublePlatform");

        coinTexture = Resources.Load<Texture>("Sprites/Pickables/Coin");
        heartTexture = Resources.Load<Texture>("Sprites/Pickables/Heart 1");
        weaponTexture = Resources.Load<Texture>("Sprites/Pickables/RandomBox");

        defaultTexture = Resources.Load<Texture>("Sprites/White1x1");

        #endregion

        gridSize = 5;

        selectedPlatformType = PlatformType.NONE;
        selectedPickableType = PickableType.NONE;
        spaceBetweenPlatforms = new Vector3(1 , 1, 0);

    }
    [MenuItem("Window/Custom Windows/LevelEditor")]

    public static void ShowWindow()
    {
        GetWindow<LevelEditor>("Level Generator");
    }

    private void OnGUI() 
    {
        #region Input Fields
        gridSize = EditorGUILayout.IntField("Enter Grid Size" ,gridSize);
        saveLevelPath = EditorGUILayout.TextField("Path To Save Level", saveLevelPath);
        spaceBetweenPlatforms = EditorGUILayout.Vector3Field("Enter the desired space between platforms",  spaceBetweenPlatforms);

        selectedPlatformType = (PlatformType) EditorGUILayout.EnumPopup("Select Platform Type ", selectedPlatformType);
        selectedPickableType = (PickableType) EditorGUILayout.EnumPopup("Select Pickable Type ", selectedPickableType);
        #endregion

        GUI.color = Color.yellow;
        //Spawn Level Button
        if (GUILayout.Button("Spawn Level"))
        {
            SpawnLevel();
            /*if(dictionary.Count > 0) SpawnLevel();
            else Debug.LogError("Error Grid is Empty!");*/
            
        }
        GUI.color = Color.white;

        GUI.color = Color.red;
        //Print the dictionary => What fields we filled up in a grid
        if(GUILayout.Button("Debug Clicked Platforms"))
        {
            foreach(KeyValuePair<int, PlatformInfo> kvp in dictionary)
            {
                Debug.Log(kvp.Value.ToString());
            }
        }
        GUI.color = Color.white;

        GUI.color = Color.green;
        //Clears out the dictionary
        if(GUILayout.Button("Reset Grid"))
        {
            dictionary.Clear();
        }
        GUI.color = Color.white;

        //Creates the grid
        CreateGrid(gridSize, 25, 25, 50, 200, "Level Grid", 25);

        // Shows the drop down menu to select the platform type
        /*if(isPlatformClick)
        {      
            selectedPlatformType = dictionary[selectedPlatformIndex].GetPlatformType();

            GUI.Box(new Rect(150, 400, 200, 50), "Platform Selection");
            selectedPlatformType = (PlatformType) EditorGUI.EnumPopup(new Rect(150, 420, 200, 50),"Select Platform", selectedPlatformType);

            //selectedPlatformType = (PlatformType) EditorGUILayout.EnumPopup("Select Platform", selectedPlatformType);

            if (dictionary[selectedPlatformIndex].GetPlatformType() != selectedPlatformType)
            {
                dictionary[selectedPlatformIndex].ChangePlatform(selectedPlatformType);

                isPlatformClick = false;
                //Debug.Log("Changed: " + selectedPlatformType);
                //Debug.Log(selectedPlatformIndex);   
            }
        }

        // Shows the drop down menu to selcet the pickable type
        if(isPickableClick)
        {
            selectedPickableType = dictionary[selectedPlatformIndex].GetPickableType();

            GUI.Box(new Rect(150, 400, 200, 50), "Pickable Selection");
            selectedPickableType = (PickableType) EditorGUI.EnumPopup(new Rect(150, 420, 200, 50),"Select Pickable", selectedPickableType);

            //selectedPickableType = (PickableType) EditorGUILayout.EnumPopup("Select Pickable", selectedPickableType);

            if(dictionary[selectedPlatformIndex].GetPickableType() != selectedPickableType)
            {
                dictionary[selectedPlatformIndex].ChangePickable(selectedPickableType);
                isPickableClick = false;
            }
        }
        */
    }

    private void CreateGrid(int gridSize, float buttonSize, float spaceBetween, float posX, float posY, string boxName, float boxOffset)
    {
        float initialPosY = posY;
        float initialPosX = posX;
        float boxSize2 = 10+ buttonSize*3 ; // *2

        float boxSize = gridSize * boxSize2 + (gridSize - 1) * spaceBetween/2;

        int buttonIndex = 1;
        GUI.Box(new Rect(posX - boxOffset, posY - boxOffset, boxSize + boxOffset*2, boxSize + boxOffset*2), boxName);

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GUI.Box(new Rect(initialPosX, initialPosY , boxSize2, boxSize2), "Platform " + buttonIndex);

                //Pickable Button
                if(dictionary.ContainsKey(buttonIndex))
                {
                    if (GUI.Button(new Rect(initialPosX, initialPosY + boxSize2 / 2 - 20, buttonSize*3, buttonSize), GetPickableTexture(dictionary[buttonIndex].GetPickableType())))
                    {
                        //if(isPlatformClick) isPlatformClick = false;

                        //selectedPlatformIndex = buttonIndex;
                        //dictionary[buttonIndex].ChangePickable(selectedPickableType);
                        //isPickableClick = true;

                        if (selectedPickableType == PickableType.NONE && dictionary[buttonIndex].GetPlatformType() == PlatformType.NONE)
                        {
                            dictionary.Remove(buttonIndex);
                        }
                        else
                        {
                            dictionary[buttonIndex].ChangePickable(selectedPickableType);
                        }
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(initialPosX, initialPosY + boxSize2 / 2 - 20, buttonSize*3, buttonSize), GetDefaultTexture()))
                    {
                        dictionary[buttonIndex] = new PlatformInfo(i, j, PlatformType.NONE, selectedPickableType);
                        //selectedPlatformIndex = buttonIndex;
                        //isPickableClick = true;
                    }
                }

                //Platform Button
                if(dictionary.ContainsKey(buttonIndex))
                {
                    if(GUI.Button(new Rect(initialPosX + boxSize2 / 2 - 41, initialPosY + boxSize2 / 2 + 5, buttonSize * 3, buttonSize), GetPlatformTexture(dictionary[buttonIndex].GetPlatformType()) ))
                    {
                        //if(isPickableClick) isPickableClick = false;
                        
                        //selectedPlatformIndex = buttonIndex;
                        //dictionary[buttonIndex].ChangePlatform(selectedPlatformType);
                        //isPlatformClick = true;

                        if (selectedPlatformType == PlatformType.NONE && dictionary[buttonIndex].GetPickableType() == PickableType.NONE)
                        {
                            dictionary.Remove(buttonIndex);
                        }
                        else
                        {
                            dictionary[buttonIndex].ChangePlatform(selectedPlatformType);
                        }
                    }
                }
                else
                {
                   
                    if (GUI.Button(new Rect(initialPosX + boxSize2 / 2 - 41, initialPosY + boxSize2 / 2 + 5, buttonSize * 3, buttonSize), GetDefaultTexture()))
                    {

                        dictionary[buttonIndex] = new PlatformInfo(i, j, selectedPlatformType, PickableType.NONE);
                        //isPlatformClick = true;
                        //selectedPlatformIndex = buttonIndex;
                    }
                }

                initialPosX += buttonSize + spaceBetween + boxSize2 / 2;
                buttonIndex++;
            }
            initialPosY += buttonSize + spaceBetween + boxSize2 / 2 ;
            initialPosX = posX;
        }
        GUI.color = Color.white;

    }

    private void SpawnLevel()
    {

        levelNumber++;
        GameObject parentObject = new GameObject();
        parentObject.name = "Level " + levelNumber;

        parentObject.transform.position = Vector3.zero;

        /*foreach(KeyValuePair<int, PlatformInfo> kvp in dictionary)
        {
            GameObject pickable = resource.GetPickablePrefab(kvp.Value.GetPickableType());
            GameObject platform = resource.GetPlatformPrefab(kvp.Value.GetPlatformType());
            Vector3 offset = new Vector3(0, 0 ,0);
            
            //Adds offset based on the position of the platform
            if(kvp.Value.GetPositionX() > 0) offset += new Vector3(spaceBetweenPlatforms.x*kvp.Value.GetPositionX(), 0, 0);
            if(kvp.Value.GetPositionY() > 0) offset += new Vector3(0, -spaceBetweenPlatforms.y*kvp.Value.GetPositionY(), 0);

            if(platform != null) Instantiate(platform, kvp.Value.GetPosition(false)+ offset, Quaternion.identity, parentObject.transform);
            if(pickable != null) Instantiate(pickable, kvp.Value.GetPosition(true)+ offset, Quaternion.identity, parentObject.transform);         
        }

        PrefabUtility.SaveAsPrefabAsset(parentObject, saveLevelPath + parentObject.name+ ".prefab");
        GameObject.DestroyImmediate(parentObject);
    */

        //Adding Script
        parentObject.AddComponent<Level>();

        parentObject.transform.position = Vector3.zero;
        List<GameObject> tempList = new List<GameObject>();

        for (int i = 0; i < 25; i++)
        {
            GameObject gm = Instantiate(testObject, Vector3.zero, Quaternion.identity, parentObject.transform);

            if(dictionary.ContainsKey(i+1))
            {
                PlatformType platformType = dictionary[i+1].GetPlatformType();

                PickableType pickableType = dictionary[i+1].GetPickableType();

                if(platformType != PlatformType.NONE)
                {
                    gm.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = GetPlatformSprite(platformType);
                    gm.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    AttachPlatformScript(platformType, gm.gameObject.transform.GetChild(1).gameObject);
                }
                else
                {
                    gm.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                }

                if(pickableType != PickableType.NONE)
                {
                    gm.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GetPickableSprite(pickableType);
                    gm.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    AttachInteractableScript(pickableType, gm.gameObject.transform.GetChild(0).gameObject);
                }
                else
                {
                    gm.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                }

            }
            else
            {
                gm.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                gm.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            }

            tempList.Add(gm);
        }

        parentObject.GetComponent<Level>().AddList(tempList);
        PrefabUtility.SaveAsPrefabAsset(parentObject, saveLevelPath + parentObject.name + ".prefab");
        DestroyImmediate(parentObject);
    }

    #region Helper Functions

    private void AttachInteractableScript(PickableType pickableType, GameObject go)
    {
        go.GetComponent<BoxCollider2D>().size = Vector2.one;
        switch (pickableType)
        {
            case PickableType.HEART:
                go.AddComponent<Heart>();
                break;
            case PickableType.COIN:
                go.AddComponent<Coin>();
                break;
            case PickableType.WEAPON:
                go.AddComponent<RandomWeapon>();
                break;
            default:
                break;
        }
    }

    private void AttachPlatformScript(PlatformType platformType, GameObject go)
    {
        BasePlatform script;
        go.layer = 8;
        go.GetComponent<BoxCollider2D>().size = new Vector2(1, 0.5f);
        switch (platformType)
        {
            case PlatformType.NORMAL:
                script = go.AddComponent<BasePlatform>();
                break;
            case PlatformType.HAY:
                script = go.AddComponent<HayPlatform>();
                break;
            case PlatformType.SPIKE:
                script = go.AddComponent<SpikePlatform>();
                break;
            case PlatformType.BREAKABLE:
                script = go.AddComponent<BreakablePlatform>();
                break;
            case PlatformType.SLIDE:
                script = go.AddComponent<SlidePlatform>();
                break;
            case PlatformType.DOUBLE:
                script = go.AddComponent<DoublePlatform>();
                break;

            default:
                break;
        }
    }

    private Sprite GetPlatformSprite(PlatformType platformType)
    {
        switch(platformType)
        {
            case PlatformType.NORMAL: return platformNormalSprite;
            case PlatformType.BREAKABLE: return platformBreakableSprite;
            case PlatformType.HAY: return platformHaySprite;

            case PlatformType.SLIDE: return platformSlideSprite;
            case PlatformType.SPIKE: return platformSpikeSprite;
            case PlatformType.DOUBLE: return platformNormalSprite;

            default: return null;
        }
    }

    private Sprite GetPickableSprite(PickableType pickableType)
    {
        switch(pickableType)
        {
            case PickableType.COIN: return coinSprite;
            case PickableType.HEART: return heartSprite;
            case PickableType.WEAPON: return weaponSprite;
            default: return null;
        }
    }

    private Texture GetPlatformTexture(PlatformType platformType)
    {
        switch(platformType)
        {
            case PlatformType.NORMAL: return platformNormalTexture;
            case PlatformType.BREAKABLE: return platformBreakableTexture;
            case PlatformType.HAY: return platformHayTexture;

            case PlatformType.SLIDE: return platformSlideTexture;
            case PlatformType.SPIKE: return platformSpikeTexture;
            case PlatformType.DOUBLE: return platformDoubleTexture;

            case PlatformType.NONE: return defaultTexture;
            default: return defaultTexture;
        }
    }

    private Texture GetPickableTexture(PickableType pickableType)
    {
        switch(pickableType)
        {
            case PickableType.COIN: return coinTexture;
            case PickableType.HEART: return heartTexture;
            case PickableType.WEAPON: return weaponTexture;
            default: return defaultTexture;
        }
    }

    private Texture GetDefaultTexture()
    {
        return defaultTexture;
    }

    #endregion

}
public class PlatformInfo 
{

    private int posX;
    private int posY;
    private PlatformType platformType;
    private PickableType pickableType;  
    public PlatformInfo( int posX, int posY, PlatformType platformType, PickableType pickableType)
    {
        this.posX = posY;
        this.posY = posX;
        this.platformType = platformType;
        this.pickableType = pickableType;
    }
    
    public float GetPositionX() => posX;
    public float GetPositionY() => posY;

    public void ChangePlatform(PlatformType newPlatformType)
    {
        this.platformType = newPlatformType;
    }
    public void ChangePickable(PickableType newPickableType)
    {
        this.pickableType = newPickableType;
    }
    public PlatformType GetPlatformType()
    {
        return platformType;
    }
    public PickableType GetPickableType()
    {
        return pickableType;
    }
    public Vector3 GetPosition(bool isPickable)
    {
        if(!isPickable) return new Vector3(posX, -posY, 0);
        else return new Vector3(posX, -posY + 0.5f, 0);
    }

    public override string ToString()
    {
        return "( " + posX  + "," + posY + " ) Is Type : " + platformType.ToString() + " and Has picklable: " + pickableType.ToString();
    }


}
