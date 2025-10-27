using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static com.puzzles.Box_Puzzle.BoxPuzzleData;

namespace com.puzzles.Box_Puzzle
{
    public class BoxPuzzleManager : MonoBehaviour
    {
        [SerializeField] private BoxPuzzleData boxPuzzleData;

        [SerializeField] private List<PlankScript> planks = new List<PlankScript>();

        private int winProgress = 0;
        private int winCondition = 9;
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
            winProgress = 9;
            for (int i = 0; i < planks.Count; i++)
            {
                PlankData plankData = boxPuzzleData.plankData[i];

                planks[i].SkipPlank();
            }
            Completed();
        }

        public void Completed()
        {
            print("Win");


            EndGamePanel();
            endGameText.text = "You won";
        }

        private void InitGame()
        {
            winProgress = 0;
            for (int i = 0; i < planks.Count; i++)
            {
                PlankData plankData = boxPuzzleData.plankData[i];
              
                planks[i].Init(this, plankData.startPos, plankData.pivotLocation, plankData.correctState);
            }
        }

        public void CheckWin(bool isCorrect, bool wasCorrect)
        {
            if (winProgress < 0)
                winProgress = 0;   
            if (isCorrect)
            {
                if (wasCorrect)
                    return;
                winProgress++;
            }
            else
            {
                if (!wasCorrect)
                    return;
                winProgress--;
            }
            if (winProgress >= winCondition)
            {
                Completed();
            }
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
