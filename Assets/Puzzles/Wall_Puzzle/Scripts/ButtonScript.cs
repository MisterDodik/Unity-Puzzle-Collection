using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.WallPuzzle
{
    public class ButtonScript : MonoBehaviour, IPointerClickHandler
    {
        public WallPuzzleManager puzzleManager;
        [SerializeField] int rotationIndex;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (rotationIndex != 0)
                puzzleManager.RotateMiddleSegment(rotationIndex, transform.gameObject);
            else
                puzzleManager.PickupIcons(transform.gameObject);
        }
    }
}
