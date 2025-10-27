using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.Gate_Barrier_Puzzle
{
    [CreateAssetMenu()]
    public class GateBarrierData : ScriptableObject
    {
        public Vector2 mapSize;            
        public Vector2 navigatorBorder;

        public GameObject checkMarkPrefab;

        public GameObject gaugePrefab;
        public Vector2 gaugeStartPosition;


        public List<GameObject> numbers = new List<GameObject>();
    }
}
