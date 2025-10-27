using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.Book_Puzzle
{
    public class CoinScript : MonoBehaviour, IPointerClickHandler
    {
        private BookPuzzleManager manager;

        [HideInInspector] public bool isCorrect = false;

        public int coinIndex;

        public int targetIndex;

        public int startIndex;
        public Vector2 startPosition;
        public void InitCoin(BookPuzzleManager _manager, int _index, int _targetIndex, Vector3 _startPosition)
        {
            isCorrect = false;

            manager = _manager;
            startIndex = _index;
            targetIndex = _targetIndex;

            startPosition = _startPosition;
            coinIndex = startIndex;
            transform.localPosition = startPosition;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            manager.MoveCoin(this);
        }

        public void CheckPosition()
        {
            bool wasCorrect = isCorrect == true;
            if (coinIndex == targetIndex)
            {
                isCorrect = true;
            }
            else
            {
                isCorrect = false;
            }         
            manager.TriggerGlow(targetIndex, isCorrect, wasCorrect);
        }
    }
}
