using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.Card_MiniGame
{
    [CreateAssetMenu()]
    public class GameData : ScriptableObject
    {
        public Vector2 upperSectionOrigin;
        public Vector2 centralSectionOrigin;
        public Vector2 lowerSectionOrigin;

        public Vector2 cardSpacing;

        public List<CardData> cardData = new List<CardData>();

        [System.Serializable]
        public class CardData
        {
            public int cardHp;
            public int AD;
            public bool hasSpecialEffect;
        }
    }
}
