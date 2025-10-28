using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SecretDoorPuzzleManager : MonoBehaviour
{
    [SerializeField] List<LevelData> levels;

    GameObject currentBackground;
    int currentLevel = 0;      
    int currentProgress = 0;
    int winCon;

    [SerializeField] GameObject barricadePrefab;
    [SerializeField] Transform barricadeParent;

    [SerializeField] GameObject buttonPrefab;
    [SerializeField] Transform buttonParent;
    
    public Transform BGParent;

    [SerializeField] Sprite buttonOne;
    [SerializeField] Sprite buttonTwo;
    [SerializeField] Sprite buttonThree;

    [SerializeField] Animator animator;


    public static SecretDoorPuzzleManager instance;


    //Grid Values
    [HideInInspector] public Vector3 blockBounds;
    [HideInInspector] public Vector2 gridOrigin;
    [HideInInspector] public Vector3 incrementAmount;
    [HideInInspector] public Vector2 buttonOffset;
    [HideInInspector] public float buttonScale;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Init(levels[currentLevel], false);
    }

    void calculateGridValues()
    {
        blockBounds = barricadePrefab.GetComponent<SpriteRenderer>().sprite.rect.size * barricadePrefab.transform.localScale;
        blockBounds = blockBounds / 100;

        LevelData level = levels[currentLevel];
        Vector2 gridSize = level.gridSize;
        Vector2 gridOffset = level.background.transform.localPosition;
        gridOrigin = new Vector2(-(gridSize.x - 1) / 2 * blockBounds.x, -(gridSize.y - 1) / 2 * blockBounds.y) + gridOffset;    // minus because we need the bottom left corner ie (0, 0)

        incrementAmount = blockBounds;
        buttonOffset = level.buttonOffset;
        buttonScale = level.buttonScale;
    }
    void clearScene(bool isSameLevel)
    {
        if(!isSameLevel)
            calculateGridValues(); 

        if (currentBackground)
            Destroy(currentBackground);

        SkullObjectPool.instance.ReturnAllObjects();

        currentProgress = 0;
        winCon = levels[currentLevel].winCondition;
    }
    void Init(LevelData data, bool isSameLevel)
    {
        clearScene(isSameLevel);

        currentBackground = Instantiate(data.background, BGParent);

        for (int i = 0; i < data.barricadeCoordinates.Count; i++)
        { 
            GameObject barricade;
            if (i < barricadeParent.childCount)
            {
                barricade = barricadeParent.GetChild(i).gameObject;
            }
            else 
                barricade = Instantiate(barricadePrefab, barricadeParent);
            barricade.transform.localPosition = gridOrigin + incrementAmount * data.barricadeCoordinates[i];
        }
     
        
        for (int i = 0; i < data.buttonData.Count; i++)
        {
            GameObject button;
            if (i < buttonParent.childCount)
            {
                button = buttonParent.GetChild(i).gameObject;
            }
            else            
                button = Instantiate(buttonPrefab, buttonParent);
            

            if (data.buttonData[i].amount == 1)
                data.buttonData[i].sprite = buttonOne;
            else if (data.buttonData[i].amount == 2)
                data.buttonData[i].sprite = buttonTwo;
            else if (data.buttonData[i].amount == 3)
                data.buttonData[i].sprite = buttonThree;

            button.GetComponent<ButtonScript>().GetInitValues(i, levels[currentLevel], isSameLevel);
        }
    }
    public void resetLevel()
    {
        Init(levels[currentLevel], true);
    }
    public void updateProgress()
    {
        currentProgress++;

        if (currentProgress < winCon)
            return;

        currentLevel++;
        if (currentLevel < levels.Count)
            Init(levels[currentLevel], false);
        else
            EndGame();
    }
    void EndGame()
    {
        EndgamePanel();
    }




    //------test functions-----
    public GameObject eventSystem;
    public GameObject endPanel;
    private void EndgamePanel()
    {
        endPanel.SetActive(true);
    }
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

public enum SpawnDirection
{
    up, 
    right,
    down,
    left
}