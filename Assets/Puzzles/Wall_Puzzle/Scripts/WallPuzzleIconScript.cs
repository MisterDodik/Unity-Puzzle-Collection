using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.WallPuzzle
{
    public class WallPuzzleIconScript : MonoBehaviour
    {
        [SerializeField] private int correctIndex;
        public bool CheckIfCorrectPosition(int index)
        {
            if (correctIndex == index)
                return true;
            else
                return false;
        }
    }
}
