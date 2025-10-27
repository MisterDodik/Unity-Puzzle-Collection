using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace com.puzzles.Soul_Boss_Minigame
{
    [System.Serializable]
    public class PooledBalls
    {
        public int ballColorIndex;
        public Queue<Ball> pooledBalls;
    }
    public class BallTrainManager : MonoBehaviour
    {
        private List<PooledBalls> ballPools = new List<PooledBalls>()
        {
            new PooledBalls()
            {
                ballColorIndex = 0,
                pooledBalls = new Queue<Ball>()
            },
            new PooledBalls()
            {
                ballColorIndex = 1,
                pooledBalls = new Queue<Ball>()
            },
            new PooledBalls()
            {
                ballColorIndex = 2,
                pooledBalls = new Queue<Ball>()
            }
        };

        [HideInInspector] public List<Ball> balls = new List<Ball>();
        [HideInInspector] public float spacing = 0.5f;

        public List<Ball> ballPrefabs;
        
        [HideInInspector] public int spawnedBalls;

        [HideInInspector] public int initialBallCount;
        [HideInInspector] public Transform ballParent;

        public PathManager pathManager;
        public SoulBossMinigameManager soulBossMinigameManager;

        public void InitializeBallTrainPools(Transform BallParent, int InitialBallCount, bool isReset)
        {
            ballParent = BallParent;
            initialBallCount = InitialBallCount;

            if (isReset)
                ResetBalls();

            balls.Clear();

            for (int i = 0; i < 25; i++)
            {
                if (ballPools[0].pooledBalls.Count < i)
                    break;
                Ball b = GetBall(0);
                spawnedBalls++;
                ballPools[0].pooledBalls.Enqueue(b);
            }
            for (int i = 0; i < 25; i++)
            {
                if (ballPools[1].pooledBalls.Count < i)
                    break;
                Ball b = GetBall(1);
                spawnedBalls++;
                ballPools[1].pooledBalls.Enqueue(b);
            }
            for (int i = 0; i < 25; i++)
            {
                if (ballPools[2].pooledBalls.Count < i)
                    break;
                Ball b = GetBall(2);
                spawnedBalls++;
                ballPools[2].pooledBalls.Enqueue(b);
            }
            spawnedBalls = 0;

            InitializeBalls();
        }
        private Ball GetBall(int colorIndex)
        {
            Ball b;
            if (spawnedBalls < ballPools[colorIndex].pooledBalls.Count)
                b = ballPools[colorIndex].pooledBalls.Dequeue();
            else
            {
                b = Instantiate(ballPrefabs[colorIndex], ballParent).GetComponent<Ball>();
                b.gameObject.SetActive(false);
                b.colorIndex = colorIndex;


            }

            b.trainManager = this;
            b.pathManager = pathManager;
            b.mainManager = soulBossMinigameManager;

            b.ResetState();                   

            return b;
        }

        private int initializedBalls;
        public void InitializeBalls()
        {
            if (initializedBalls >= initialBallCount)
                return;


            int randomIndex = Random.Range(0, ballPools.Count);
            Ball b = GetBall(randomIndex);

            b.gameObject.name = spawnedBalls.ToString();        

            b.index = spawnedBalls;
            balls.Add(b);
            //b.isPaused = false;
            //b.speedUp = false;
            b.spawnNext = true;
            b.AddDelay();
            
            spawnedBalls++;
            initializedBalls++;
        }

        public IEnumerator InsertBallAt(int insertIndex)
        {
            float newBallProgress = balls[insertIndex - 1].realDistance;

            // pausing the back half of the train
            for (int i = insertIndex; i < balls.Count; i++)
                balls[i].isPaused = true;

            // speeding up the front balls to make space
            for (int i = 0; i < insertIndex; i++)
                balls[i].speedUp = true;

            Ball back = insertIndex < balls.Count ? balls[insertIndex] : balls[balls.Count - 1];
            float startingDistance = back.realDistance;
            float requiredDistance = (startingDistance + 2*spacing) % pathManager.totalPathLength;

            while (true)
            {
                bool condition;
                if (balls.Count == 1 && insertIndex == 1)   
                {
                    back.isPaused = false;
                    back.speedUp = true;
                    requiredDistance = (startingDistance + spacing) % pathManager.totalPathLength;
                    condition = back.realDistance >= requiredDistance;
                }
                else
                {
                    float currentGap = pathManager.GetGapBetweenBalls(insertIndex, insertIndex - 1);
                    condition = currentGap >= 2 * spacing - 0.01f;
                }
                if (condition)
                {
                    // snapping all front balls
                    for (int i = insertIndex - 1; i >= 0; i--)
                    {
                        balls[i].realDistance = requiredDistance;
                        balls[i].UpdateRealDistanceSegment();
                        float t = (balls[i].realDistance - balls[i].segmentStart) / balls[i].segmentLength;
                        balls[i].transform.localPosition = Vector2.Lerp(balls[i].posA, balls[i].posB, t);

                        requiredDistance = (requiredDistance + spacing + pathManager.totalPathLength)
                                            % pathManager.totalPathLength;
                    }
                    break;
                }
                yield return null;
            }

            for (int i = 0; i < balls.Count; i++)
            {
                balls[i].isPaused = false;
                balls[i].speedUp = false;
            }

        
            spawnedBalls++;
            Ball b = GetBall(soulBossMinigameManager.currentBall);

            b.colorIndex = soulBossMinigameManager.currentBall;
            b.spawnNext = false;
            b.gameObject.SetActive(true);

            b.realDistance = newBallProgress;
            b.index = insertIndex;
            if (insertIndex >= balls.Count)
                balls.Add(b);
            else
                balls.Insert(insertIndex, b);

            for (int i = insertIndex + 1; i < balls.Count; i++)
            {
                balls[i].index++;
            }

            CheckIfMatch(insertIndex, b);
        }

        public void AddNewBall(int index)
        {
            if (index >= balls.Count && balls.Count > 1)
                index--;
            if (index == 0)
                index = 1;
            StartCoroutine(InsertBallAt(index));      
        }

        private void CheckIfMatch(int newBallIndex, Ball ball)
        {
            int newBallColorIndex = ball.colorIndex;
            List<int> matchedIndexes = new List<int> { newBallIndex };

            for (int i = newBallIndex + 1; i<balls.Count ; i++)
            {
                if (balls[i].colorIndex == newBallColorIndex)
                    matchedIndexes.Add(i);
                else
                    break;
            }
            for (int i = newBallIndex - 1; i > -1; i--)
            {
                if (balls[i].colorIndex == newBallColorIndex)
                    matchedIndexes.Add(i);
                else
                    break;
            }
            if(matchedIndexes.Count > 2)
                RemoveSetOfBalls(matchedIndexes, false);
        }

        private void RemoveSetOfBalls(List<int> toRemove, bool isSkip)
        {
            if (toRemove.Count == 0)
                return;

            toRemove.Sort(); 
            toRemove.Reverse(); 

            int removedCount = 0;
            int lowestIndex = toRemove[toRemove.Count - 1];

            foreach (int index in toRemove)
            {
                if (index < 0 || index >= balls.Count)
                    continue;

                Ball b = balls[index];
                
                b.Explode();
                //b.gameObject.SetActive(false);
                ballPools[b.colorIndex].pooledBalls.Enqueue(b);
                balls.RemoveAt(index);
                removedCount++;
            }

            for (int i = lowestIndex; i < balls.Count; i++)
            {
                balls[i].index -= removedCount;

                if (lowestIndex == 0)
                    continue;
                balls[i].isPaused = true;
            }


            for (int i = 0; i < lowestIndex; i++)
            {
                if (lowestIndex == 0)       //this means that the first ball was removed
                    continue;
                balls[i].StartCatchingUp(toRemove.Count);
            }


            spawnedBalls -= removedCount;
            spawnedBalls = Mathf.Max(spawnedBalls, 0);

            //if (balls.Count <= 0)
            //    soulBossMinigameManager.Completed(); 
            if (balls.Count <= 0)
                soulBossMinigameManager.LoadNextLevel(isSkip);
        }

        public void UnpauseAll()
        {
            for (int i = 0; i < balls.Count; i++)
            {
                balls[i].isPaused = false;
            }
        }
        public void StopBalls()
        {
            for(int i = 0; i < balls.Count; i++)
            {
                balls[i].isPaused = true;
            }
        }


        public void Skip()
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < balls.Count; i++)
            {
                indexes.Add(i);
                balls[i].isPaused = true;
            }
            RemoveSetOfBalls(indexes, true);
        }

        private void ResetBalls()
        {
            for (int i = balls.Count - 1; i >= 0; i--)
            {
                Ball b = balls[i];

                b.realDistance = 0;
                b.gameObject.SetActive(false);
                ballPools[b.colorIndex].pooledBalls.Enqueue(b);
                balls.RemoveAt(i);
            }
            spawnedBalls = 0;
            initializedBalls = 0;
        }
    }
}
