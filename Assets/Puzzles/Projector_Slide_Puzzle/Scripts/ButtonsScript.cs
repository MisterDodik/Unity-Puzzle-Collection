using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace com.puzzles.ProjectorSlidePuzzle
{
    public class ButtonsScript : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent action;

        private SpriteRenderer spriteRenderer;
        public Sprite defaultSprite;
        public Sprite clickedSprite;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            ChangeSprite(false);
            action.Invoke();
        }

        public void ChangeSprite(bool isActive)
        {
            if (isActive)
                spriteRenderer.sprite = defaultSprite;
            else
                spriteRenderer.sprite = clickedSprite;
        }
    }
}
