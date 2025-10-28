using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoneTableManager : MonoBehaviour
{
    public DiscData data;
    [SerializeField] GameObject centerCirclePrefab;
    [SerializeField] GameObject circlePrefab;
    [SerializeField] GameObject buttonPrefab;

    [SerializeField] Transform circleParent;
    [SerializeField] Transform buttonParent;

    float angleStep;
    int circleCount;

    List<CheckWinRefactor> circles = new List<CheckWinRefactor>();
    bool toggleCollision = true;
    bool toggleButton = true;

    [HideInInspector] public bool moveInProgress = false;

    GameObject currentButton;
    [SerializeField] GameObject pressedTriangle;
    float duration = 1.5f;

    int winCondition = 8;
    int winProgress = 0;
    [SerializeField] private Animator animator;

    public static StoneTableManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        InitPuzzle();
        circleCount = data.discCount + 1; //plus 1 central
    }
    void InitPuzzle()
    {
        angleStep = 360 / data.discCount;

        // Spawning Buttons
        for (int i = 0; i < data.discCount; i++)
        {
            GameObject spawned = Instantiate(buttonPrefab, buttonParent);

            float angle = -135 + i * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            Vector2 spawnPosition = new Vector2(data.buttonDist * Mathf.Cos(radians), data.buttonDist * Mathf.Sin(radians));

            spawned.transform.localPosition = spawnPosition;
            spawned.transform.localEulerAngles = new Vector3(0, 0, angle);
        }


        // Spawning Circles
        for (int i = 0; i < data.discCount; i++)
        {
            GameObject spawned = Instantiate(circlePrefab, circleParent);

            float angle = i * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            Vector2 spawnPosition = new Vector2(data.circleDist * Mathf.Cos(radians + data.circleAngularOffset), data.circleDist * Mathf.Sin(radians + data.circleAngularOffset));

            spawned.transform.localPosition = spawnPosition;

            spawned.transform.GetChild(0).localEulerAngles = new Vector3(0, 0, data.circleAngles[i]);

            // Same concept as before, just automated this time
            ModifyCheckWin(i, spawned);
        }

        GameObject center = Instantiate(centerCirclePrefab, new Vector3(0, 0, 0), Quaternion.identity, circleParent);
        ModifyCheckWin(data.discCount, center);


    }

    void ModifyCheckWin(int index, GameObject spawned)
    {
        CheckWinRefactor check = spawned.GetComponent<CheckWinRefactor>();
        check.winPosition = data.circleWinPos[index];
        circles.Add(check);
    }


    struct Circles
    {
        public Transform circle1;
        public Transform circle2;
        public Transform centerCircle;
    }

    public void Rotate(int buttonIndex)
    {
        currentButton = buttonParent.GetChild(buttonIndex).gameObject;
        toggleButtonActivity();

        Circles circles = GetCircles(buttonIndex);
        StartCoroutine(RotateOverTime(circles, buttonIndex));
    }

    Circles GetCircles(int index)
    {
        return new Circles
        {
            circle1 = circleParent.GetChild(index % (circleCount - 1)),
            circle2 = circleParent.GetChild((index + 1) % (circleCount - 1)),
            centerCircle = circleParent.GetChild(circleCount - 1)
        };
    }

    IEnumerator RotateOverTime(Circles circles, int index)
    {
        moveInProgress = true;

        toggleCollisionChecks(circles);

        pressedTriangle.SetActive(true);
        pressedTriangle.transform.localRotation = Quaternion.Euler(0, 0, (index - 3) * 45);

        Transform circle1 = circles.circle1;
        Transform circle2 = circles.circle2;
        Transform centerCircle = circles.centerCircle;

        float elapsedTime = 0f;
        Vector3 circle1Pos = circle1.localPosition;
        Vector3 circle2Pos = circle2.localPosition;
        Vector3 centerPos = centerCircle.localPosition;

        Quaternion currentButtonRotation = currentButton.transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, currentButtonRotation.eulerAngles.z + 123);

        //1 -> 2, 2->center, center -> 1
        while (elapsedTime < duration)
        {
            circle1.transform.localPosition = Vector3.Lerp(circle1Pos, circle2Pos, elapsedTime / duration);
            circle2.transform.localPosition = Vector3.Lerp(circle2Pos, centerPos, elapsedTime / duration);
            centerCircle.transform.localPosition = Vector3.Lerp(centerPos, circle1Pos, elapsedTime / duration);

            currentButton.transform.localRotation = Quaternion.Lerp(currentButtonRotation, endRotation, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        circle1.localPosition = circle2Pos;
        circle2.localPosition = centerPos;
        centerCircle.localPosition = circle1Pos;

        circle1.SetSiblingIndex((index + 1) % (circleCount - 1));
        circle2.SetSiblingIndex(circleCount - 1);
        centerCircle.SetSiblingIndex(index % (circleCount - 1));

        pressedTriangle.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        toggleCollisionChecks(circles);
        toggleButtonActivity();
        Win();

        moveInProgress = false;
    }

    void toggleCollisionChecks(Circles circles)
    {
        toggleCollision = !toggleCollision;
        foreach(BoxCollider2D collider in circles.circle1.GetComponentsInChildren<BoxCollider2D>())
        {
            collider.enabled = toggleCollision;
        }
        foreach (BoxCollider2D collider in circles.circle2.GetComponentsInChildren<BoxCollider2D>())
        {
            collider.enabled = toggleCollision;
        }
        foreach (BoxCollider2D collider in circles.centerCircle.GetComponentsInChildren<BoxCollider2D>())
        {
            collider.enabled = toggleCollision;
        }

    }
    // Once a button is pressed, all the other get disabled for a brief time, and the pressed button changes its sprite
    void toggleButtonActivity()
    {
        toggleButton = !toggleButton;
        if (toggleButton)
            currentButton.GetComponent<SpriteRenderer>().sprite = data.enableSprite;
        else
            currentButton.GetComponent<SpriteRenderer>().sprite = data.disableSprite;
    }

    // Called every time the button is pressed to check if the win condition is met
    void Win()
    {
        winProgress = 0;

        foreach (CheckWinRefactor circle in circles)
        {
            winProgress += circle.GetPosition();
        }
        if (winProgress >= winCondition)
        {
            toggleButtonActivity();
            EndgamePanel();
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
