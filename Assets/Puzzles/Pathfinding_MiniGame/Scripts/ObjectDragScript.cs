using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.PathfindingMiniGame
{
    public class ObjectDragScript : MonoBehaviour, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(eventData.position);
            position.z = 0;

            position.x = Mathf.Clamp(position.x, -5f, 5f);
            position.y = Mathf.Clamp(position.y, -4f, 4f);

            transform.position = position;
        }
    }
}
