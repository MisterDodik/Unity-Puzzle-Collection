using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.Chess_MiniGame
{
    public class Rook : BasePiece
    {
        public override void Setup(Cell pieceCell, Vector2Int _finalPosition, ChessBoardPlacementHandler _chessBoardPlacementHandler)
        {
            // Base setup
            base.Setup(pieceCell, _finalPosition, _chessBoardPlacementHandler);

            movementDirection = new Vector3Int(7, 7, 0);        // Rook can move up to 7 cells horizontally or vertically, but not diagonally
        }
    }
}
