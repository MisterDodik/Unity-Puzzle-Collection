using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static com.puzzles.Soul_Boss_Minigame.MinigameData;

namespace com.puzzles.Soul_Boss_Minigame
{
    public class PathManager : MonoBehaviour
    {
        [HideInInspector] public List<Transform> waypoints = new List<Transform>();

        public float totalPathLength;

        [HideInInspector] public List<float> cumulativeDistances; // Store cumulative distances for each segment

        public BallTrainManager ballTrainManager;
        public Transform puzzleBg;
        private Transform pathParent;
        public void InitializePath(Transform PathParent, List<PathSegment> path)
        {
            if (totalPathLength > 1)
                return;

            pathParent = PathParent;
            waypoints.Clear();
            foreach (Transform waypoint in pathParent)
            {
                waypoints.Add(waypoint);
            }
            CalculateCumulativeDistances();
        }
        private void CalculateCumulativeDistances()
        {
            cumulativeDistances = new List<float>();
            float totalDistance = 0f;

            // Iterate through each segment
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                float segmentDistance = Vector3.Distance(waypoints[i].localPosition, waypoints[i + 1].localPosition);
                totalDistance += segmentDistance;

                cumulativeDistances.Add(totalDistance);
            }
            totalPathLength = cumulativeDistances[cumulativeDistances.Count - 1];
        }

        public float GetGapBetweenBalls(int ballIndex1, int ballIndex2)
        {
            float realDistance1 = (ballTrainManager.balls[ballIndex1].realDistance);
            float realDistance2 = (ballTrainManager.balls[ballIndex2].realDistance);

            return Mathf.Abs(realDistance1 - realDistance2); 
        }

        public float GetCumulativeDistance(int index)
        {
            if (index <= 0) return 0f;
            return cumulativeDistances[Mathf.Clamp(index - 1, 0, cumulativeDistances.Count - 1)];
        }

        public float GetSegmentLength(int index)
        {
            if (index < 0) return 0f;
            if (index >= waypoints.Count)
                index = index % waypoints.Count;
            return Vector2.Distance(waypoints[index].localPosition, waypoints[index + 1].localPosition);
        }

        public int GetSegmentIndexFromDistance(float distance)
        {
            for (int i = 0; i < cumulativeDistances.Count; i++)
            {
                if (distance < cumulativeDistances[i])
                    return i;
            }
            return cumulativeDistances.Count - 1;
        }
   
    }
}
