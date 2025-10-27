using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.Box_Puzzle
{
    [CreateAssetMenu]
    public class BoxPuzzleData : ScriptableObject
    {
        public List<PlankData> plankData;

        [System.Serializable]
        public class PlankData
        {
            public State correctState;
            public Vector3 pivotLocation;
            public Vector3 startPos;
        }
    }
}
