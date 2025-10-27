using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;


namespace com.puzzles.LockerPuzzle
{
    public class LockerPuzzleTriangleScript : MonoBehaviour, IEventSystemHandler, IPointerClickHandler
    {
        GameObject pipe;

        LockerPuzzleManager lockerPuzzleManager;
        [SerializeField] GameObject pipeTwo;
        [SerializeField] GameObject pipeThree;
        public GameObject selectTriangle;

        public bool hasBall = false;

        bool isEnd = false;
        [HideInInspector] public bool interactable = false;
        public void GetInitData(LockerPuzzleData.TriangleData triangleData, LockerPuzzleManager manager)
        {
            GetComponent<Collider2D>().enabled = true;
            hasBall = false;

            lockerPuzzleManager = manager;

            for (int i = 1; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);

            if (triangleData.isPipeThree)
                pipe = Instantiate(pipeThree, transform);
            else
                pipe = Instantiate(pipeTwo, transform);

            pipe.transform.localPosition = new Vector2(0, 0);
            pipe.transform.localEulerAngles = new Vector3(0, 0, triangleData.pipeRotation);

            if (triangleData.isStart)
            {
                hasBall = true;
            }

            if (triangleData.isEnd)
                isEnd = true;
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickEvent();
        }
        public void OnClickEvent()
        {         
            if (hasBall)
                return;
            if (!interactable)
                return;
            lockerPuzzleManager.MoveBall(transform);

            hasBall = true;
            interactable = false;
            selectTriangle.SetActive(false);
        }
        public void RotateTriangle()
        {
            Vector3 currentRotation = transform.localEulerAngles;
            if (isEnd)
                currentRotation.z += 0;
            else
                currentRotation.z += 120;

            transform.DORotate(currentRotation, lockerPuzzleManager.moveDuration).OnComplete(() =>
            {
                if (isEnd)
                {
                    lockerPuzzleManager.LoadNextLevel();
                }
                else
                {
                    activateNeighbors();
                }
            });
        }

        private int collidingPipes = 0;

        public void SetInteractable(bool state)
        {
            if(state)
                collidingPipes++;
            interactable = state;
            selectTriangle.SetActive(state);
        }

        public void CheckIfStillColliding()
        {
            collidingPipes--;

            if (collidingPipes <= 0)
            {
                interactable = false;
                selectTriangle.SetActive(false);

                collidingPipes = 0; 
            }
        }

        Collider2D[] neighbors;
        public void FindNeighbors()
        {
            neighbors = Physics2D.OverlapCircleAll(transform.position, 2f); 
        }
        public void resetNeighbors()        
        {
            foreach (Collider2D collider in neighbors)
            {
                if(collider.GetComponent<LockerPuzzleTriangleScript>()!=null || collider.GetComponent<LockerPuzzlePipeScript>() != null)
                    collider.enabled = false;
            }
        }
        public void activateNeighbors()
        {
            foreach (Collider2D collider in neighbors)
            {      
                if(collider.GetComponent<LockerPuzzleTriangleScript>()!=null || collider.GetComponent<LockerPuzzlePipeScript>() != null)
                    collider.enabled = true;
            }
        }       
    }
}
