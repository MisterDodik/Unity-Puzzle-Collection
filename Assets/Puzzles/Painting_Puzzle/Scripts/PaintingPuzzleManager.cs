using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.puzzles.Painting_Puzzle
{
    public class PaintingPuzzleManager : MonoBehaviour
    {
        [SerializeField] private Transform puzzleParent;
        [SerializeField] private PaintingPuzzleData puzzleData;
        [SerializeField] private Transform piecesPrefab;
        [SerializeField] private List<GameObject> glowObjects = new List<GameObject>();

        [SerializeField] private Color defaultColor;
        [SerializeField] private Color selectedColor;
        public float swapDuration = .5f;

        private Transform spawnedPiecesParent;
        private List<PieceScript> spawnedPieces = new List<PieceScript>();
        private Dictionary<int, Vector2> indexPositionDict = new Dictionary<int, Vector2>();

        private int[] progress = { 0, 0, 0, 0 };
        private Transform selectedPiece;
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
            // Disable click events
            eventHandler.SetActive(false);

            Sequence sequence = DOTween.Sequence();

            foreach(PieceScript piece in spawnedPieces)
            {
                Vector2 finalPosition;
                indexPositionDict.TryGetValue(piece.targetIndex, out finalPosition);
                sequence.Join(piece.transform.DOLocalMove(finalPosition, swapDuration).OnComplete(() =>
                {
                    piece.OnSwapEvents(piece.targetIndex);
                }));
            }

            sequence.Play().OnComplete(() =>
            {
                // Enable click events
                eventHandler.SetActive(true);
            });
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
            StartCoroutine(delayedEndGame());
        }
        private void InitGame()
        {
            if (spawnedPiecesParent == null)
            {
                spawnedPiecesParent = Instantiate(piecesPrefab, puzzleParent);

                for (int i = 0; i < puzzleData.piecePositions.Count; i++)
                {
                    indexPositionDict[i] = puzzleData.piecePositions[i];
                }
            }
            for (int i = 0; i < glowObjects.Count; i++)
            {
                glowObjects[i].SetActive(false);
                progress[i] = 0;
            }
            

            for (int i=0; i<puzzleData.piecesData.Count; i++)
            {
                PaintingPuzzleData.PieceData data = puzzleData.piecesData[i];

                PieceScript piece;

                if (i < spawnedPieces.Count)
                    piece = spawnedPieces[i];
                else
                {
                    piece = spawnedPiecesParent.transform.GetChild(i).GetComponent<PieceScript>();

                    spawnedPieces.Add(piece);
                }
                piece.InitPiece(this, data.startingIndex, data.targetIndex);

                Vector2 piecePosition;
                indexPositionDict.TryGetValue(data.startingIndex, out piecePosition);
                piece.transform.localPosition = piecePosition;
            }
        }




        public void SwapSelectPieces(Transform piece)
        {
            if (piece == selectedPiece)
            {
                selectedPiece.GetComponent<SpriteRenderer>().color = defaultColor;
                selectedPiece = null;
                return;
            }
            if (selectedPiece == null)
            {
                selectedPiece = piece;
                selectedPiece.GetComponent<SpriteRenderer>().color = selectedColor;
                return;
            }

            selectedPiece.GetComponent<SpriteRenderer>().color = defaultColor;
            SwapTiles(piece, selectedPiece);
        }

        private void SwapTiles(Transform piece1, Transform piece2, Vector2? customPosition = null)
        {
            // Disable click events
            eventHandler.SetActive(false);

            Vector2 pos1 = piece1.localPosition;
            Vector2 pos2 = customPosition ?? piece2.localPosition;

            piece1.DOKill();
            piece2?.DOKill();

            piece1.DOLocalMove(pos2, swapDuration).SetEase(Ease.InOutQuad);
            piece2?.DOLocalMove(pos1, swapDuration).SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    selectedPiece = null;

                    int index1 = piece1.GetComponent<PieceScript>().currentIndex;
                    int index2 = piece2.GetComponent<PieceScript>().currentIndex;

                    piece1.GetComponent<PieceScript>().OnSwapEvents(index2);
                    piece2.GetComponent<PieceScript>().OnSwapEvents(index1);

                    // Enable click events
                    eventHandler.SetActive(true);
                });
        }

        public void CheckWin(int index, bool wasCorrect)
        {
            if (!wasCorrect) 
                progress[index]++;
            else
            {
                progress[index] = Mathf.Max(0, progress[index] - 1); ;
                return;
            }

            if (progress[index] == 4)
            {
                FixBatchOfTiles(index);
                glowObjects[index].SetActive(true);
            }

            foreach (int item in progress)
            {
                if (item != 4)
                    return;
            }

            OnCompleteEvents();
            return;
        }


        void FixBatchOfTiles(int batchIndex, int tileCount = 16, int batchSize = 4)
        {
            int totalBatchCount = tileCount / batchSize;

            int shiftedBatchIndex = (batchIndex + 1) % totalBatchCount;

            int start = (shiftedBatchIndex * batchSize - 2 + tileCount) % tileCount;

            for (int i = 0; i < batchSize; i++)
            {
                int index = (start + i) % tileCount;
                foreach (var tile in spawnedPieces)
                {
                    if (tile.targetIndex == index)
                    {
                        tile.isCorrect = true;
                    }
                }
            }

            
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

