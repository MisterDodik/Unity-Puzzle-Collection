using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.Box_Puzzle
{
    public enum State
    {
        Up,
        Down,
        Center
    }
    public class PlankScript : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public BoxPuzzleManager manager;

        private bool wasCorrect = false;
        private State correctPosition;

        private Vector3 offset;

        private Vector3 start;
        private Vector3 moveDirection;
        
        private float maxDistance = 1.5f;
        private float downBound;
        private float upBound;

        private SpriteRenderer spriteRenderer;
        private int startSortingOrder;

        public State currentState = State.Center;
        public PlankScript neighborPlank;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            startSortingOrder = spriteRenderer.sortingOrder;
        }
        public void Init(BoxPuzzleManager _manager, Vector3 startPosition, Vector3 pivotPosition, State _correctState)
        {
            transform.localPosition = startPosition;
            start = startPosition;

            correctPosition = _correctState;
            manager = _manager;
            moveDirection = start - pivotPosition;
            moveDirection.Normalize();

            downBound = -maxDistance;
            upBound = maxDistance;
            wasCorrect = false;

            if(spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                startSortingOrder = spriteRenderer.sortingOrder;
            }
            spriteRenderer.sortingOrder = startSortingOrder;
            currentState = State.Center;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (neighborPlank != null)
            {
                if(neighborPlank.currentState == State.Up)
                {
                    downBound = 0;
                }
                else
                {
                    downBound = -maxDistance;
                }
            }
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePos.z = 10;
            offset = transform.localPosition - worldMousePos;
        }

        public void OnDrag(PointerEventData eventData)
        {

            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePos.z = 10;
            Vector3 relative = (worldMousePos + offset) - start;

            Vector3 projected = Vector3.Project(relative, moveDirection);

            float distance = Vector3.Dot(projected, moveDirection);
            distance = Mathf.Clamp(distance, downBound, upBound);

            if (distance > .05f)
            {
                currentState = State.Down;
                spriteRenderer.sortingOrder = startSortingOrder;
            }
            else if (distance < -.05f)
            {
                currentState = State.Up;           
                spriteRenderer.sortingOrder = startSortingOrder + 2;
            }
            else
            {
                currentState = State.Center;
                spriteRenderer.sortingOrder = startSortingOrder;
            }
            transform.localPosition = start + moveDirection * distance;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            float distance = Vector3.Dot(transform.localPosition - start, moveDirection);
            distance = Mathf.Abs(distance);
            if (currentState == correctPosition && Mathf.Abs(distance - maxDistance) < 0.05f)
            {
                manager.CheckWin(true, wasCorrect);

                wasCorrect = true;
            }
            else
            {
                manager.CheckWin(false, wasCorrect);

                wasCorrect = false;
            }
        }
        public void SkipPlank()
        {
            int sign = correctPosition == State.Down ? 1 : -1;
            transform.localPosition = start + sign * moveDirection * maxDistance;
        }
    }
}
