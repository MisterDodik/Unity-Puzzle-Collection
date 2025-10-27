using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.Painting_Puzzle
{
    public class PieceScript : MonoBehaviour, IPointerClickHandler
    {
        private PaintingPuzzleManager manager;
        public int currentIndex;
        public int targetIndex;

        public bool isCorrect = false;

        private bool wasCorrect = false;
        public void InitPiece(PaintingPuzzleManager _manager, int _startingIndex, int _targetIndex)
        {
            manager = _manager;
            currentIndex = _startingIndex;
            targetIndex = _targetIndex;

            isCorrect = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isCorrect)
                return;
            manager.SwapSelectPieces(transform);
        }

        public void OnSwapEvents(int newIndex)
        {
            if (newIndex == currentIndex) return;

            int previousIndex = currentIndex;   
            currentIndex = newIndex;        

            CheckPosition(previousIndex);
        }

        private void CheckPosition(int previousIndex)
        {
            bool nowCorrect = (targetIndex == currentIndex);

            if (nowCorrect && !wasCorrect)
            {
                manager.CheckWin(Mathf.FloorToInt(mod((currentIndex - 2), 16) / 4), false);
                wasCorrect = true;
            }
            else if (!nowCorrect && wasCorrect)
            {
                manager.CheckWin(Mathf.FloorToInt(mod((previousIndex - 2), 16) / 4), true); 
                wasCorrect = false;
            }
        }

        private int mod(int x, int y)
        {
            int result = x % y;
            return result < 0 ? result + y : result;
        }
     
     

    }
}
