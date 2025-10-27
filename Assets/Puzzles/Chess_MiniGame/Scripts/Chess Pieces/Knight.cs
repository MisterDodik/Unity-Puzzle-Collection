using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.Chess_MiniGame
{
    public class Knight : BasePiece
    {
        private void CreateCellPath(int flipper)
        {
            // Target position
            int currentX = currentCell.boardPosition.x;
            int currentY = currentCell.boardPosition.y;

            // Left
            MatchesState(currentX - 2, currentY + (1 * flipper));

            // Upper left
            MatchesState(currentX - 1, currentY + (2 * flipper));

            // Upper right
            MatchesState(currentX + 1, currentY + (2 * flipper));

            // Right
            MatchesState(currentX + 2, currentY + (1 * flipper));
        }

        protected override void CheckPathing()
        {
            // Draw top half
            CreateCellPath(1);

            // Draw bottom half
            CreateCellPath(-1);
        }

        private void MatchesState(int targetX, int targetY)
        {
            CellAvailability cellState = CellAvailability.None;
            cellState = currentCell._Board.CheckCell(targetX, targetY, this);

            if (cellState == CellAvailability.Available && cellState != CellAvailability.OutOfBounds)
                highlightedCells.Add(currentCell._Board.chessBoard[targetX, targetY]);

        }
    }
}
