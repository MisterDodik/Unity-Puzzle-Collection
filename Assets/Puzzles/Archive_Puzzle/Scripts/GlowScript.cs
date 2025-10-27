using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace com.puzzles.ArchivePuzzle{
    public class GlowScript : MonoBehaviour, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        SpriteMask spriteMask;
        SortingGroup sortingGroup;

        private ImagePieceScript childPiece;
        private Vector3 offset;




        Vector3[] boundsCoord = new Vector3[2];
        Vector3[] childBoundsCoord = new Vector3[2];

        private void Start()
        {
            if (!TryGetComponent<SortingGroup>(out sortingGroup))
                sortingGroup = gameObject.AddComponent<SortingGroup>();

            if (!TryGetComponent<SpriteMask>(out spriteMask))
                spriteMask = gameObject.AddComponent<SpriteMask>();

            spriteMask.sprite = GetComponent<SpriteRenderer>().sprite;
            sortingGroup.sortingOrder = 1;
        }
        public void LoadChild(ImagePieceScript child)
        {
            childPiece = child;

            LoadBounds();
        }
        private void LoadBounds()
        {
            Bounds bounds = GetComponent<SpriteRenderer>().sprite.bounds;
            boundsCoord[0] = transform.localPosition + bounds.center + bounds.extents;
            boundsCoord[1] = transform.localPosition + bounds.center - bounds.extents;

            Bounds childBounds = childPiece.GetComponent<SpriteRenderer>().sprite.bounds;
            childBoundsCoord[0] = childBounds.center + childBounds.extents;
            childBoundsCoord[1] = childBounds.center - childBounds.extents;
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (childPiece != null && !childPiece.isCorrect)
            {
                GetComponent<SpriteRenderer>().enabled = true;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (childPiece != null && !childPiece.isCorrect)
            {
                GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (childPiece == null || childPiece.isCorrect)
                return;

            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePos.z = 10;
            offset = childPiece.transform.position - worldMousePos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (childPiece == null || childPiece.isCorrect)
                return;

            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePos.z = 10;

            Vector2 newPos = worldMousePos + offset;
            if (childBoundsCoord[0].y + newPos.y < boundsCoord[0].y)
                newPos.y = childPiece.transform.position.y;
            if (childBoundsCoord[0].x + newPos.x < boundsCoord[0].x)
                newPos.x = childPiece.transform.position.x;
            if (childBoundsCoord[1].y + newPos.y > boundsCoord[1].y)
                newPos.y = childPiece.transform.position.y;
            if (childBoundsCoord[1].x + newPos.x > boundsCoord[1].x)
                newPos.x = childPiece.transform.position.x;


            childPiece.transform.position = newPos;

            if (Vector2.Distance(childPiece.transform.localPosition, childPiece.winPos) < 0.3f)
            {
                childPiece.CloseEnough();
                childPiece.puzzleManager.CheckWin();
            }
        }
    }
}
