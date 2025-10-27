using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.Chess_MiniGame
{
    public class Queen : BasePiece
    {
        public override void Setup(Cell pieceCell, Vector2Int _finalPosition, ChessBoardPlacementHandler _chessBoardPlacementHandler)
        {
            // Base setup
            base.Setup(pieceCell, _finalPosition, _chessBoardPlacementHandler);

            movementDirection = new Vector3Int(7, 7, 7);    // Queen can move up to 7 cells horizontally, vertically, and diagonally
        }
    }
}
