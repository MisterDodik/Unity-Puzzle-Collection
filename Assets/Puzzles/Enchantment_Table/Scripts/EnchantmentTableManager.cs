using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnchantmentTableManager : MonoBehaviour
{
    public EnchantmentTableNodePositions data;

    [SerializeField] GameObject hexagonPrefab;
    [SerializeField] Transform hexagonParent;

    [SerializeField] GameObject pathPrefab;
    [SerializeField] Transform pathParent;

    public Sprite hexagonShineSprite;
    public Sprite defaultHexagonSprite;

    // end screen
    [SerializeField] Animator animator;
    int winCondition = 18;
    [HideInInspector] int gameProgress = 0;
    
    [HideInInspector] public bool moveInProgress = false;
    
    
    public static EnchantmentTableManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        Init();
    }
    public void checkWin(int score)
    {
        gameProgress += score;
        if (gameProgress >= winCondition)
        {
            EndgamePanel();
            gameObject.SetActive(false);
        }
    }
    void Init()
    {
        // Spawning hexagons
        for (int i = 0, j = 0; i < data.hexagonPositions.Count; i++)
        {
            float y = data.hexagonPositions[i].yPos;

            foreach (float x in data.hexagonPositions[i].xPos)
            {
                Vector3 spawnPosition = new Vector3(x, y, 0);
                GameObject hexagon = (GameObject)Instantiate(hexagonPrefab, spawnPosition, Quaternion.identity, hexagonParent);

                j++;

            }
        }

        // Spawning paths (angle 0)
        for (int i = 0; i < data.pathPositions1.Count; i++)
        {
            float y = data.pathPositions1[i].yPos;

            foreach (float x in data.pathPositions1[i].xPos)
            {
                Vector3 spawnPosition = new Vector3(x, y, 0);
                Instantiate(pathPrefab, spawnPosition, Quaternion.identity, pathParent);
            }
        }

        // Spawning paths (angle 60)
        for (int i = 0; i < data.pathPositions2.Count; i++)
        {
            float y = data.pathPositions2[i].yPos;

            foreach (float x in data.pathPositions2[i].xPos)
            {
                Vector3 spawnPosition = new Vector3(x, y, 0);
                Instantiate(pathPrefab, spawnPosition, Quaternion.Euler(0, 0, 60), pathParent);
            }
        }

        // Spawning paths (angle -60)
        for (int i = 0; i < data.pathPositions3.Count; i++)
        {
            float y = data.pathPositions3[i].yPos;

            foreach (float x in data.pathPositions3[i].xPos)
            {
                Vector3 spawnPosition = new Vector3(x, y, 0);
                Instantiate(pathPrefab, spawnPosition, Quaternion.Euler(0, 0, -60), pathParent);
            }
        }



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
