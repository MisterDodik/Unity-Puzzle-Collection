using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace com.puzzles.LockerPuzzle
{
    public class LockerPuzzleManager : MonoBehaviour
    {
        [SerializeField] LockerPuzzleData levelsData;
        public int currentLevel = 0; 

        [SerializeField] Transform generalParent;

        [SerializeField] GameObject defaultTrianglePrefab;
        [SerializeField] GameObject startTrianglePrefab;
        [SerializeField] GameObject endTrianglePrefab;
        [SerializeField] Transform triangleParent;
        
        GameObject ballTriangle;
        GameObject startTriangle;
        GameObject endTriangle;
        List<GameObject> defaultTriangles = new List<GameObject>();

        [SerializeField] GameObject ballPrefab;
        Transform ball;

        [HideInInspector]public float moveDuration = 0.5f;

        private void Start()
        {
            InitLevel(levelsData.levels[currentLevel]);
        }

        public void ResetGame()
        {
            InitLevel(levelsData.levels[currentLevel]);            
        }
        public void Skip()
        {
            StartCoroutine(skipLevel(levelsData.solutions[currentLevel]));
        }

        public void Completed()
        {
            print("Win");
            EndgamePanel();
        }

        private IEnumerator skipLevel(LockerPuzzleData.Solution solution)
        {
            //Disable mouse events
            eventSystem.SetActive(false);

            ResetGame();

            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < solution.path.Length; i++)
            {
                Vector2 origin = new Vector2(2.23f / 2 * solution.path[i].x, 2 - 2 * solution.path[i].y) * levelsData.levels[currentLevel].gridScale;
                origin += levelsData.levels[currentLevel].gridPos;
                LockerPuzzleTriangleScript triangle;
                Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, 0.1f);

                foreach (Collider2D item in colliders)
                {
                    if (item.TryGetComponent<LockerPuzzleTriangleScript>(out triangle))
                    {
                        triangle.OnClickEvent();
                        break;
                    }
                }

                 yield return new WaitForSeconds(moveDuration*3f);
            }

            //Enable mouse events
            eventSystem.SetActive(true);
        }




        public void LoadNextLevel()
        {
            currentLevel++;
            if (currentLevel == levelsData.levels.Count)
                Completed();

            InitLevel(levelsData.levels[currentLevel]);

        }

        private void deactivateTriangles()
        {
            foreach (GameObject triangle in defaultTriangles)
                triangle.SetActive(false);
        }
        private void InitLevel(LockerPuzzleData.Level data)
        {
            deactivateTriangles();

            triangleParent.localPosition = data.gridPos;
            triangleParent.localScale = data.gridScale;

            float currentRow = 0;
            bool isDifferentRow=false;

            if(startTriangle==null)
                startTriangle = Instantiate(startTrianglePrefab, triangleParent);
            if(endTriangle==null)
                endTriangle = Instantiate(endTrianglePrefab, triangleParent);
            if (ball == null)
                ball = Instantiate(ballPrefab, generalParent).transform;
            for (int i = 0, k = 0, l=0; i < data.level.Count; i++, k++)
            {

                if (currentRow != data.level[i].position.y)
                {
                    currentRow = data.level[i].position.y;
                    isDifferentRow = !isDifferentRow;
                    k = 0;
                }

                if (data.level[i].IsEmpty)
                    continue;
                GameObject triangle;

                if (data.level[i].isEnd)
                    triangle = endTriangle;
                else if (data.level[i].isStart)
                    triangle = startTriangle;
                else
                {
                    if (l < defaultTriangles.Count)
                    {
                        triangle = defaultTriangles[l];
                        triangle.SetActive(true);
                    }
                    else
                    {
                        triangle = Instantiate(defaultTrianglePrefab, triangleParent);
                        defaultTriangles.Add(triangle);
                    }
                        l++;
                }
               
                if (isDifferentRow)
                    triangle.transform.localEulerAngles = new Vector3(0, 0, (k % 2 == 0 ? 0 : 180));
                else
                    triangle.transform.localEulerAngles = new Vector3(0, 0, (k % 2 == 0 ? 180 : 0));


                Vector2 trianglePos = new Vector2(2.23f / 2 * data.level[i].position.x, 2 - 2 * data.level[i].position.y);
                if (triangle.transform.localEulerAngles.z == 0)
                    trianglePos.y -= 0.65f;
                triangle.transform.localPosition = trianglePos;

                if (data.level[i].isStart)
                {                  
                    ballTriangle = triangle;
                    ball.localPosition = triangle.transform.position;
                }
                
                triangle.GetComponent<LockerPuzzleTriangleScript>().GetInitData(data.level[i], this);
            }
            StartCoroutine(LoadNeightbors());
        }

        IEnumerator LoadNeightbors()
        {
            yield return null;
            for (int i = 0; i < triangleParent.childCount; i++)
            {
                if (triangleParent.GetChild(i) != ball)
                    triangleParent.GetChild(i).GetComponent<LockerPuzzleTriangleScript>().FindNeighbors();
            }
        }

        public void MoveBall(Transform desiredTransform)
        {
            //Vector2 endPos = desiredTransform.position;
            Vector2 endPos = transform.InverseTransformPoint(desiredTransform.position);
            ball.DOKill();

            LockerPuzzleTriangleScript lockerPuzzleTriangleScript = ballTriangle.GetComponent<LockerPuzzleTriangleScript>();
            lockerPuzzleTriangleScript.hasBall = false;
            lockerPuzzleTriangleScript.resetNeighbors();

            ball.DOLocalMove(endPos, moveDuration)
                .SetEase(Ease.Linear) 
                .OnComplete(() =>
                {
                    //lockerPuzzleTriangleScript.activateNeighbors();
                    ballTriangle = desiredTransform.gameObject;
                    ballTriangle.GetComponent<LockerPuzzleTriangleScript>().RotateTriangle();
                });
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
}
