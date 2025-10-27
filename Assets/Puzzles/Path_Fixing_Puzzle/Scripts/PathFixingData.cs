using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.Path_Fixing_Puzzle
{
    [System.Serializable]
    [CreateAssetMenu()]
    public class PathFixingData : ScriptableObject
    {
        public Vector2 gridOrigin;
        public Vector2 gridSize;
        public Vector2 tileOffset;
        public Vector2 emptyCell;

        public List<TileData> tileData = new List<TileData>();
        public List<TileSolution> solutions = new List<TileSolution>();

        [System.Serializable]
        public class TileData
        {
            public int amountToSpawn;
            public GameObject tilePrefab;
            public Vector2 pointingTo;
        }

        [System.Serializable]
        public class TileSolution
        {
            public Vector2 finalGridPosition;
        }
    }
}
