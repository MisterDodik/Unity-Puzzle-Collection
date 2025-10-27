using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace com.puzzles.Soul_Boss_Minigame
{
    public class Ball : MonoBehaviour, IPointerClickHandler
    {
        public float baseSpeed = 1f;
        public int index;

        public bool isPaused = false;
        public bool speedUp = false;


        public int colorIndex;

        public PathManager pathManager;
        public BallTrainManager trainManager;
        public SoulBossMinigameManager mainManager;
        [HideInInspector]public Collider2D coll;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }
        private void Start()
        {
            segmentIndex = pathManager.GetSegmentIndexFromDistance(0);
            segmentStart = pathManager.GetCumulativeDistance(segmentIndex);
            segmentLength = pathManager.GetSegmentLength(segmentIndex);
            posA = pathManager.waypoints[segmentIndex].localPosition;
            posB = pathManager.waypoints[segmentIndex + 1].localPosition;
            transform.localPosition = posA;

            coll = GetComponent<Collider2D>();
            coll.enabled = false;       //mzd ce biti problem ako se reaktivira ovaj game object al aj
        }


        public bool spawnNext;
        public float realDistance;

        private void FixedUpdate()
        {
            if (isPaused)
                return;

            if (speedUp)
                realDistance += baseSpeed * 10f * Time.fixedDeltaTime;
            else if (isReversing)
                realDistance += -baseSpeed * 10f * Time.fixedDeltaTime;
            else
                realDistance += baseSpeed * Time.fixedDeltaTime;
           
            if(realDistance > trainManager.spacing * 3 && coll.enabled == false)
                coll.enabled = true;

            if (isReversing)
            {         
                if (currentDistance - targetDistance > realDistance)
                {
                    isReversing = false;

                    speedUp = false;
                    trainManager.UnpauseAll();
                    realDistance = currentDistance - targetDistance;
                    targetDistance = 0;
                }
            }



            if (spawnNext)
            {
                if (realDistance >= trainManager.spacing)
                {
                    trainManager.InitializeBalls();
                    spawnNext = false;
                }
            }


            if (realDistance >= pathManager.totalPathLength)       //loop, add lose animation here
                mainManager.GameOver();       

            UpdateRealDistanceSegment();
            float t = (realDistance - segmentStart) / segmentLength;
            transform.localPosition = Vector2.Lerp(posA, posB, t);

        }
        [HideInInspector] public int segmentIndex;
        [HideInInspector] public float segmentStart;
        [HideInInspector] public float segmentLength;
        [HideInInspector] public Vector2 posA;
        [HideInInspector] public Vector2 posB;

        public void UpdateRealDistanceSegment()
        {
            int newSegmentIndex = pathManager.GetSegmentIndexFromDistance(realDistance);
            if(newSegmentIndex != segmentIndex)
            {
                segmentIndex = newSegmentIndex;
                segmentStart = pathManager.GetCumulativeDistance(segmentIndex);
                segmentLength = pathManager.GetSegmentLength(segmentIndex);
                posA = pathManager.waypoints[segmentIndex].localPosition;
                posB = pathManager.waypoints[segmentIndex + 1].localPosition;
            }
        }

        public bool isReversing = false;
        private float targetDistance = 0;
        private float currentDistance = 0;
        public void StartCatchingUp(int numberOfRemovedBalls)
        {
            currentDistance = realDistance;
            targetDistance = numberOfRemovedBalls * trainManager.spacing;
            isReversing = true;
        }

        public void ResetState()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;

                if (i == 0)
                    child.SetActive(true);
                else
                    child.SetActive(false);
            }
        }
        public void Explode()
        {
            StartCoroutine(explosionAnimation());
        }

        public void AddDelay()
        {
            gameObject.SetActive(true);
            StartCoroutine(delayedActivation());
        }
        IEnumerator delayedActivation()
        {
            spriteRenderer.enabled = false;
            yield return null;
            spriteRenderer.enabled = true;
        }
        IEnumerator explosionAnimation()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;

                if (i == 0)
                    child.SetActive(false);
                else
                {
                    child.SetActive(true);
                    yield return new WaitForSeconds(.1f);
                }

            }

            segmentIndex = pathManager.GetSegmentIndexFromDistance(0);
            segmentStart = pathManager.GetCumulativeDistance(segmentIndex);
            segmentLength = pathManager.GetSegmentLength(segmentIndex);
            posA = pathManager.waypoints[segmentIndex].localPosition;
            posB = pathManager.waypoints[segmentIndex + 1].localPosition;
            transform.localPosition = posA;
            spawnNext = false;
            speedUp = false;
            isPaused = false;
            index = 0;
            realDistance = 0;
            gameObject.SetActive(false);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Vector3 mousePos = eventData.position;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            mainManager.FireAndRotate(mousePos);
        }
    }
}
