using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.PathfindingMiniGame
{
    public class PathFindingManager : MonoBehaviour
    {
        private Camera mainCamera;
        float cameraSize;
        public float transitionDuration = 1f;

        [SerializeField] private GameObject currentPart;
        private int currentPartIndex = 0;
        [SerializeField] private List<GameObject> parts;

        private void Start()
        {
            mainCamera = Camera.main;
            cameraSize = mainCamera.orthographicSize;
            currentPart = parts[currentPartIndex];
        }

        public void Completed()
        {
            currentPart.SetActive(false);
            print("Win");
        }

        public void OptionsController(int index)
        {        
            if(index == 1)  //path B
            {
                parts.Add(parts[1]);
                parts.RemoveAt(1);
            }

            LoadNextPart();
        }
        
        public void LoadNextPart()
        {
            if (currentPartIndex >= parts.Count - 1)
            {
                Completed();
                return;
            }

            StartCoroutine(LoadNextPartCoroutine());
            currentPartIndex++;
        }

        IEnumerator LoadNextPartCoroutine()
        {
            //disable mouse events

            float elapsedTime = 0f;
            float targetSize = cameraSize - 0.4f;

            while (elapsedTime < transitionDuration / 2)
            {
                elapsedTime += Time.deltaTime;
                mainCamera.orthographicSize = Mathf.Lerp(cameraSize, targetSize, elapsedTime / transitionDuration);
                yield return null;
            }

            if (currentPart)
                currentPart.SetActive(false);

            currentPart = parts[currentPartIndex];
            currentPart.SetActive(true);

            mainCamera.orthographicSize = cameraSize;
            //enable mouse events

        }
    }
}
