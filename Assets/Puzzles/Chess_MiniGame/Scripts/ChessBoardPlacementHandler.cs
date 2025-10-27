using System;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;


namespace com.puzzles.Chess_MiniGame
{
    public enum CellAvailability
    {
        None,
        Unavailable,
        Available,
        OutOfBounds
    }

    public sealed class ChessBoardPlacementHandler : MonoBehaviour {
        [SerializeField] private GameObject _highlightPrefab;
        [HideInInspector] public Cell[,] chessBoard;

        public List<GameObject> boxGlows = new List<GameObject>();
        public List<GameObject> cellUps = new List<GameObject>();
        public ChessMinigameManager manager;

        public void InitBoard(bool isReset)
        {
            foreach(GameObject cellUp in cellUps)
                cellUp.SetActive(false);
            if (isReset)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        chessBoard[i, j].UpdateCurrent(null);
                    }
                }

                return;
            }
            CreateChessBoard();

            GenerateArray();
        }

        private void CreateChessBoard()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    GameObject newCell = cellUps[y*8 + x];
                    newCell.SetActive(false);
                    newCell.transform.parent = manager.puzzleParent;
                }
            }
        }
        private void GenerateArray() 
        {
            chessBoard = new Cell[8, 8];
            for (var i = 0; i < 8; i++) {
                for (var j = 0; j < 8; j++) {
                    int index = i * 8 + j;
                    Cell cell = cellUps[i * 8 + j].GetComponent<Cell>();
                    chessBoard[i, j] = cell;

                    GameObject _highlightObject = null;
                    GameObject _cellUp = null;
                    if (index < boxGlows.Count)
                        _highlightObject = boxGlows[i * 8 + j];
                    if (index < cellUps.Count)
                        _cellUp = cellUps[i * 8 + j];

                    _cellUp.GetComponent<SpriteRenderer>().sortingOrder = 199 - i * 2;                  
                    
                    cell.Setup(new Vector2Int(i, j), _highlightObject, this);
                }
            }
        }

        public void PlaceEnemyPieces(BasePiece _dummyEnemy, List<Vector2Int> enemyCoord)
        {
            foreach(Vector2Int item in enemyCoord)
            {
                Vector2Int coord = new Vector2Int(item.y, item.x);
                Cell cell = GetTile(item.y, item.x);
                cell.InitCurrent(_dummyEnemy, coord, this);
            }
        }
        public void LiftCell(Vector2Int _coordinates)
        {
            GameObject _cellUp = cellUps[_coordinates.x * 8 + _coordinates.y];
            _cellUp.SetActive(true);



            manager.OnCorrectPlace();
        }
        internal Cell GetTile(int i, int j) 
        {
            try {
                return chessBoard[i, j];
            } catch (Exception) {
                print("Invalid row or column.");
                return null;
            }
        }
        public Cell FindClosestCell(Vector3 worldPosition)
        {
            Cell closestCell = null;
            float closestDistance = float.MaxValue;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Cell cell = chessBoard[x, y];
                    Vector3 cellPosition = cell.transform.localPosition;
                    float distance = Vector3.Distance(worldPosition, cellPosition);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCell = cell;
                    }
                }
            }

            return closestCell;
        }
        internal void ClearHighlights() {
            for (var i = 0; i < 8; i++) {
                for (var j = 0; j < 8; j++) {
                    var tile = GetTile(i, j);
                    tile.highlightObject.SetActive(false);           
                }
            }
        }


        public CellAvailability CheckCell(int potentialX, int potentialY, BasePiece checkingPiece)
        {
            if (potentialX < 0 || potentialX > 7)
                return CellAvailability.OutOfBounds;

            if (potentialY < 0 || potentialY > 7)
                return CellAvailability.OutOfBounds;

            Cell targetCell = chessBoard[potentialX, potentialY];

            if (targetCell.currentPiece != null)
            {
                return CellAvailability.Unavailable;
            }

            return CellAvailability.Available;
        }
    }
}
