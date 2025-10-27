using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.Soul_Boss_Minigame
{
    public class CharacterScript : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector] public SoulBossMinigameManager manager;
        public void OnPointerClick(PointerEventData eventData)
        {
            manager.ChangeBall();
        }
    }
}
