using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.puzzles.ArchivePuzzle
{
    public class ArchivePuzzleManager : MonoBehaviour
    {
        private int currentLevel = 0;
        private GameObject currentLevelParent;

        [SerializeField] private GameObject imagePiece;
        private List<GameObject> imagePieces = new List<GameObject>();  //pool
        [SerializeField] private List<GameObject> levelParents = new List<GameObject>();

        private int winCondition;
        private int currentProgress;

        [SerializeField] private List<Sprites> sprites = new List<Sprites>();

        private void Start()
        {
            InitPuzzle(currentLevel);
        }
     
        public void ResetGame()
        {
            InitPuzzle(currentLevel);
        }
        public void Skip()
        {
            currentProgress = 0;
            StartCoroutine(skipCoroutine());
        }
        IEnumerator skipCoroutine()
        {
            for (int i = 0; i < currentLevelParent.transform.childCount - 1; i++)
            {
                ImagePieceScript imagePieceScript = imagePieces[i].GetComponent<ImagePieceScript>();
                imagePieces[i].transform.localPosition = imagePieceScript.winPos;
                imagePieceScript.CloseEnough();
                yield return new WaitForSeconds(0.1f);
                CheckWin();
            }
        }
        public void Completed()
        {
            print("Win");
            EndgamePanel();
        }

        private void LoadNextLevel()
        {
            currentLevel++;
            if(currentLevel == 3)
            {
                Completed();
                return;
            }

            StartCoroutine(loadLevel());
        }
        IEnumerator loadLevel()
        {
            yield return new WaitForSeconds(0.5f);
            InitPuzzle(currentLevel);
        }
        private void InitPuzzle(int level)
        {
            if(currentLevelParent!=null)
                currentLevelParent.SetActive(false);

            currentLevelParent = levelParents[level];
            currentLevelParent.SetActive(true);

            winCondition = currentLevelParent.transform.childCount - 1;
            currentProgress = 0;

            for (int i=0; i < currentLevelParent.transform.childCount - 1; i++)
            {
                Transform glow = currentLevelParent.transform.GetChild(i);

                GameObject piece;
                if (i < imagePieces.Count)
                {
                    piece = imagePieces[i];
                }
                else
                {
                    piece = Instantiate(imagePiece);
                    imagePieces.Add(piece);
                }
                
                piece.transform.parent = glow;
                piece.transform.localPosition = Vector3.zero;
                piece.GetComponent<ImagePieceScript>().Initialize(this, sprites[level]);

            }
        }
        public void CheckWin()
        {
            currentProgress++;
            if (currentProgress >= winCondition)
            {
                LoadNextLevel();
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



    [System.Serializable]
    public class Sprites
    {
        public Sprite defaultSprite;
        public Sprite winSprite;
    }
}


