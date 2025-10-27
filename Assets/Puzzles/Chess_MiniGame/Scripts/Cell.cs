using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace com.puzzles.Chess_MiniGame
{
    public class Cell : MonoBehaviour
    {
        [HideInInspector] public Vector2Int boardPosition = Vector2Int.zero;

        [HideInInspector] public ChessBoardPlacementHandler _Board = null;

        [HideInInspector] public BasePiece currentPiece = null;

        [HideInInspector] public GameObject highlightObject;

        [HideInInspector] public GameObject cellUp;

        //Cell Data
        public void Setup(Vector2Int newBoardPosition, GameObject _highlightObject, ChessBoardPlacementHandler newBoard)
        {
            boardPosition = newBoardPosition;
            highlightObject = _highlightObject;
            if (highlightObject != null) 
                highlightObject.SetActive(false);
            _Board = newBoard;
        }

        // Initial piece setup
        public void InitCurrent(BasePiece piece, Vector2Int _finalPosition, ChessBoardPlacementHandler _chessBoardPlacementHandler)
        {    
            currentPiece = piece;
            if (piece.transform.childCount > 0)
                piece.transform.GetChild(0).gameObject.SetActive(false);
            piece.Setup(this, _finalPosition, _chessBoardPlacementHandler);
        }
        public void UpdateCurrent(BasePiece piece)
        {
            if (piece == null)
            {
                if (currentPiece == null)
                    return;
                currentPiece.UpdatePiece(null);
                currentPiece = piece;
                return;
            }
            currentPiece = piece;
            if (piece.transform.childCount > 0)
                piece.transform.GetChild(0).gameObject.SetActive(false);
            currentPiece.UpdatePiece(this);
        }

        // Called from Piece script, spawns red or green highlight based on availability of the cell
        public void HighlightSelf()
        {
            highlightObject.SetActive(true);
        }
    }

}
