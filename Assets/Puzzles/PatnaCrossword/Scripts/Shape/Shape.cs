using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.frameworks.PatnaCrossword
{
    public class Shape : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public GridData CurrentShapeData;
        public GameObject shapeSquare;
        public List<GameObject> shapeSquaresContainer = new List<GameObject>();

        private Vector3 _startPos;

        private List<GridSquare> occupyingGridSquares = new List<GridSquare>();

        private List<Vector2Int> targetPositions = new List<Vector2Int>();


        public bool OnCorrectPos = false;
        public void CreateShape(GridData shapeData, Vector3 initialPos, bool alreadyExists)
        {

            if (occupyingGridSquares.Count > 0)
            {
                foreach (GridSquare item in occupyingGridSquares)
                {
                    item.ResetOccupiedSquare();
                }
                occupyingGridSquares.Clear();
            }

            OnCorrectPos = false;

            _startPos = initialPos;

            if (alreadyExists)
                return;

            CurrentShapeData = shapeData;

            Color customColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            for (var i = 0; i < shapeData.rows; i++)
            {
                for (var j = 0; j < shapeData.columns; j++)
                {
                    if (shapeData.board[i].column[j])
                    {
                        GameObject newSquare = Instantiate(shapeSquare, transform);
                        ShapeSquare squareScript = newSquare.GetComponent<ShapeSquare>();

                        squareScript.InitializeSquare(shapeData, i, j, this, customColor);

                        shapeSquaresContainer.Add(newSquare);
                    }
                        
                }
            }
            if (shapeSquaresContainer.Count == 0)
                return;

            SpriteRenderer squareRenderer = shapeSquaresContainer[0].GetComponent<SpriteRenderer>();

            Vector2 moveDistance = new Vector2(squareRenderer.sprite.bounds.size.x * squareRenderer.transform.localScale.x,
                                                squareRenderer.sprite.bounds.size.y * squareRenderer.transform.localScale.y);

            int currentIndexInList = 0;

            for (int row = 0; row<shapeData.rows; row++)
            {
                for (int column = 0; column < shapeData.columns; column++)
                {
                    if (shapeData.board[row].column[column])    
                    {
                        shapeSquaresContainer[currentIndexInList].transform.localPosition =
                            new Vector2(GetXPosition(shapeData, column, moveDistance), GetYPosition(shapeData, row, moveDistance));
                        currentIndexInList++;


                        Vector2Int gridPosition = new Vector2Int(row, column);
                        targetPositions.Add(shapeData.initialTargetPosition + gridPosition);
                    }
                }
            }
        }

        public Vector2 FindFinalPosition(Vector3 startPosn, float squareSize)
        {
            Vector3 targetWorldPos = new Vector3(
                    startPosn.x + squareSize * (targetPositions[0].y),
                    startPosn.y - squareSize * (targetPositions[0].x)
                );

            Transform firstShapeSquare = shapeSquaresContainer[0].transform;
            Vector3 shapeChildLocalOffset = firstShapeSquare.localPosition;

            return (targetWorldPos - shapeChildLocalOffset);
        }

        public float GetYPosition(GridData shapeData, int row, Vector2 moveDistance)
        {
            float shiftOnY = 0;

            if (shapeData.rows > 1)
            {
                if (shapeData.rows % 2 != 0)
                {
                    var middleSquareIndex = (shapeData.rows - 1) / 2;
                    var multiplier = Mathf.Abs((shapeData.rows - 1) / 2 - row);
                    if (row < middleSquareIndex)
                    {
                        shiftOnY = moveDistance.y * 1;
                        shiftOnY *= multiplier;
                    }
                    else if (row > middleSquareIndex)
                    {
                        shiftOnY = moveDistance.y * -1;
                        shiftOnY *= multiplier;
                    }
                }
                else
                {
                    var middleSquareIndex2 = (shapeData.rows == 2) ? 1 : (shapeData.rows / 2);
                    var middleSquareIndex1 = (shapeData.rows == 2) ? 0 : (shapeData.rows / 2 - 1);
                    var multiplier = shapeData.rows / 2 - row;
                    if (multiplier <= 0)
                        multiplier = -1 * multiplier + 1;

                    if (row == middleSquareIndex1 || row == middleSquareIndex2)
                    {
                        if (row == middleSquareIndex2)
                        {
                            shiftOnY = (moveDistance.y / 2) * -1;
                        }

                        if (row == middleSquareIndex1)
                        {
                            shiftOnY = (moveDistance.y) / 2;
                        }
                    }

                    if (row < middleSquareIndex1 && row < middleSquareIndex2)
                    {
                        shiftOnY = moveDistance.y * 1;
                        shiftOnY *= multiplier;
                        shiftOnY -= (moveDistance.y / 2);
                    }
                    else if (row > middleSquareIndex1 && row > middleSquareIndex2)
                    {
                        shiftOnY = moveDistance.y * -1;
                        shiftOnY *= multiplier;
                        shiftOnY += (moveDistance.y / 2);
                    }
                }
            }

            return shiftOnY;
        }


        public float GetXPosition(GridData shapeData, int column, Vector2 moveDistance)
        {
            float shiftOnX = 0;

            if(shapeData.columns > 1)
            {
                if (shapeData.columns % 2 != 0)
                {
                    var middleSquareIndex = (shapeData.columns - 1) / 2;
                    int multiplier = Mathf.Abs((shapeData.columns - 1) / 2 - column);
                    if (column < middleSquareIndex)
                    {
                        shiftOnX = moveDistance.x * -1;
                        shiftOnX *= multiplier;
                    }
                    else if (column > middleSquareIndex)
                    {
                        shiftOnX = moveDistance.x * 1;
                        shiftOnX *= multiplier;
                    }
                }
                else
                {
                    var middleSquareIndex2 = (shapeData.columns == 2) ? 1 : (shapeData.columns / 2);
                    var middleSquareIndex1 = (shapeData.columns == 2) ? 0 : (shapeData.columns / 2 - 1);
                    var multiplier = (shapeData.columns) / 2 - column;
                    if (multiplier <= 0)
                        multiplier = -1 * multiplier + 1;
                    if(column==middleSquareIndex1 || column == middleSquareIndex2)
                    {
                        if (column == middleSquareIndex2)
                        {
                            shiftOnX = moveDistance.x / 2;
                        }

                        if (column == middleSquareIndex1)
                        {
                            shiftOnX = (moveDistance.x / 2) * -1;
                        }
                    }

                    if (column<middleSquareIndex1 && column < middleSquareIndex2)
                    {
                        shiftOnX = moveDistance.x * -1f;
                        shiftOnX *= multiplier;
                        shiftOnX += (moveDistance.x / 2);
                    }
                    else if (column > middleSquareIndex1 && column > middleSquareIndex2)
                    {
                        shiftOnX = moveDistance.x * 1f;
                        shiftOnX *= multiplier;
                        shiftOnX -= (moveDistance.x / 2);
                    }
                }
            }

            return shiftOnX;
        }


        public int GetTotalActiveSquares(GridData shapeData)
        {
            int total = 0;

            for (var i = 0; i < shapeData.rows; i++)
            {
                for (var j = 0; j < shapeData.columns; j++)
                {
                    if (shapeData.board[i].column[j])
                        total++;
                }
            }

            return total;
        }



        public void ReturnToStartPos()
        {
            transform.localPosition = _startPos;
        }

        public void SnapToGrid(GridSquare gridSquare, Vector3 gridCellPositionOffset)
        {

            if (!occupyingGridSquares.Contains(gridSquare))
                occupyingGridSquares.Add(gridSquare);

            Vector3 startPos = transform.localPosition;
            Vector3 newPos = startPos + gridCellPositionOffset;

            transform.localPosition = newPos;

            if (CheckIfPlaceCorrectly())
            {
                OnCorrectPos = true;
                BlockPuzzleFrameworkManager.OnCorrectPosition();
            }
        }

        private bool CheckIfPlaceCorrectly()
        {
            if (occupyingGridSquares.Count != targetPositions.Count)
                return false;
            foreach(GridSquare item in occupyingGridSquares)
            {
                Vector2Int gridSquarePos = new Vector2Int(item.RowIndex, item.ColumnIndex);

                if (!targetPositions.Contains(gridSquarePos))
                    return false;
            }
            return true;
        }


        public void PlaceOnCorrect(Vector2 finalPos)
        {
            if (occupyingGridSquares.Count > 0)
            {
                foreach (GridSquare item in occupyingGridSquares)
                {
                    item.ResetOccupiedSquare();
                }
                occupyingGridSquares.Clear();
            }
            transform.localPosition = finalPos;

            OnCorrectPos = true;
        }



        private Vector3 offset;
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (OnCorrectPos)
                return;

            if (occupyingGridSquares.Count > 0)
            {
                foreach(GridSquare item in occupyingGridSquares)
                {
                    item.ResetOccupiedSquare();
                }
                occupyingGridSquares.Clear();
            }
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePos.z = 10;
            offset = transform.position - worldMousePos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (OnCorrectPos)
                return;

            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePos.z = 10;
            transform.position = worldMousePos + offset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (OnCorrectPos)
                return;
            BlockPuzzleFrameworkManager.CheckIfShapeCanBePlaced?.Invoke(this);
        }
    }
}
