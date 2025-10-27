using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.MaskPuzzle
{
    public class MaskScript : MonoBehaviour, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private MaskPuzzleManager gameManager;
        private Vector2 startPosition;
        public Vector2 correctPosition;

        public void InitMask(MaskPuzzleManager manager, Vector2 startPos, Vector2 correctPos)
        {
            gameManager = manager;
            startPosition = startPos;
            correctPosition = correctPos;
        }

        private Vector3 offset;
        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePos.z = 10;
            offset = transform.position - worldMousePos;
        }

        public void OnDrag(PointerEventData eventData)
        {   
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePos.z = 10;
            transform.position = worldMousePos + offset;

           
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (Vector2.Distance(transform.localPosition, startPosition) < 0.5f)
            {
                transform.localPosition = startPosition;
                gameManager.MoveMasks(transform);
            }
        }

        public bool CorrectPosition()
        {
            if ((Vector2)transform.localPosition == correctPosition)
                return true;
            return false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }
    }

}
