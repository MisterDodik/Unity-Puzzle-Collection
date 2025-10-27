using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.frameworks.PatnaCrossword
{
    public class ShapeSquare : MonoBehaviour
    {
        public Shape parentObject;

        public void InitializeSquare(GridData squareData, int row, int column, Shape parent, Color color)
        {
            if (squareData == null)
                return;
            if (squareData.defaultSprite != null)
            {
                GetComponent<SpriteRenderer>().sprite = squareData.defaultSprite;
            }
            else
            {
                Sprite sprite = squareData.board[row].sprites[column];
                if (sprite!=null)
                    GetComponent<SpriteRenderer>().sprite = sprite;
            }
            GetComponent<SpriteRenderer>().color = color;

            parentObject = parent;
        }
    }
}
