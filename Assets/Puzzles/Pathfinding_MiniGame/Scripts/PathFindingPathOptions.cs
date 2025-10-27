using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.PathfindingMiniGame
{
    public class PathFindingPathOptions : MonoBehaviour, IPointerClickHandler
    {
        public int OptionIndex;

        [SerializeField] PathFindingManager gameManager; 
        public void OnPointerClick(PointerEventData eventData)
        {
            gameManager.OptionsController(OptionIndex);
        }
    }

}
