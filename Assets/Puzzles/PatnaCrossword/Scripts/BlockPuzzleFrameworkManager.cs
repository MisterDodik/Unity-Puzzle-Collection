using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.frameworks.PatnaCrossword
{
    public abstract class BlockPuzzleFrameworkManager : MonoBehaviour
    {
        [SerializeField] private GameObject squarePrefab;
        [SerializeField] private GridData gridData;

        public Transform puzzleTransform;

        private List<GameObject> spawnedSquares = new List<GameObject>();
        private List<GameObject> newlyAddedSquares = new List<GameObject>();
        private List<GameObject> spawnedShapes = new List<GameObject>();
        private List<GameObject> newlyAddedShapes = new List<GameObject>();


        public GameObject shapePrefab;
        public Transform shapeParent;
        public List<GridData> shapeData = new List<GridData>();
        private List<Vector3> initialShapePositions = new List<Vector3>();
        private List<Vector3> finalShapePositions = new List<Vector3>();

        public static Action<Shape> CheckIfShapeCanBePlaced;
        public static Action OnCorrectPosition;

        private int winCondition = 0;
        private int winProgress = 0;
        private void OnEnable()
        {
            CheckIfShapeCanBePlaced += CheckIfShapeFits;
            OnCorrectPosition += CheckWin;
        }
        private void OnDisable()
        {
            CheckIfShapeCanBePlaced -= CheckIfShapeFits;
            OnCorrectPosition -= CheckWin;
        }

        private void CheckIfShapeFits(Shape shape)
        {
            var squareIndexes = new List<GridSquare>();

            foreach (GameObject item in spawnedSquares)
            {
                GridSquare gridSquare = item.GetComponent<GridSquare>();

                if (!gridSquare.isOccupied && gridSquare.isSelected)
                {
                    squareIndexes.Add(gridSquare);
                }
                
            }

            if (shape.shapeSquaresContainer.Count == squareIndexes.Count)
            {
                foreach(GridSquare item in squareIndexes)
                {
                    item.PlaceShapePieceHere();
                }
            }
            else
            {
                shape.ReturnToStartPos();
            }
        }
        private void CheckWin()
        {
            winProgress++;
            if (winCondition == winProgress)
                PuzzleCompleted();
        }



        float squareSize;
        Vector3 startPosn;
        Vector3 refPosn;

        protected void InitializePuzzle()
        {
            LoadPuzzle(false);
        }
        protected void ResetPuzzle()
        {
            LoadPuzzle(true);
        }


        protected void SkipPuzzle()
        {
            for (var i = 0; i < spawnedShapes.Count; i++)
            {
                Shape shape = spawnedShapes[i].GetComponent<Shape>();

                if (shape.OnCorrectPos)
                    continue;

                shape.PlaceOnCorrect(finalShapePositions[i]);               
            }
            PuzzleCompleted();
        }

        public abstract void PuzzleCompleted();

        private void LoadPuzzle(bool isReset)
        {
            winProgress = 0;

            InitGrid();
            InitShapes(isReset);
        }


        private void InitGrid()
        {
            newlyAddedSquares.Clear();
            var objCounter = 0;

            squareSize = squarePrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
            refPosn = new Vector3(0.7f, -0.05f);
            startPosn = new Vector3(refPosn.x - squareSize * gridData.columns / 2, refPosn.y + squareSize * gridData.rows / 2);

            for (var i = 0; i < gridData.rows; i++)
            {
                for (var j = 0; j < gridData.columns; j++)
                {
                    if (gridData.board[i].column[j])
                    {
                        GameObject obj;
                        if (spawnedSquares.Count < (objCounter + 1))
                        {
                            obj = Instantiate(squarePrefab, puzzleTransform);
                            newlyAddedSquares.Add(obj);
                        }
                        else
                            obj = spawnedSquares[objCounter];

                        obj.SetActive(true);
                        var posn = new Vector3(startPosn.x + squareSize * (0.5f + j),
                            startPosn.y - squareSize * (0.5f + i));
                        obj.transform.localPosition = posn;
                        var square = obj.GetComponent<GridSquare>();
                        square.InitGridSquare(i, j);
                        objCounter++;
                    }

                }
            }

            spawnedSquares.AddRange(newlyAddedSquares);
        }


        private void InitShapes(bool isReset)
        {
            if (!isReset)
            {
                foreach (GameObject item in spawnedShapes)
                    Destroy(item);
                spawnedShapes.Clear();
            }

            winCondition = shapeData.Count;

            newlyAddedShapes.Clear();
            finalShapePositions.Clear();

            var objCounter = 0;

            for (int i = 0; i < shapeData.Count; i++)
            {
                GameObject shape;

                bool alreadyExists = true;
                if (spawnedShapes.Count < (objCounter + 1))
                {
                    shape = Instantiate(shapePrefab, shapeParent); ;
                    newlyAddedShapes.Add(shape);
                    alreadyExists = false;
                }
                else
                    shape = spawnedShapes[objCounter];
                shape.SetActive(true);
                objCounter++;


                Vector3 position;
                if (i < initialShapePositions.Count)
                    position = initialShapePositions[i];
                else
                {
                    float x;
                    if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
                        x = UnityEngine.Random.Range(-8f, -5f);
                    else
                        x = UnityEngine.Random.Range(5f, 10f);

                    float y = UnityEngine.Random.Range(-4f, 4f);
                    position = new Vector3(x, y, 0f);
                }
                shape.transform.localPosition = position;

                shape.GetComponent<Shape>().CreateShape(shapeData[i], position, alreadyExists);

                finalShapePositions.Add(shape.GetComponent<Shape>().FindFinalPosition(new Vector3(startPosn.x + squareSize * (0.5f),
                            startPosn.y - squareSize * (0.5f)), squareSize));
            }

            spawnedShapes.AddRange(newlyAddedShapes);
        }
    }
}