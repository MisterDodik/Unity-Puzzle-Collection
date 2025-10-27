using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static com.puzzles.ProjectorSlidePuzzle.ProjectorSlideData;

namespace com.puzzles.ProjectorSlidePuzzle
{
    public class ProjectorSlidePuzzleManager : MonoBehaviour
    {
        public ProjectorSlideData GameData;
        public GameObject piecePrefab;

        public float moveDuration = 1f;

        public Transform pieceParent;
        private List<PieceScript> spawnedPieceScripts = new List<PieceScript>();

        public int totalPieces = 8;
        private bool isMiddleShapeEjected = false;
        public GameObject centralPieceSquare;
        private Vector3 centralPiecePosition;
        private const int CENTRAL_PIECE_INDEX = 100;

        int[] defaultSortingOrders = new int[5];

        [SerializeField] private ButtonsScript centralButton;
        [SerializeField] private ButtonsScript rotateLeftButton;
        [SerializeField] private ButtonsScript rotateRightButton;

        private void Start()
        {
            InitGame();
        }

        public void ResetGame()
        {
            endPanel.SetActive(false);
            InitGame();
        }
        public void Skip()
        {
            centralPieceSquare.SetActive(false);
            
            for (int i = 0; i < GameData.symbols.Count; i++)
            {
                SymbolData data = GameData.symbols[i];
                PieceScript piece = spawnedPieceScripts[i];
                piece.gameObject.SetActive(true);
                piece.transform.localEulerAngles = new Vector3(0, 0, data.correctPositionIndex * 45);
            }
            Completed();
        }
        public void Completed()
        {
            print("Win");
            endPanel.SetActive(true);
        }


        private void InitGame()
        {
            centralPieceSquare.SetActive(true);
            UpdateCentralSquareSymbol(0);
            centralPiecePosition = centralPieceSquare.transform.localPosition;
            isMiddleShapeEjected = false;


            for (int i=0; i<GameData.symbols.Count; i++)
            {
                SymbolData data = GameData.symbols[i];

                GameObject piece;
                if (i<spawnedPieceScripts.Count)
                {
                    piece = spawnedPieceScripts[i].gameObject;
                }
                else
                {
                    piece = Instantiate(piecePrefab, pieceParent);
                    piece.GetComponent<PieceScript>().InitPiece(data.symbolIndex, this, data.WinCombination);

                    GameObject symbol = Instantiate(data.symbolGameObject, piece.transform);

                    spawnedPieceScripts.Add(piece.GetComponent<PieceScript>());
                }
                piece.SetActive(true);
                piece.transform.localEulerAngles = new Vector3(0, 0, data.pieceIndex * 45);
            }

            for (int i = 0; i < spawnedPieceScripts.Count; i++)
            {        
                PieceScript pieceScript = spawnedPieceScripts[i];
                if (i == spawnedPieceScripts.Count - 1) 
                {
                    pieceScript.leftNeightbor = null;
                    pieceScript.rightNeightbor = null;
                    pieceScript.gameObject.SetActive(false);      
                    pieceScript.currentIndex = CENTRAL_PIECE_INDEX;     
                    continue;
                }

                pieceScript.currentIndex = i;


                if (i == 0)
                {
                    pieceScript.rightNeightbor = null;
                    pieceScript.leftNeightbor = spawnedPieceScripts[i + 1];
                }
                else if (i == spawnedPieceScripts.Count - 2)
                {
                    pieceScript.leftNeightbor = null;
                    pieceScript.rightNeightbor = spawnedPieceScripts[i - 1];
                }
                else
                {
                    pieceScript.rightNeightbor = spawnedPieceScripts[i - 1];
                    pieceScript.leftNeightbor = spawnedPieceScripts[i + 1];
                }

            }


            for (int i = 0; i < spawnedPieceScripts[0].transform.childCount; i++)
            {
                Transform spriteRenderer = spawnedPieceScripts[0].transform.GetChild(i);
                defaultSortingOrders[i] = spriteRenderer.GetComponent<SpriteRenderer>().sortingOrder;
            }
        }


        public PieceScript FindPiece(int pieceIndex)
        {
            foreach(PieceScript item in spawnedPieceScripts)
            {
                if (item.currentIndex == pieceIndex)
                    return item;
            }
            return null;
        }

        public void CheckWin()
        {
            bool isOver = true;
            foreach(PieceScript item in spawnedPieceScripts)
            {
                if(item.leftNeightbor == null || item.rightNeightbor == null || item.currentIndex == CENTRAL_PIECE_INDEX)
                {
                    isOver = false;
                    break;
                }
                if (item.leftNeightbor.symbolIndex != item.WinCombination.leftNeighborSymbolIndex ||
                    item.rightNeightbor.symbolIndex != item.WinCombination.rightNeighborSymbolIndex)
                {
                    isOver = false;
                    break;
                }
            }
            
            if(isOver)
            {
                Completed();
            }
        }



        
        public static Action RotateTableLeftAction;
        public static Action RotateTableRightAction;
        public static Action CentralButtonAction;

        private void OnEnable()
        {
            RotateTableLeftAction += RotateTableLeft;
            RotateTableRightAction += RotateTableRight;
            CentralButtonAction += CentralButtonMechanics;
        }
        private void OnDisable()
        {
            RotateTableLeftAction -= RotateTableLeft;
            RotateTableRightAction -= RotateTableRight;
            CentralButtonAction -= CentralButtonMechanics;
        }

        public void CentralButtonMechanics()
        {
            centralButton.ChangeSprite(false);
            if (FindPiece(totalPieces - 1) == null)
            {
                if (!isMiddleShapeEjected)
                {
                    SquareToPiece();
                    isMiddleShapeEjected = true;
                }
            }
            else
            {
                if (!isMiddleShapeEjected)
                {
                    SwapPieceAndSquare();
                }
                else
                { 
                    PieceToSquare();
                    isMiddleShapeEjected = false;
                }
            }
        }
        void UpdateCentralSquareSymbol(int index)
        {
            for(int i = 0; i<centralPieceSquare.transform.childCount;  i++)
            {
                Transform child = centralPieceSquare.transform.GetChild(i);
                if (i == index)
                    child.gameObject.SetActive(true);
                else
                    child.gameObject.SetActive(false);
            }
        }
        private void SwapPieceAndSquare()
        {
            //disable mouse events
            DisableMouseEvents();

            PieceScript piece = FindPiece(totalPieces - 1);
            PieceScript centralPiece = FindPiece(CENTRAL_PIECE_INDEX);

            Sequence sequence = DOTween.Sequence();

            sequence.Join(piece.transform.DOLocalMove(centralPiecePosition + new Vector3(0, -1.5f, 0), moveDuration));
            sequence.Join(centralPieceSquare.transform.DOLocalMove(centralPiecePosition + new Vector3(0, 1.5f, 0), moveDuration));


            for (int i = 0; i < piece.transform.childCount; i++)
            {
                Transform spriteRenderer = piece.transform.GetChild(i);
                spriteRenderer.GetComponent<SpriteRenderer>().sortingOrder += 5;
            }

            sequence.OnComplete(() =>
            {
                centralPiece.transform.localEulerAngles = Vector3.zero;
                centralPiece.gameObject.SetActive(true);
                centralPiece.currentIndex = totalPieces - 1;

                centralPiece.leftNeightbor = piece.leftNeightbor;
                centralPiece.rightNeightbor = piece.rightNeightbor;

                piece.gameObject.SetActive(false);
                piece.transform.localPosition = Vector3.zero;
                piece.currentIndex = CENTRAL_PIECE_INDEX;

                if (piece.leftNeightbor != null)
                    piece.leftNeightbor.rightNeightbor = centralPiece;
                if (piece.rightNeightbor != null)
                    piece.rightNeightbor.leftNeightbor = centralPiece;

                piece.leftNeightbor = null;
                piece.rightNeightbor = null;


                centralPieceSquare.transform.localPosition = centralPiecePosition;
                centralPieceSquare.SetActive(true); 
                UpdateCentralSquareSymbol(piece.symbolIndex);

                for (int i = 0; i < piece.transform.childCount; i++)
                {
                    piece.transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = defaultSortingOrders[i];
                }

                centralButton.ChangeSprite(true);
                CheckWin();

                //enable mouse events
                EnableMouseEvents();
            });
        }
        private void PieceToSquare()
        {
            //disable mouse events
            DisableMouseEvents();

            PieceScript piece = FindPiece(totalPieces-1);
            if (piece == null)
                return;

            for (int i = 0; i < piece.transform.childCount; i++)
            {
                Transform spriteRenderer = piece.transform.GetChild(i);
                spriteRenderer.GetComponent<SpriteRenderer>().sortingOrder += 5;
            }

            piece.transform.DOLocalMove(centralPiecePosition + new Vector3(0, -1.5f, 0), moveDuration).OnComplete(() =>
            {
                piece.gameObject.SetActive(false);
                piece.transform.localPosition = Vector3.zero;
                centralPieceSquare.SetActive(true);

                piece.currentIndex = CENTRAL_PIECE_INDEX;

                UpdateCentralSquareSymbol(piece.symbolIndex);

                PieceScript leftPiece = FindPiece(0);
                PieceScript rightPiece = FindPiece(6);

                leftPiece.rightNeightbor = null;
                rightPiece.leftNeightbor = null;

                piece.leftNeightbor = null;
                piece.rightNeightbor = null;


                for (int i = 0; i < piece.transform.childCount; i++)
                {
                    piece.transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = defaultSortingOrders[i];
                }

                centralButton.ChangeSprite(true);

                //enable mouse events
                EnableMouseEvents();
            });
        }
        private void SquareToPiece()
        {
            //disable mouse events
            DisableMouseEvents();

            PieceScript piece = FindPiece(CENTRAL_PIECE_INDEX);
            if (piece == null) 
                return;
            centralPieceSquare.transform.DOLocalMove(centralPiecePosition + new Vector3(0, 1.5f, 0), moveDuration).OnComplete(()=>
            {
                centralPieceSquare.SetActive(false);
                centralPieceSquare.transform.localPosition = centralPiecePosition;

                piece.transform.localEulerAngles = Vector3.zero;
                piece.gameObject.SetActive(true);
                piece.currentIndex = totalPieces - 1;


                PieceScript leftPiece = FindPiece(0);
                PieceScript rightPiece = FindPiece(6);

                leftPiece.rightNeightbor = piece;
                rightPiece.leftNeightbor = piece;

                piece.leftNeightbor = leftPiece;
                piece.rightNeightbor = rightPiece;

                CheckWin();

                centralButton.ChangeSprite(true);

                //enable mouse events
                EnableMouseEvents();
            });
        }




        public void RotateTableLeft()
        {
            //disable mouse events
            DisableMouseEvents();

            Sequence rotateSequence = DOTween.Sequence();

            for (int i = 0; i < spawnedPieceScripts.Count; i++)
            {
                Transform piece = spawnedPieceScripts[i].transform;
                Vector3 startRotation = piece.localEulerAngles;

                piece.DOKill();

                rotateSequence.Join(piece.transform.DOLocalRotate(startRotation + new Vector3(0, 0, 45), moveDuration).SetEase(Ease.InOutQuad));
            }

            rotateSequence.OnComplete(() =>
            {
                foreach (PieceScript item in spawnedPieceScripts)
                {
                    if (item.currentIndex == CENTRAL_PIECE_INDEX)
                        continue;

                    item.currentIndex = mod(item.currentIndex + 1, totalPieces);
                }

                rotateLeftButton.ChangeSprite(true);

                //enable mouse events
                EnableMouseEvents();
            });
        }

        public void RotateTableRight()
        {
            //disable mouse events
            DisableMouseEvents();

            Sequence rotateSequence = DOTween.Sequence();

            for (int i = 0; i < spawnedPieceScripts.Count; i++)
            {
                Transform piece = spawnedPieceScripts[i].transform;
                Vector3 startRotation = piece.localEulerAngles;

                piece.DOKill();

                rotateSequence.Join(piece.transform.DOLocalRotate(startRotation + new Vector3(0, 0, -45), moveDuration).SetEase(Ease.InOutQuad));
            }

            rotateSequence.OnComplete(() =>
            {
                foreach (PieceScript item in spawnedPieceScripts)
                {
                    if (item.currentIndex == CENTRAL_PIECE_INDEX)
                        continue;
                    item.currentIndex = mod(item.currentIndex - 1, totalPieces);
                }

                rotateRightButton.ChangeSprite(true);

                //enable mouse events
                EnableMouseEvents();
            });
        }

        public int mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }






        //------test functions-----
        public EventSystem eventSystem;
        public GameObject endPanel;
        public void DisableMouseEvents()
        {
            eventSystem.GetComponent<EventSystem>().enabled = false;
        }
        public void EnableMouseEvents()
        {
            eventSystem.GetComponent<EventSystem>().enabled = true;
        }
        public void LoadNextScene()
        {
            StartCoroutine(loadScene());
        }
        IEnumerator loadScene()
        {
            yield return new WaitForSeconds(0.1f);
            SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % (SceneManager.sceneCount+1));
        }
        public void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
