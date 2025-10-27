using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.PathfindingMiniGame
{
    [CreateAssetMenu(fileName = "PathFindingMiniGameData")]
    public class PathFindingMiniGameData : ScriptableObject
    {

        public List<StringPuzzle> stringPuzzles;

        [System.Serializable]
        public class StringPuzzle
        {
            public List<Vector2Int> edges;        //connected pairs
            public Vector3[] positions;
            public bool[] isStationary;
        }
    }
}
