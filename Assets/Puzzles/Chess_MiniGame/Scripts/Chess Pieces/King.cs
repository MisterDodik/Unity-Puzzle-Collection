using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.Chess_MiniGame
{
    public class King : BasePiece
    {
        public override void Setup(Cell pieceCell, Vector2Int _finalPosition, ChessBoardPlacementHandler _chessBoardPlacementHandler)
        {
            // Base setup
            base.Setup(pieceCell, _finalPosition, _chessBoardPlacementHandler);

            movementDirection = new Vector3Int(1, 1, 1);    // King can move 1 cell in any direction: horizontally, vertically, or diagonally
        }
    }
}
