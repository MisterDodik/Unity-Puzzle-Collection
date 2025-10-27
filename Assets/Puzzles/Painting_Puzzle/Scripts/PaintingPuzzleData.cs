using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.Painting_Puzzle
{
    [CreateAssetMenu()]
    public class PaintingPuzzleData : ScriptableObject
    {

        public List<PieceData> piecesData = new List<PieceData>();
        public List<Vector2> piecePositions = new List<Vector2>();

        [System.Serializable]
        public class PieceData
        {
            public int startingIndex;
            public int targetIndex;
        }
    }
}
