using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.puzzles.WallPuzzle
{
    public class WallPuzzleManager : MonoBehaviour
    {
        [SerializeField] private WallPuzzleData gameData;

        [SerializeField] private Transform innerCircle;
        [SerializeField] private Transform iconHolderLeft;
        [SerializeField] private Transform iconHolderRight;

        [SerializeField] private float minRotation = -45;
        [SerializeField] private float maxRotation = 45;
        [SerializeField] private float rotationDuration = 0.5f;

        [SerializeField] private Transform iconParent;
        private List<Transform> icons = new List<Transform>();
        private List<Transform> initialIcons = new List<Transform>();
        private List<int> iconStates = new List<int>();         
        private List<Vector3> iconInitialPositions = new List<Vector3>();

        [SerializeField] private float pickupDuration = 0.5f;

        private int occupyingIndex1;
        private int occupyingIndex2;


        private void Start()
        {
            InitPuzzle();
        }

        public void ResetGame()
        {
            if (iconHolderLeft.childCount > 0)
                iconHolderLeft.GetChild(0).SetParent(iconParent);
            if (iconHolderRight.childCount > 0)
                iconHolderRight.GetChild(0).SetParent(iconParent);
            InitPuzzle();
        }
        public void Skip()
        {
            if (iconHolderLeft.childCount > 0)
                iconHolderLeft.GetChild(0).SetParent(iconParent);
            if (iconHolderRight.childCount > 0)
                iconHolderRight.GetChild(0).SetParent(iconParent);

            for (int i = 0; i < iconParent.childCount; i++)
            {
                Transform child = iconParent.GetChild(i);
                child.localPosition = gameData.winPositions[i];
                
                GameObject glow = child.GetChild(0).gameObject;
                glow.SetActive(true);
            }

            Completed();
        }

        public void Completed()
        {
            print("Win");
            EndgamePanel();
        }

        private void InitPuzzle()
        {
            occupyingIndex1 = 2;
            occupyingIndex2 = 3;


            if (icons.Count == iconParent.childCount)
            {
                innerCircle.localEulerAngles = Vector3.zero;

                icons.Clear();
                for (int i = 0; i < initialIcons.Count; i++)
                {
                    initialIcons[i].localPosition = iconInitialPositions[i];
                    icons.Add(initialIcons[i]);
                }
                for (int i = 0; i < iconStates.Count; i++)
                    iconStates[i] = 1;
                CheckWin();
                return;
            }

            foreach(Transform item in iconParent)
            {
                initialIcons.Add(item);
                icons.Add(item);
                iconInitialPositions.Add(item.localPosition);
                iconStates.Add(1);
            }
        }


        public void RotateMiddleSegment(int index, GameObject button)
        {
            float currentRotation = NormalizeAngle(innerCircle.localEulerAngles.z);
            if (index == -1)
            {
                if (currentRotation - 22.5f < minRotation)
                    return;

                //Disable mouse events
                eventSystem.SetActive(false);
                button.SetActive(false);

                Vector3 newAngle = new Vector3(0, 0, currentRotation - 22.5f);
                innerCircle.DOLocalRotate(newAngle, rotationDuration).OnComplete(() =>
                {
                    innerCircle.localEulerAngles = newAngle;
                    //Enable mouse events
                    eventSystem.SetActive(true);
                    button.SetActive(true);
                });

                occupyingIndex1++;
                occupyingIndex2++;

            }
            else if (index == 1) 
            {
                if (currentRotation > maxRotation)
                    return;

                //Disable mouse events
                eventSystem.SetActive(false);

                button.SetActive(false);

                Vector3 newAngle = new Vector3(0, 0, currentRotation + 22.5f);
                innerCircle.DOLocalRotate(newAngle, rotationDuration).OnComplete(() =>
                {
                    innerCircle.localEulerAngles = newAngle;
                    //Enable mouse events
                    eventSystem.SetActive(true);

                    button.SetActive(true);
                });

                occupyingIndex1--;
                occupyingIndex2--;
            }
        }
        private float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f)
                angle -= 360f;
            return angle;
        }


        public void PickupIcons(GameObject button)
        {
            StartCoroutine(OnMiddleButtonClickEvent(button));

            bool leftEmpty = iconStates[occupyingIndex1] == 0;
            bool rightEmpty = iconStates[occupyingIndex2] == 0;

            Transform leftSegmentChild = iconHolderLeft.childCount > 0 ? iconHolderLeft.GetChild(0) : null;
            Transform rightSegmentChild = iconHolderRight.childCount > 0 ? iconHolderRight.GetChild(0) : null;

            Transform leftIcon = icons[occupyingIndex1];
            Transform rightIcon = icons[occupyingIndex2];


            // === SINGLE SIDE ===
            if (leftEmpty && !rightEmpty)
            {
                if (leftSegmentChild != null)
                {
                    leftSegmentChild.SetParent(iconParent);
                    leftSegmentChild.DOLocalMove(iconInitialPositions[occupyingIndex1], pickupDuration);
                    SwapListObjects(occupyingIndex1, leftSegmentChild);
                    iconStates[occupyingIndex1] = 1;
                }
                return;
            }

            if (rightEmpty && !leftEmpty)
            {
                if (rightSegmentChild != null)
                {
                    rightSegmentChild.SetParent(iconParent);
                    rightSegmentChild.DOLocalMove(iconInitialPositions[occupyingIndex2], pickupDuration);
                    SwapListObjects(occupyingIndex2, rightSegmentChild);
                    iconStates[occupyingIndex2] = 1;
                }
                return;
            }

            // LEFT SIDE
            if (leftEmpty)
            {
                if (leftSegmentChild != null)
                {
                    leftSegmentChild.SetParent(iconParent);
                    leftSegmentChild.DOLocalMove(iconInitialPositions[occupyingIndex1], pickupDuration);
                    SwapListObjects(occupyingIndex1, leftSegmentChild);
                    iconStates[occupyingIndex1] = 1;
                }
            }
            else
            {
                iconStates[occupyingIndex1] = 0;
                if (leftSegmentChild != null)
                {
                    leftSegmentChild.SetParent(iconParent);
                    leftSegmentChild.DOLocalMove(iconInitialPositions[occupyingIndex1], pickupDuration);
                    SwapListObjects(occupyingIndex1, leftSegmentChild);
                    iconStates[occupyingIndex1] = 1;
                }

                leftIcon.SetParent(iconHolderLeft);
                leftIcon.DOLocalMove(Vector2.zero, pickupDuration);
            }

            // RIGHT SIDE
            if (rightEmpty)
            {
                if (rightSegmentChild != null)
                {
                    rightSegmentChild.SetParent(iconParent);
                    rightSegmentChild.DOLocalMove(iconInitialPositions[occupyingIndex2], pickupDuration);
                    SwapListObjects(occupyingIndex2, rightSegmentChild);
                    iconStates[occupyingIndex2] = 1;
                }
            }
            else
            {
                iconStates[occupyingIndex2] = 0;
                if (rightSegmentChild != null)
                {
                    rightSegmentChild.SetParent(iconParent);
                    rightSegmentChild.DOLocalMove(iconInitialPositions[occupyingIndex2], pickupDuration);
                    SwapListObjects(occupyingIndex2, rightSegmentChild);
                    iconStates[occupyingIndex2] = 1;
                }

                rightIcon.SetParent(iconHolderRight);
                rightIcon.DOLocalMove(Vector2.zero, pickupDuration);
            }
        }
        IEnumerator OnMiddleButtonClickEvent(GameObject button)
        {
            button.SetActive(false);
            yield return new WaitForSeconds(pickupDuration);
            button.SetActive(true);


            CheckWin();
        }
        private void CheckWin()
        {
            int currentProgress = 0;
            for (int i = 0; i < icons.Count; i++)
            {
                Transform icon = icons[i];
                GameObject glow = icon.GetChild(0).gameObject;
                if (icon.IsChildOf(iconHolderLeft) || icon.IsChildOf(iconHolderRight))
                {
                    glow.SetActive(false);
                    continue;
                }
                if (icon.GetComponent<WallPuzzleIconScript>().CheckIfCorrectPosition(i))
                {
                    glow.SetActive(true);
                    currentProgress++;
                }
                else
                    glow.SetActive(false);

            }

            if (currentProgress >= icons.Count)
            {
                Completed();
            }
        }
        void SwapListObjects(int index1, Transform desiredObject)
        {
            Transform temp = icons[index1];
            icons[icons.IndexOf(desiredObject)] = temp;
            icons[index1] = desiredObject;
        }











        //------test functions-----
        public GameObject eventSystem;
        public GameObject endPanel;
        private void EndgamePanel()
        {
            endPanel.SetActive(true);
        }
        public void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
