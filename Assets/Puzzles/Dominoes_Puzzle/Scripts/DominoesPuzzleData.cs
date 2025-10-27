using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.Dominoes_Puzzle
{
    [CreateAssetMenu()]
    public class DominoesPuzzleData : ScriptableObject
    {
        public List<TriangleData> triangleData = new();

        [System.Serializable]   
        public class TriangleData
        {
            public bool isActive = false;
            public Vector2 endPos;
            public List<int> triangleIndexesToActivate = new();
        }
    }
}
