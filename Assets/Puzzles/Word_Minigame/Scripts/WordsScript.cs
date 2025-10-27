using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.Word_Minigame
{
    public class WordsScript : MonoBehaviour, IPointerClickHandler
    {
        public string word;
        public bool isCorrect = false;

        SpriteRenderer spriteRenderer;

        public WordMinigameManager manager;

        public int levelIndex = 0;
        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (levelIndex == 1)
            {
                StartFloating(); 
            }
            else if (levelIndex == 2)
            {
                StartFloating(); 
                StartBlinking(); 
            }
        }

        private void StartFloating()
        {
            float floatAmount = Random.Range(0.1f, 0.25f);
            float duration = Random.Range(2f, 4f);

            Vector3 originalPos = transform.localPosition;

            transform.DOLocalMoveY(originalPos.y + floatAmount, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        private void StartBlinking()
        {
            Sequence blinkSequence = DOTween.Sequence();
            blinkSequence.Append(spriteRenderer.DOFade(1f, 0.1f))
                         .AppendInterval(1f)
                         .Append(spriteRenderer.DOFade(0f, 0.1f))
                         .AppendInterval(1f)
                         .SetLoops(-1);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (spriteRenderer.enabled && spriteRenderer.color.a > 0.5f)
            {
                manager.HighlightWord(spriteRenderer, isCorrect, word);
            }
        }
    }
}
