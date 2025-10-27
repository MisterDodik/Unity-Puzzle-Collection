using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.Chess_MiniGame
{
    public class Pawn : BasePiece
    {
        public override void Setup(Cell pieceCell, Vector2Int _finalPosition, ChessBoardPlacementHandler _chessBoardPlacementHandler)
        {
            // Base setup
            base.Setup(pieceCell, _finalPosition, _chessBoardPlacementHandler);

            movementDirection = new Vector3Int(0, 1, 1);     
            //movementDirection = _Color == Color.white ? new Vector3Int(0, -1, -1) : new Vector3Int(0, 1, 1);    // White pawns start at the top and move downward;
                                                                                                                // Black pawns start at the bottom and move upward
        }
        private bool MatchesState(int targetX, int targetY, CellAvailability targetState)
        {
            CellAvailability cellState = CellAvailability.None;
            cellState = currentCell._Board.CheckCell(targetX, targetY, this);

            if (cellState == targetState)
            {
                highlightedCells.Add(currentCell._Board.chessBoard[targetX, targetY]);
                return true;
            }

            return false;
        }

        protected override void CheckPathing()
        {
            // Target position
            int currentX = currentCell.boardPosition.x;
            int currentY = currentCell.boardPosition.y;

            // Forward
            MatchesState(currentX + movementDirection.y, currentY , CellAvailability.Available);

        }
    }
}
