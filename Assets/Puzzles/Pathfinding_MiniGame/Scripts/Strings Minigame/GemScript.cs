using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.PathfindingMiniGame
{
    public class GemScript : MonoBehaviour,  IPointerUpHandler, IDragHandler, IPointerDownHandler
    {
        public List<LineRenderer> lines;
        public List<int> indices;
        public IndividualStringLevelManager lineManager;

        public void InitGem(int index, LineRenderer line, IndividualStringLevelManager levelManager)
        {
            lines.Add(line);
            indices.Add(index);
            
            if(lineManager == null)
                lineManager = levelManager;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }


        public void OnDrag(PointerEventData eventData)
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(eventData.position);
            position.z = 0;
            transform.position = position;

            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].SetPosition(indices[i], transform.position);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            lineManager.CheckIntersections();
        }
    }

}
