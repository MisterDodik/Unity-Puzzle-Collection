using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.frameworks.PatnaCrossword
{
    public class GridSquare : MonoBehaviour
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public bool isOccupied { get; set; }
        public bool isSelected { get; set; }
        public Shape Shape { get; set; }
        public ShapeSquare ShapeSquare { get; set; }

        private List<ShapeSquare> placedShapeSquares = new List<ShapeSquare>();
        private void Start()
        {
            isSelected = false;
            isOccupied = false;
            Shape = null;
            ShapeSquare = null;
        }
        public void InitGridSquare(int row, int col)
        {
            RowIndex = row;
            ColumnIndex = col;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isOccupied)
                return;
            isSelected = true;

            ShapeSquare potentialSquare = collision.GetComponent<ShapeSquare>();
            if (potentialSquare != null)
            {
                Shape = potentialSquare.parentObject;
                ShapeSquare = potentialSquare;
            }

            //GetComponent<SpriteRenderer>().color = Color.black;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            isSelected = true;
            if (isOccupied)
                return;

            ShapeSquare potentialSquare = collision.GetComponent<ShapeSquare>();
            if (potentialSquare != null)
            {
                Shape = potentialSquare.parentObject;
                ShapeSquare = potentialSquare;
            }

            GetComponent<SpriteRenderer>().color = Color.black;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (isOccupied)
                return;
            ShapeSquare potentialSquare = collision.GetComponent<ShapeSquare>();
            if (potentialSquare != null)
            {
                Shape = null;
                ShapeSquare = null;
            }
            isSelected = false;
            isOccupied = false;
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        public void PlaceShapePieceHere()
        {
            isSelected = false;
            isOccupied = true;
            GetComponent<SpriteRenderer>().color = Color.black;

            
            Shape.SnapToGrid(this, transform.parent.InverseTransformPoint(transform.position) - transform.parent.InverseTransformPoint(ShapeSquare.transform.position));
        }

        public void ResetOccupiedSquare()
        {
            isOccupied = false;
            Shape = null;
            ShapeSquare = null;
        }
    }
}