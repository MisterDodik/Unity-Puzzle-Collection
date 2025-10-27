using DG.Tweening;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.puzzles.Chess_MiniGame
{
    public class ChessMinigameManager : MonoBehaviour
    {
        [SerializeField] private ChessMinigameData gameData;
        public ChessBoardPlacementHandler boardPlacementHandler;
        public Transform puzzleParent;

        [SerializeField] private Transform pieceParent;
        [SerializeField] private BasePiece dummyEnemy;
        
        [SerializeField] private Transform boxGlowsPrefab;
        private Transform boxGlowsParent;
        private List<GameObject> boxGlows = new List<GameObject>();

        [SerializeField] private Transform cellUpPrefab;
        private Transform cellUpParent;
        private List<GameObject> cellUps = new List<GameObject>();

        private List<ChessPlayerPlacementHandler> playerPiecesPool = new List<ChessPlayerPlacementHandler>();

        private int progress = 0;
        private int winCondition = 32;
        private void Start()
        {
            InitGame(false);
        }

        public void ResetGame()
        {
            progress = 0;
            InitGame(true);
        }
        public void Skip()
        {
            Sequence sequence = DOTween.Sequence();

            foreach(ChessPlayerPlacementHandler piece in playerPiecesPool)
            {
                sequence.Join(piece.pieceType.Skip());
            }

            sequence.Play();
        }

        public void Completed()
        {
            print("win");
        }

        private void OnCompletedEvents()
        {       
            EndGamePanel();
            Completed();
        }
        private void InitGame(bool isReset)
        {
            if (cellUpParent == null)
                cellUpParent = Instantiate(cellUpPrefab, puzzleParent);
            if (boxGlowsParent == null)
                boxGlowsParent = Instantiate(boxGlowsPrefab, puzzleParent);

            for (int i = 0; i<boxGlowsParent.childCount; i++)
            {
                if (i>=boxGlows.Count)
                    boxGlows.Add(boxGlowsParent.GetChild(i).gameObject);
            }
            for (int i = 0; i < cellUpParent.childCount; i++)
            {
                if (i >= cellUps.Count)
                    cellUps.Add(cellUpParent.GetChild(i).gameObject);
            }

            boardPlacementHandler.boxGlows = boxGlows;
            boardPlacementHandler.cellUps = cellUps;
            boardPlacementHandler.manager = this;

            boardPlacementHandler.InitBoard(isReset);
            SpawnPieces(isReset);
        }

        private void SpawnPieces(bool isReset)
        {
            boardPlacementHandler.PlaceEnemyPieces(dummyEnemy, gameData.enemyPieces);
            if (isReset)
            {
                foreach(ChessPlayerPlacementHandler piece in playerPiecesPool)
                {
                    piece.PlacePiece();
                }
                return;
            }
            for (int i = 0; i < gameData.playerPieces.Count; i++)
            {
                ChessMinigameData.PlayerPieces playerPieces = gameData.playerPieces[i];

                for(int j=0; j<playerPieces.piecePositions.Count; j++)
                {
                    ChessPlayerPlacementHandler piece = Instantiate(playerPieces.piece, pieceParent).GetComponent<ChessPlayerPlacementHandler>();
                    piece.InitPiece(playerPieces.piecePositions[j], playerPieces.finalPositions[j], boardPlacementHandler);

                    playerPiecesPool.Add(piece);
                }
            }

        }

        public void OnCorrectPlace()
        {
            progress++;
            if (progress >= winCondition)
                OnCompletedEvents();
        }



        //---test functions
        public GameObject endgamePanel;
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
