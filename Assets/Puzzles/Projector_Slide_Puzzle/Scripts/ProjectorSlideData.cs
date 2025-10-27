using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.ProjectorSlidePuzzle
{
    [CreateAssetMenu]
    public class ProjectorSlideData : ScriptableObject
    {

        public List<SymbolData> symbols;

        [System.Serializable]
        public class SymbolData
        {
            public GameObject symbolGameObject;
            public int pieceIndex;
            public int symbolIndex;     //symbol name -1

            public WinCombination WinCombination;
            public int correctPositionIndex;
        }

        [System.Serializable]
        public class WinCombination
        {
            public int leftNeighborSymbolIndex;
            public int rightNeighborSymbolIndex;
        }
    }
}
