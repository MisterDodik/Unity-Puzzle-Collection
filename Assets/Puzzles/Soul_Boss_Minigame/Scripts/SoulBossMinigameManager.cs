using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.puzzles.Soul_Boss_Minigame
{
    public class SoulBossMinigameManager : MonoBehaviour
    {
        [SerializeField] CharacterScript character;

        [SerializeField] private List<GameObject> possibleBalls= new List<GameObject>();
        private List<GameObject> pooledBalls = new List<GameObject>();
        private List<GameObject> characterRays = new List<GameObject>();

        private GameObject ballToFire;
        [SerializeField] private float firedBallSpeed = 10;
        [HideInInspector] public int currentBall = 0;

        [SerializeField] private int initialBallCount;

        [SerializeField] private MinigameData gameData;

        [SerializeField] private Transform PathParent;
        [SerializeField] private Transform singleWaypoint;
        [SerializeField] private Transform BallParent;
        [SerializeField] private Transform PuzzleBg;

        [SerializeField] private BallTrainManager ballTrainManager;
        [SerializeField] private PathManager pathManager;

        [SerializeField] private int totalLevelCount = 5;
        private int currentLevel = 0;
        private void Start()
        {
            InitGame(false);
        }
        public void ResetGame()
        {
            InitGame(true);
        }
        public void Skip()
        {
            currentLevel = totalLevelCount;
            ballTrainManager.Skip();
        }

        public void Completed()
        {
            print("Win");


            EndGamePanel();
            endGameText.text = "You won";
        }

        private void InitGame(bool isReset)
        {

            if (PathParent.childCount == 0)
            {
                foreach (Vector2 waypointPos in gameData.waypointLocations)
                {
                    Transform wpGO = Instantiate(singleWaypoint, PathParent);
                    wpGO.transform.localPosition = waypointPos;
                }
            }

            pathManager.puzzleBg = PuzzleBg;
            pathManager.InitializePath(PathParent, null);

            ballTrainManager.InitializeBallTrainPools(BallParent, initialBallCount, isReset);

            character.manager = this;

            currentBall = 0;
            for (int i=0; i < possibleBalls.Count; i++)
            {
                GameObject ball;
                if (i < pooledBalls.Count)
                    ball = pooledBalls[i];
                else
                {
                    ball = Instantiate(possibleBalls[i], character.gameObject.transform);
                    ball.GetComponent<BallToFireScript>().trainManager = ballTrainManager;
                    pooledBalls.Add(ball);
                }
                ball.transform.localPosition= Vector2.zero;

                ball.SetActive(false);

                if (i >= characterRays.Count)
                {
                    GameObject rayObject = character.transform.GetChild(i).gameObject;
                    characterRays.Add(rayObject);

                    //rayObject.SetActive(false);
                }
                characterRays[i].SetActive(false);
            }
            ballToFire = pooledBalls[currentBall];
            characterRays[currentBall].SetActive(true);

        }


        public void ChangeBall()
        {
            characterRays[currentBall].SetActive(false);

            currentBall = (currentBall+1) % pooledBalls.Count;

            ballToFire = pooledBalls[currentBall];
            ballToFire.transform.localPosition = Vector2.zero;

            characterRays[currentBall].SetActive(true);
        }

        public void FireAndRotate(Vector3 mousePosition)
        {
            //disable mouse events
            eventHandler.SetActive(false);

            ballToFire.GetComponent<BallToFireScript>().hitObjectsCount = 0;

            mousePosition = PuzzleBg.transform.InverseTransformPoint(mousePosition);
            Vector3 direction = (mousePosition - character.transform.localPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            character.transform.localRotation = Quaternion.Euler(0, 0, angle - 20);

            ballToFire.SetActive(true);
            characterRays[currentBall].SetActive(false);
            Vector3 offscreenTarget = ballToFire.transform.position + direction * 10f;

            float distance = Vector3.Distance(ballToFire.transform.position, offscreenTarget);
            float duration = distance / firedBallSpeed;

            ballToFire.transform.DOMove(offscreenTarget, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    ballToFire.SetActive(false);
                    ChangeBall();

                    //enable mouse events
                    eventHandler.SetActive(true);
                });

        }

        public void LoadNextLevel(bool isSkip)
        {
            if (!isSkip)
                initialBallCount += 3;
            currentLevel++;
            if (currentLevel >= totalLevelCount)
                Completed();
            else
                InitGame(true);
        }

        public void GameOver()
        {
            currentLevel--;
            ballTrainManager.Skip();
        }






        //---test functions
        public GameObject endgamePanel;
        public TextMeshProUGUI endGameText;
        public GameObject eventHandler;
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
