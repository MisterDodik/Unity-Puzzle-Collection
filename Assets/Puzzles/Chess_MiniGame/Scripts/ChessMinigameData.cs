using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.Chess_MiniGame
{
    [CreateAssetMenu()]
    public class ChessMinigameData : ScriptableObject
    {
        public List<Vector2Int> enemyPieces = new List<Vector2Int>();
        public List<PlayerPieces> playerPieces = new List<PlayerPieces>();

        [System.Serializable]
        public class PlayerPieces
        {
            public ChessPlayerPlacementHandler piece;
            public List<Vector2Int> piecePositions = new List<Vector2Int>();
            public List<Vector2Int> finalPositions = new List<Vector2Int>();
        }
    }
}
