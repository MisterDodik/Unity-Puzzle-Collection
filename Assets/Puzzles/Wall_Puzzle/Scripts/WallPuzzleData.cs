using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.WallPuzzle
{
    [CreateAssetMenu]
    public class WallPuzzleData : ScriptableObject
    {
        public List<Vector3> winPositions;
    }
}
