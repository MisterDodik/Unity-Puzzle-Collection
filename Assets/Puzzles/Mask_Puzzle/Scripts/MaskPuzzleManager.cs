using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.puzzles.MaskPuzzle
{
    public class MaskPuzzleManager : MonoBehaviour
    {
        public MaskPuzzleData gameData;

        private List<Vector2> startPositions = new List<Vector2>();
        public Transform maskParent;

        public List<Transform> placedMasks = new List<Transform>();

        public float moveDuration = 0.5f;


        private int currentProgress = 0;
        private int winCon = 9;

        private void Start()
        {
            InitPuzzle();
        }  

        public void ResetGame()
        {
            InitPuzzle();
        }
        public void Skip()
        {
            placedMasks.Clear();
            for (int i = 0; i < maskParent.childCount; i++)
            {
                Transform child = maskParent.GetChild(i);
                placedMasks.Add(child);
                child.localPosition = child.GetComponent<MaskScript>().correctPosition;
            }

            CheckWin();
        }
        
        public void Completed()
        {
            print("Win");
            EndgamePanel();
        }

        private void InitPuzzle()
        {
            currentProgress = 0;

            if (startPositions.Count == 0)
            {
                for(int i = 0; i<maskParent.childCount; i++)
                {
                    Transform child = maskParent.GetChild(i);
                    startPositions.Add(child.position);
                    child.GetComponent<MaskScript>().InitMask(this, gameData.pathPoints[0], gameData.pathPoints[i+1]);
                }
            }


            if (placedMasks.Count > 0)
            {
                for(int i=0; i < maskParent.childCount; i++)
                {
                    maskParent.GetChild(i).position = startPositions[i];
                    maskParent.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
                }
                placedMasks.Clear();
            }
        }


        public void MoveMasks(Transform newMask, Vector2? customPosition = null)
        {
            newMask.GetComponent<BoxCollider2D>().enabled = false;

            placedMasks.Add(newMask);


            Sequence moveSequence = DOTween.Sequence();

            for (int i = placedMasks.Count - 1, j = 0; i >= 0; i--, j++)
            {
                Transform item = placedMasks[i];
                Vector2 targetPos =  customPosition ?? gameData.pathPoints[j + 1];
                item.DOKill();

                moveSequence.Join(item.DOLocalMove(targetPos, moveDuration).SetEase(Ease.InOutQuad));
            }

            moveSequence.OnComplete(() =>
            {
                if (placedMasks.Count == winCon)
                    CheckWin();
            });
        }
      
        
        
        private void CheckWin()
        {
            foreach(Transform item in placedMasks)
            {
                if (item.GetComponent<MaskScript>().CorrectPosition())
                    currentProgress++;
            }
            if (currentProgress >= winCon)
                Completed();
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
