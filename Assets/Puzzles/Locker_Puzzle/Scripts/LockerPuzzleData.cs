using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.LockerPuzzle
{
    [Serializable]
    [CreateAssetMenu(fileName = "LockerPuzzleData")]
    public class LockerPuzzleData : ScriptableObject
    {
        public List<Level> levels;
        public List<Solution> solutions;

        [System.Serializable]
        public class Level
        {
            public Vector2 gridPos;
            public Vector2 gridScale;
            public List<TriangleData> level;
        }
        [System.Serializable]
        public class Solution
        {
            public Vector2[] path;
        }

        [System.Serializable]
        public class TriangleData
        {
            public bool IsEmpty;
            public Vector2 position;        //in grid
            public float pipeRotation;
            public bool isPipeThree;
            public bool isStart;
            public bool isEnd;
        }
    }

}