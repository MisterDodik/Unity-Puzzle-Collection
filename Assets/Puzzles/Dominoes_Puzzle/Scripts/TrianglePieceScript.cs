using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.Dominoes_Puzzle
{
    public class TrianglePieceScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        private Vector3 offset;

        private int startingOrder;
        [HideInInspector] public SpriteRenderer spriteRenderer;

        private DominoesPuzzleManager manager;
        private Transform glow;
        public Vector3 endPos;
        private Vector3 startPos;

        private bool isCorrect = false;
        private bool isActive = false;

        [SerializeField] private int isUp = 1;

        private List<TrianglePieceScript> trianglesToActivate = new List<TrianglePieceScript>();
        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            startingOrder = spriteRenderer.sortingOrder;
            
            if (startPos == Vector3.zero)
                startPos = transform.localPosition;
        }

        public void LoadTriangle(DominoesPuzzleManager _manager, Vector3 _endPos, List<TrianglePieceScript> _trianglesToActivate)
        {
            isCorrect = false;
            isActive = false;

            manager = _manager;
            endPos = _endPos;

            trianglesToActivate = _trianglesToActivate;

            Start();
            transform.localPosition = startPos;
        }


        private void TriggerGlow(bool isActive)
        {
            if (glow == null)
                glow = manager.glow.transform;
            if (isActive)
            {
                glow.SetParent(transform);
                glow.transform.localPosition = Vector3.zero;
                glow.transform.localScale = new Vector3(1, isUp, 1);

                glow.gameObject.SetActive(true);
                
            }
            else
            {
                if (glow == null)
                    return;
                glow.SetParent(manager.puzzleBg);
                glow.gameObject.SetActive(false);
            }
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isCorrect)
                return;

            TriggerGlow(true);

            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePos.z = 10;
            offset = transform.localPosition - worldMousePos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isCorrect)
                return;
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePos.z = 10;

            Vector2 newPos = worldMousePos + offset;
            transform.localPosition = newPos;

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isCorrect)
                return;
            TriggerGlow(true);

            spriteRenderer.sortingOrder += 10;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            TriggerGlow(false);
            if(!isActive)
            {
                transform.localPosition = startPos;
                return;
            }
            OnPointerUpEvents();
        }

        public void OnPointerUpEvents()
        {
            spriteRenderer.sortingOrder = startingOrder;

            if (isCorrect)
                return;
            if ((transform.localPosition - endPos).sqrMagnitude < 0.2f)
            {
                transform.localPosition = endPos;
                isCorrect = true;
                manager.CheckWin();

                foreach (TrianglePieceScript triangle in trianglesToActivate)
                    triangle.ActivateTriangle(true);
            }
            else
            {
                transform.localPosition = startPos;
            }
        }


        public void ActivateTriangle(bool state)
        {
            isActive = state;
        }
    }
}
