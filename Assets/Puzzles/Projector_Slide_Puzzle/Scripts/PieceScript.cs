using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;
using static com.puzzles.ProjectorSlidePuzzle.ProjectorSlideData;

namespace com.puzzles.ProjectorSlidePuzzle
{
    public class PieceScript : MonoBehaviour, IPointerClickHandler
    {
        public PieceScript leftNeightbor;
        public PieceScript rightNeightbor;

        public int currentIndex;

        public ProjectorSlidePuzzleManager manager;

        public int symbolIndex;

        public WinCombination WinCombination;
        public void InitPiece(int _symbolIndex, ProjectorSlidePuzzleManager _manager, WinCombination _winCombination)
        {
            symbolIndex = _symbolIndex; 
            manager = _manager;
            WinCombination = _winCombination;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Rotate(manager.totalPieces);
        }

        public void Rotate(int totalPieces)
        {
            if (leftNeightbor != null && rightNeightbor != null)
            {
                return;
            }

            Vector3 startRotation = transform.localEulerAngles;

            Vector3 rotationAmount = Vector3.zero;
            
            int sign = 1;
            if (leftNeightbor == null)
            {
                sign = 1;
                rightNeightbor.leftNeightbor = null;
                rightNeightbor = null;
            }
            else if (rightNeightbor == null)
            {
                sign = -1;
                leftNeightbor.rightNeightbor = null;
                leftNeightbor = null;
            }

            rotationAmount.z += 45 * sign;


            //disable mouse events
            manager.DisableMouseEvents();

            transform.DOLocalRotate(startRotation + rotationAmount, manager.moveDuration).OnComplete(() =>
            {
                currentIndex = manager.mod(currentIndex + sign, totalPieces);
                if (sign == 1)
                {
                    leftNeightbor = manager.FindPiece(manager.mod(currentIndex + 1, totalPieces));
                    leftNeightbor.rightNeightbor = this;
                }
                else if (sign == -1)
                {
                    rightNeightbor = manager.FindPiece(manager.mod(currentIndex - 1, totalPieces));
                    rightNeightbor.leftNeightbor = this;
                }

                manager.CheckWin();

                //reenable mouse events
                manager.EnableMouseEvents();
            });
        }
    }
}
