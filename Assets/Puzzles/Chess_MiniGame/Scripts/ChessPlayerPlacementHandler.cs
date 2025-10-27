using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace com.puzzles.Chess_MiniGame
{
    public class ChessPlayerPlacementHandler : MonoBehaviour 
    {
        private int row, column;
        private Vector2Int finalPosition;
        private ChessBoardPlacementHandler chessBoardPlacementHandler;
        public BasePiece pieceType;
        public void InitPiece(Vector2Int _coordinates, Vector2Int _finalPosition, ChessBoardPlacementHandler _chessBoardPlacementHandler)
        {
            row = _coordinates.y;
            column = _coordinates.x;
            finalPosition = new Vector2Int(_finalPosition.y, _finalPosition.x);
            chessBoardPlacementHandler = _chessBoardPlacementHandler;
            pieceType = GetComponent<BasePiece>();

            PlacePiece();
        }
        public void PlacePiece()
        {           
            Cell cell = chessBoardPlacementHandler.GetTile(row, column);
            transform.localPosition = cell.transform.localPosition;
            cell.InitCurrent(pieceType, finalPosition, chessBoardPlacementHandler);
        }
       
        private void Start() 
        {          
            if (GetComponent<BoxCollider2D>()==null)
            {
                gameObject.AddComponent<BoxCollider2D>();
            }          
        }
    }
}