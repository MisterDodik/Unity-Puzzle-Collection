using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.LockerPuzzle
{
    public class LockerPuzzlePipeScript : MonoBehaviour
    {
        private LockerPuzzleTriangleScript parentTriangle;

        public bool increased = false;

        private void Start()
        {
            parentTriangle = GetComponentInParent<LockerPuzzleTriangleScript>();
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            LockerPuzzlePipeScript otherPipe = other.GetComponent<LockerPuzzlePipeScript>();
            if (otherPipe == null)
                return;

            LockerPuzzleTriangleScript otherTriangle = otherPipe.GetComponentInParent<LockerPuzzleTriangleScript>();
            if (otherTriangle == null)
                return;

            if (otherTriangle.hasBall)
            {
                if (!increased) // Only set interactable once
                {
                    parentTriangle.SetInteractable(true);
                    increased = true;
                }
            }
        }

      

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.GetComponent<LockerPuzzlePipeScript>() != null)
            {
                parentTriangle.CheckIfStillColliding();
                increased = false;

            }
        }
    }
}
