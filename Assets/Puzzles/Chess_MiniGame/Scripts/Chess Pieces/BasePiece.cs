using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace com.puzzles.Chess_MiniGame
{

    public abstract class BasePiece : EventTrigger
    {
        private ChessBoardPlacementHandler chessBoardPlacementHandler;
        protected Cell currentCell = null;

        protected Vector3Int movementDirection = Vector3Int.one;    // Every piece has its unique movement rules        

        protected List<Cell> highlightedCells = new List<Cell>();

        private Vector2Int finalPosition;
        private bool isCorrect = false;

        private SpriteRenderer spriteRenderer;

        // Initial setup
        public virtual void Setup(Cell pieceCell, Vector2Int _finalPosition, ChessBoardPlacementHandler _chessBoardPlacementHandler)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            currentCell = pieceCell;
            chessBoardPlacementHandler = _chessBoardPlacementHandler;
            finalPosition = _finalPosition;

            isCorrect = false;
            CheckPosition();
        }
        public Tween Skip()
        {
            if (isCorrect)
                return null;
            Cell cell = chessBoardPlacementHandler.GetTile(finalPosition.x, finalPosition.y);
            return transform.DOLocalMove(cell.transform.localPosition, .5f).OnComplete(() =>
            {
                Cell potentialNewCell = chessBoardPlacementHandler.FindClosestCell(transform.localPosition);
                currentCell.UpdateCurrent(null);
                potentialNewCell.UpdateCurrent(this);

                CheckPosition();
                ClearCells();
            });
        }
        public void UpdatePiece(Cell pieceCell)
        {
            currentCell = pieceCell;
        }
        private void CreateCellPath(int xDirection, int yDirection, int movement)
        {
            // Target position
            int currentX = currentCell.boardPosition.x;
            int currentY = currentCell.boardPosition.y;
            // Check each cell
            for (int i = 1; i <= movement; i++)
            {
                currentX += xDirection;
                currentY += yDirection;

                // Get the state of the target cell
                CellAvailability cellState = CellAvailability.None;
                cellState = currentCell._Board.CheckCell(currentX, currentY, this);

                // If the cell is not free, break
                if (cellState != CellAvailability.Available)
                    break;

                // Add to list
                highlightedCells.Add(currentCell._Board.chessBoard[currentX, currentY]);
            }
        }

        // Checks all 8 directions separately
        protected virtual void CheckPathing()
        {
            // Horizontal
            CreateCellPath(1, 0, movementDirection.x);
            CreateCellPath(-1, 0, movementDirection.x);

            // Vertical 
            CreateCellPath(0, 1, movementDirection.y);
            CreateCellPath(0, -1, movementDirection.y);

            // Upper diagonal
            CreateCellPath(1, 1, movementDirection.z);
            CreateCellPath(-1, 1, movementDirection.z);

            // Lower diagonal
            CreateCellPath(-1, -1, movementDirection.z);
            CreateCellPath(1, -1, movementDirection.z);
        }



        public override void OnPointerDown(PointerEventData eventData)
        {
            if (isCorrect)
                return;
            base.OnPointerDown(eventData);
            lastPosition = transform.localPosition;
            if (transform.childCount > 0)
                transform.GetChild(0).gameObject.SetActive(true);
            CheckPathing();
            ShowCells();
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (isCorrect)
                return;
            base.OnPointerUp(eventData);

            if (transform.childCount > 0)
                transform.GetChild(0).gameObject.SetActive(false);

            if((lastPosition - (Vector2)transform.localPosition).sqrMagnitude < 0.1f)
                ClearCells();
        }

        private Vector2 lastPosition;
        private Vector3 offset;
        private int lastSortingOrder;
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (isCorrect)
                return;
            base.OnBeginDrag(eventData);
            lastPosition = transform.localPosition;
            lastSortingOrder = spriteRenderer.sortingOrder;

            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePos.z = 10;
            offset = transform.localPosition - worldMousePos;
        }
        public override void OnDrag(PointerEventData eventData)
        {
            if (isCorrect)
                return;
            base.OnDrag(eventData);

            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePos.z = 10;
            Vector2 newPos = worldMousePos + offset;
            transform.localPosition = newPos;

            Cell potentialNewCell = chessBoardPlacementHandler.FindClosestCell(transform.localPosition);
            if (potentialNewCell == null)
                return;

            int sortingOrder = lastSortingOrder;
            if (potentialNewCell.currentPiece == null)
                sortingOrder = 200 - potentialNewCell.boardPosition.x * 2;
            else
                sortingOrder = 300;

            spriteRenderer.sortingOrder = sortingOrder;
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (isCorrect)
                return;

            base.OnEndDrag(eventData);
            Cell potentialNewCell = chessBoardPlacementHandler.FindClosestCell(transform.localPosition);
            if (!highlightedCells.Contains(potentialNewCell))
            {
                transform.localPosition = lastPosition;

                spriteRenderer.sortingOrder = lastSortingOrder;
                transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = lastSortingOrder;
            }
            else
            {
                transform.localPosition = potentialNewCell.transform.localPosition;
                currentCell.UpdateCurrent(null);
                potentialNewCell.UpdateCurrent(this);

                CheckPosition();
            }
            ClearCells();
        }

        private void CheckPosition()
        {
            int sortingOrder = 200 - currentCell.boardPosition.x * 2;
            spriteRenderer.sortingOrder = sortingOrder;
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;

            float scale = 1f - (currentCell.boardPosition.x / 7f) * 0.25f;
            transform.localScale = new Vector3(scale, scale, scale);
            if (currentCell.boardPosition == finalPosition)
            {
                Vector3 currentPos = transform.localPosition;
                currentPos.y += 0.45f;
                transform.localPosition = currentPos;

                isCorrect = true;
                chessBoardPlacementHandler.LiftCell(finalPosition);
            }
        }

        protected void ShowCells()
        {
            foreach (Cell item in highlightedCells)
                item.HighlightSelf();
        }

        protected void ClearCells()
        {
            chessBoardPlacementHandler.ClearHighlights();
            highlightedCells.Clear();
        }
    }
}
