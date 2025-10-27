using System;
using UnityEngine;

namespace com.frameworks.PatnaCrossword
{
    [CreateAssetMenu]
    [Serializable]
    public class GridData : ScriptableObject
    {
        public int rows = 0;
        public int columns = 0;
        public Sprite defaultSprite;    //use if all square blocks use the same sprite
        public int startIndex = 0;
        public int destinationIndex = 0;
        public Grid[] board;


        public Vector2Int initialTargetPosition;       //top-left corner 

        [Serializable]
        public class Grid
        {
            public bool[] column;
            public Sprite[] sprites;
            private int _size = 0;

            public Grid() { }

            public Grid(int size)
            {
                CreateGrid(size);
            }

            public void CreateGrid(int size)
            {
                _size = size;
                column = new bool[_size];
                sprites = new Sprite[_size];
                ClearGrid();
            }

            public void ClearGrid()
            {
                for (var i = 0; i < _size; i++)
                {
                    column[i] = false;
                    sprites[i] = null;
                }
            }
        }

        public void Clear()
        {
            for (var i = 0; i < rows; i++)
            {
                board[i].ClearGrid();
            }
        }

        public void CreateLevelBoard()
        {
            board = new Grid[rows];

            for (var i = 0; i < rows; i++)
            {
                board[i] = new Grid(columns);
            }
        }

    }
}


