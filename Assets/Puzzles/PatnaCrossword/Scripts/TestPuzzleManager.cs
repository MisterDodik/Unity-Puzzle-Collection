using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.frameworks.PatnaCrossword
{
    public class TestPuzzleManager : BlockPuzzleFrameworkManager 
    {
        private void Start()
        {
            InitializePuzzle();
        }

        public void ResetGame()
        {
            ResetPuzzle();
        }
        public void Skip()
        {
            SkipPuzzle();
        }

        public void Completed()
        {
            print("win");
        }

        public override void PuzzleCompleted()
        {
            Completed();
        }

    }

}
