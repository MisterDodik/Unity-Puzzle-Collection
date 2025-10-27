using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.Soul_Boss_Minigame
{
    [CreateAssetMenu()]
    public class MinigameData : ScriptableObject
    {
        public List<PathSegment> paths = new List<PathSegment>();

        [System.Serializable]
        public class PathSegment
        {
            public GameObject segmentGameObject;
            public Vector2 segmentPosition;
            public Vector3 segmentRotation;
            public Vector3 segmentScale = new Vector3(1, 1, 1);
        }


        public List<Vector2> waypointLocations = new List<Vector2>();
    }
}
