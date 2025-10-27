using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace com.puzzles.Dominoes_Puzzle
{
    public class DominoesPuzzleManager : MonoBehaviour
    {
        public Transform puzzleBg;
        public GameObject glow;
        [SerializeField] private DominoesPuzzleData puzzleData;
        [SerializeField] private Transform triangleParentPrefab;

        private Transform triangleParent;

        private List<TrianglePieceScript> triangles = new List<TrianglePieceScript>();

        private int progress = 0;
        private int winCondition = 11;

        private float skipTime = .5f;
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
            //Disable mouse events
            eventSystem.SetActive(false);

            glow.SetActive(false);
            Sequence sequence = DOTween.Sequence();

            foreach (TrianglePieceScript triangle in triangles)
            {
                if (triangle.endPos == Vector3.zero)
                    continue;
                triangle.spriteRenderer.sortingOrder += 10;
                triangle.ActivateTriangle(false);
                sequence.Join(triangle.transform.DOLocalMove(triangle.endPos, skipTime));             
            }

            sequence.Play().OnComplete(() =>
            {
                foreach (TrianglePieceScript triangle in triangles)
                    triangle.OnPointerUpEvents();

                // Reenable mouse events
                eventSystem.SetActive(true);
            });
        }

        public void Completed()
        {
            print("Win");
        }

        private void OnCompleteEvents()
        {
            EndgamePanel();
            Completed();
        }
        private void InitGame()
        {
            glow.SetActive(false);
            progress = 0;
            if (triangleParent == null)
                triangleParent = Instantiate(triangleParentPrefab, puzzleBg);

            for (int i = 0; i<triangleParent.childCount; i++)
            {
                TrianglePieceScript triangle;

                if (i < triangles.Count)
                {
                    triangle = triangles[i];
                }
                else
                {
                    triangle = triangleParent.GetChild(i).GetComponent<TrianglePieceScript>();
                    triangles.Add(triangle);
                }

                if (i >= puzzleData.triangleData.Count)
                {
                    triangle.LoadTriangle(this, Vector3.zero, null);
                    continue;
                }
                DominoesPuzzleData.TriangleData data = puzzleData.triangleData[i];
                List<TrianglePieceScript> trianglesToActivate = new();
                for (int j = 0; j < data.triangleIndexesToActivate.Count; j++)
                {
                    trianglesToActivate.Add(triangleParent.GetChild(data.triangleIndexesToActivate[j]).GetComponent<TrianglePieceScript>());
                }
                triangle.LoadTriangle(this, data.endPos, trianglesToActivate);
                if (data.isActive)
                    triangle.ActivateTriangle(true);
            }

            winCondition = 11;
        }

        public void CheckWin()
        {
            progress++;
            if (progress >= winCondition)
                OnCompleteEvents();
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
