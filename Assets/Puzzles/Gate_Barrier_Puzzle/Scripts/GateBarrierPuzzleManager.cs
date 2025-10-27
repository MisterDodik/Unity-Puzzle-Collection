using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.puzzles.Gate_Barrier_Puzzle
{
    public class GateBarrierPuzzleManager : MonoBehaviour
    {
        [SerializeField] private GateBarrierData gameData;

        [SerializeField] private Transform mapParent;
        [SerializeField] private Transform deviceParent;

        [HideInInspector] public Vector2 mapSize;           
        [HideInInspector] public Vector2 navigatorBorder;

        private List<GameObject> checkMarks = new List<GameObject>();
        private List<GameObject> gauges = new List<GameObject>();

        private List<GameObject> spawnedNumbers = new List<GameObject>();
        private GameObject currentNumber;

        public int ectoplasmCount = 8;
        private List<Vector2> ectoplasmPositions = new List<Vector2>();

        private int currentEctoplasm = 0;
        private Vector2 currentEctoplasmPosition;

        [SerializeField] private DragHandler detectionDevice;
        [SerializeField] private Transform devicePivot;

        [SerializeField] private GameObject particle;
        [SerializeField] private GameObject rippleDetection;

        private bool gameOver = false;
        private void Start()
        {
            InitGame();
        }
        public void ResetGame()
        {
            InitGame();
        }
        public void Skip()
        {
            foreach (GameObject gauge in gauges)
                gauge.SetActive(true);
            if(currentNumber!=null)
                currentNumber.SetActive(false);
            spawnedNumbers[ectoplasmCount].SetActive(true);

            Vector2 mappedCheckMarkPos;
            for (int i = 0; i < checkMarks.Count; i++)
            {
                currentEctoplasmPosition = ectoplasmPositions[i];

                Vector2 positionRatio = new Vector2(
                  (currentEctoplasmPosition.x + mapSize.x / 2) / mapSize.x,
                  (currentEctoplasmPosition.y + mapSize.y / 2) / mapSize.y
                );
                mappedCheckMarkPos = (positionRatio * 2f - Vector2.one) * navigatorBorder;
                checkMarks[i].SetActive(true);
                checkMarks[i].transform.localPosition = mappedCheckMarkPos;
            }
            OnCompleteEvents();
        }

        public void Completed()
        {
            print("Win");
        }

        IEnumerator delayedEndGame()
        {
            yield return new WaitForSeconds(1);
            EndGamePanel();
            Completed();
        }
        private void OnCompleteEvents()
        {
            if (!gameOver)
            {
                gameOver = true;
                StartCoroutine(delayedEndGame());
            }
        }
        private void InitGame()
        {
            mapSize = gameData.mapSize;
            navigatorBorder = gameData.navigatorBorder;

            for (int i = 0; i < ectoplasmCount; i++)
            {
                Vector2 randomPosition = new Vector2(UnityEngine.Random.Range(-mapSize.x / 2 + 50, mapSize.x / 2 - 50), UnityEngine.Random.Range(-mapSize.y / 2 + 50, mapSize.y / 2 - 50));
                ectoplasmPositions.Add(randomPosition);

                GameObject checkMark = Instantiate(gameData.checkMarkPrefab, mapParent);
                checkMark.SetActive(false);
                checkMarks.Add(checkMark);

                GameObject gauge = Instantiate(gameData.gaugePrefab, deviceParent);
                gauge.transform.localPosition = gameData.gaugeStartPosition + new Vector2(0, i * 0.14f);
                gauge.SetActive(false);
                gauges.Add(gauge);
            }

            for (int i = 0; i < gameData.numbers.Count; i++)
            {
                GameObject number = Instantiate(gameData.numbers[i], deviceParent);
                number.SetActive(false);
                spawnedNumbers.Add(number);

            }
            currentNumber = spawnedNumbers[0];
            currentNumber.SetActive(true);

            particle.SetActive(false);
            rippleDetection.SetActive(false);

            detectionDevice.InitDevice(this);
        }

        public Vector2 CurrentEctoplasm()
        {
            currentEctoplasmPosition = ectoplasmPositions[currentEctoplasm];
            return currentEctoplasmPosition;
        }


        public void OnEctoplasmFound()
        {
            detectionDevice.dragPaused = true;
            checkMarks[currentEctoplasm].SetActive(true);
            
            Vector2 positionRatio = new Vector2(
              (currentEctoplasmPosition.x + mapSize.x / 2) / mapSize.x,
              (currentEctoplasmPosition.y + mapSize.y / 2) / mapSize.y
            );

            Vector2 mappedCheckMarkPos = (positionRatio * 2f - Vector2.one) * navigatorBorder;
            checkMarks[currentEctoplasm].transform.localPosition = mappedCheckMarkPos;

            gauges[currentEctoplasm].SetActive(true);

            if (currentNumber != null)
                currentNumber.SetActive(false);
            currentNumber = spawnedNumbers[currentEctoplasm + 1];
            currentNumber.SetActive(true);


            StartCoroutine(particleAnimation());       
            if (currentEctoplasm >= checkMarks.Count - 1 && !gameOver)
            {
                OnCompleteEvents();
                return;
            }
            detectionDevice.dragPaused = false;
            currentEctoplasm++;
        }

        IEnumerator particleAnimation()
        {
            rippleDetection.transform.localPosition = devicePivot.transform.position;
            particle.transform.localPosition = devicePivot.transform.position;

            rippleDetection.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            rippleDetection.SetActive(false);
            particle.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            particle.SetActive(false);
        }












        //---test functions
        public GameObject endgamePanel;
        public void EndGamePanel()
        {
            endgamePanel.SetActive(true);
        }
        public void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
