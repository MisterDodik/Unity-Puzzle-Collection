using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace com.puzzles.Word_Minigame
{
    public class WordMinigameManager : MonoBehaviour
    {
        private List<SpriteRenderer> highlightedWords = new List<SpriteRenderer>();
        private string currentHighlightedWord;
        private int currentLevelProgress;
        private int currentLevelWinCondition = 0;

        private GameObject currentLevelParent;

        [SerializeField] private List<GameObject> levels = new List<GameObject>();
        [SerializeField] private Transform levelsParent;
        [SerializeField] private List<GameObject> spawnedLevels = new List<GameObject>();
        [SerializeField] private List<GameObject> endScreens = new List<GameObject>();

        private int currentLevel = 0;


        private void Start()
        {
            InitGame(false);
        }

        public void ResetGame()
        {
            endPanel.SetActive(false);
            InitGame(true);
        }
        public void Skip()
        {
            StartCoroutine(DelayedNextLevel());
        }

        public void Completed()
        {
            print("Win");

            endPanel.SetActive(true);
        }

        private void InitGame(bool isReset)
        {
            currentLevel = 0;

            if (!isReset)
            {
                foreach (GameObject level in levels)
                {
                    spawnedLevels.Add(Instantiate(level, levelsParent));
                }
            }

            foreach (GameObject level in spawnedLevels)
            {
                level.SetActive(false);
            }         

            foreach (GameObject item in endScreens)
            {
                item.SetActive(false);
            }

            LoadLevel(currentLevel);
        }

        private void LoadLevel(int levelIndex)
        {
            currentLevelProgress = 0;

            if (highlightedWords.Count > 0)
            {
                foreach (SpriteRenderer item in highlightedWords)
                    item.color = Color.white;

                highlightedWords.Clear();
            }
            currentLevelWinCondition = 0;

            if (levelIndex >= spawnedLevels.Count)
            {
                Completed();
                return;
            }

            currentLevelParent = spawnedLevels[levelIndex];
            currentLevelParent.SetActive(true);
            foreach(WordsScript item in currentLevelParent.GetComponentsInChildren<WordsScript>())
            {
                item.manager = this;
                item.levelIndex = currentLevel;

                if (item.isCorrect)
                    currentLevelWinCondition++;
            }

        }

        IEnumerator DelayedNextLevel()
        {
            //disable mouse events
            yield return new WaitForSeconds(0.3f);

            if (currentLevelParent != null)
                currentLevelParent.SetActive(false);
            endScreens[currentLevel].SetActive(true);

            yield return new WaitForSeconds(2);

            //enable mouse events
            
            endScreens[currentLevel].SetActive(false);

            LoadLevel(++currentLevel);
        }
        public void HighlightWord(SpriteRenderer spriteRenderer, bool isCorrectWord, string word)
        {
            if (highlightedWords.Contains(spriteRenderer))
                return;

            if (word != currentHighlightedWord)
            {
                currentHighlightedWord = word;

                if (highlightedWords.Count > 0)
                {
                    foreach (SpriteRenderer item in highlightedWords)
                        item.color = Color.white;

                    highlightedWords.Clear();
                }
            }
            if (isCorrectWord)
            {
                spriteRenderer.color = Color.yellow;
                currentLevelProgress++;

                if (currentLevelProgress >= currentLevelWinCondition)
                {
                    StartCoroutine(DelayedNextLevel());
                }
            }
            else
            {
                spriteRenderer.color = new Color(0, 0.9f, 1, 1);
                currentLevelProgress = 0;
            }
            highlightedWords.Add(spriteRenderer);           
        }





        //------test functions-----
        public GameObject endPanel;
        public void LoadNextScene()
        {
            StartCoroutine(loadScene());
        }
        IEnumerator loadScene()
        {
            yield return new WaitForSeconds(.1f);
            SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % (SceneManager.sceneCount + 1));
        }
        public void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

}
