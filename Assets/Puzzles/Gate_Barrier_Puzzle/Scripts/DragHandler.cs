using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.Gate_Barrier_Puzzle
{
    public enum LightHint
    {
        None,
        Left,
        Right,
        Top, 
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        All 
    }
    public class DragHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Vector2 bottomLeftBorder;
        [SerializeField] private Vector2 topRightBorder;

        private Vector3 offset;
        private Vector3 logicPosition;
        private Vector2 halfMapSize;

        private GateBarrierPuzzleManager manager;
        [SerializeField] private Transform navigator;

        private Coroutine autoScrollCoroutine;
        private float moveSpeed = 50;

        private bool isDragging = false;
        [HideInInspector] public bool dragPaused = false;

        private Vector3 currentEctoplasm;
        private float absorbDistance = 5f;
        private float directionThreshold = 5f;

        private LightHint currentHint = LightHint.None;

        [SerializeField] private List<GameObject> indicators = new List<GameObject>();  

        public void InitDevice(GateBarrierPuzzleManager _manager)
        {
            manager = _manager;

            halfMapSize = manager.mapSize / 2f;
            currentEctoplasm = manager.CurrentEctoplasm();

            ResetIndicator();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (dragPaused)
                return;
            isDragging = true;
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePos.z = 10;
            offset = transform.position - worldMousePos;

            if (autoScrollCoroutine == null)
            {
                autoScrollCoroutine = StartCoroutine(AutoScrollWhileDragging());
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            if (autoScrollCoroutine != null)
            {
                StopCoroutine(autoScrollCoroutine);
                autoScrollCoroutine = null;
            }
        }
        
        private IEnumerator AutoScrollWhileDragging()
        {
            while (isDragging)
            {
                Vector3 screenPointer = GetPointerPosition();

                Vector3 worldPointer = Camera.main.ScreenToWorldPoint(screenPointer);
                worldPointer.z = 10;

                Vector3 newPos = worldPointer + offset;
                Vector3 delta = newPos - transform.position;

                Vector3 clampedPos = new Vector3(
                    Mathf.Clamp(newPos.x, bottomLeftBorder.x, topRightBorder.x),
                    Mathf.Clamp(newPos.y, bottomLeftBorder.y, topRightBorder.y),
                    newPos.z
                );
                transform.position = clampedPos;

               
                bool outsideX = newPos.x < bottomLeftBorder.x || newPos.x > topRightBorder.x;
                bool outsideY = newPos.y < bottomLeftBorder.y || newPos.y > topRightBorder.y;

                if (outsideX || outsideY)
                {
                    Vector3 scrollDir = delta.normalized;
                    logicPosition += scrollDir * (moveSpeed * Time.deltaTime);
                }
                else
                {
                    logicPosition += delta;
                }

                logicPosition = new Vector3(
                    Mathf.Clamp(logicPosition.x, -halfMapSize.x, halfMapSize.x),
                    Mathf.Clamp(logicPosition.y, -halfMapSize.y, halfMapSize.y),
                    logicPosition.z
                );

                UpdateNavigator();

                LocateEctoplasm();

                yield return null;
            }
        }

        private void LocateEctoplasm()
        {
            Vector3 diff = currentEctoplasm - logicPosition;

            if (diff.magnitude <= absorbDistance)
            {
                SetHint(LightHint.All);
                manager.OnEctoplasmFound();
                currentEctoplasm = manager.CurrentEctoplasm();
                OnPointerUp(null);
                return;
            }
            LightHint newHint = CalculateDirectionHint(diff);

            if (newHint != currentHint)
            {
                SetHint(newHint);
                currentHint = newHint;
            }
        }
        private LightHint CalculateDirectionHint(Vector3 diff)
        {
            //float x = diff.x;
            //float y = diff.y;

            //bool right = x > directionThreshold;
            //bool left = x < -directionThreshold;
            //bool up = y > directionThreshold;
            //bool down = y < -directionThreshold;

            //if (right && up) return LightHint.TopRight;
            //if (left && up) return LightHint.TopLeft;
            //if (right && !up && !down) return LightHint.Right;
            //if (left && !up && !down) return LightHint.Left;
            //if (left && down) return LightHint.BottomLeft;
            //if (right && down) return LightHint.BottomRight;

            //if (up) return y >= 0 ? LightHint.TopRight : LightHint.BottomRight;
            //if (down) return y >= 0 ? LightHint.TopLeft : LightHint.BottomLeft;

            //if (Mathf.Abs(x) > Mathf.Abs(y))
            //    return x >= 0 ? LightHint.Right : LightHint.Left;
            //else
            //    return y >= 0 ? LightHint.TopRight : LightHint.BottomRight;

            float x = diff.x;
            float y = diff.y;

            bool right = x > directionThreshold;
            bool left = x < -directionThreshold;
            bool up = y > directionThreshold;
            bool down = y < -directionThreshold;

            // Diagonals
            if (right && up) return LightHint.TopRight;
            if (left && up) return LightHint.TopLeft;
            if (right && down) return LightHint.BottomRight;
            if (left && down) return LightHint.BottomLeft;

            // Straight directions
            if (right) return LightHint.Right;
            if (left) return LightHint.Left;
            if (up) return LightHint.Top;
            if (down) return LightHint.Bottom;

            // Fallback: closest axis even if within threshold
            if (Mathf.Abs(x) > Mathf.Abs(y))
                return x >= 0 ? LightHint.Right : LightHint.Left;
            else
                return y >= 0 ? LightHint.Top : LightHint.Bottom;
        
        }
        private void SetHint(LightHint hint)
        {
            ResetIndicator();
            switch (hint)
            {
                case LightHint.Left:
                    indicators[1].SetActive(true);
                    indicators[3].SetActive(true);
                    break;
                case LightHint.Right:
                    indicators[0].SetActive(true);
                    indicators[2].SetActive(true);
                    break;
                case LightHint.Top:
                    indicators[2].SetActive(true);
                    indicators[3].SetActive(true);
                    break;
                case LightHint.Bottom:
                    indicators[0].SetActive(true);
                    indicators[1].SetActive(true);
                    break;
                case LightHint.TopLeft:
                    indicators[3].SetActive(true);
                    break;
                case LightHint.TopRight:
                    indicators[2].SetActive(true);
                    break;
                case LightHint.BottomLeft:
                    indicators[1].SetActive(true);
                    break;
                case LightHint.BottomRight:
                    indicators[0].SetActive(true);
                    break;
                case LightHint.All:
                    indicators[0].SetActive(true);
                    indicators[1].SetActive(true);
                    indicators[2].SetActive(true);
                    indicators[3].SetActive(true);
                    break;
            }
        }
        private void ResetIndicator()
        {
            for (int i = 0; i < indicators.Count; i++)
            {
                indicators[i].SetActive(false);
            }
        }
        private void UpdateNavigator()
        {
            Vector2 positionRatio = new Vector2(
                (logicPosition.x + halfMapSize.x) / manager.mapSize.x,
                (logicPosition.y + halfMapSize.y) / manager.mapSize.y
            );

            Vector2 mappedNavigatorPos = (positionRatio * 2f - Vector2.one) * manager.navigatorBorder;
            navigator.transform.localPosition = mappedNavigatorPos;
        }

        private Vector3 GetPointerPosition()
        {
            if (Application.isMobilePlatform)
                return Input.touchCount > 0 ? (Vector3)Input.GetTouch(0).position : Vector3.zero;
            else
                return Input.mousePosition;
        }
    }
}


