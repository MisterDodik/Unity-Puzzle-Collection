using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.Soul_Boss_Minigame
{
    public class SinglePathScript : MonoBehaviour, IPointerClickHandler
    {
        public SoulBossMinigameManager manager;
        public void OnPointerClick(PointerEventData eventData)
        {

            Vector3 mousePos = eventData.position;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            manager.FireAndRotate(mousePos);
        }
    }
}
