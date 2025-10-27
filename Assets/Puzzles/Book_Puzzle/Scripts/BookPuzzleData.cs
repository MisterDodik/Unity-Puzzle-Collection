using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.Book_Puzzle
{
    [CreateAssetMenu()]
    public class BookPuzzleData : ScriptableObject
    {
        public List<CoinData> coinData;
        public List<GlowIndicators> glowIndicatorData;

        [System.Serializable]
        public class CoinData
        {
            public GameObject coinPrefab;
            public Vector2 coinPosition;
            public int targetIndex;
        }

        [System.Serializable]
        public class GlowIndicators
        {
            public GameObject glowIndicatorPrefab;
            public Vector2 glowPosition;
        }
    }
}
