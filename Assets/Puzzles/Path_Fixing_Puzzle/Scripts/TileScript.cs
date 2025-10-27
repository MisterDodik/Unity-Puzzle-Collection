using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.Path_Fixing_Puzzle
{
    public class TileScript : MonoBehaviour, IEventSystemHandler, IPointerClickHandler
    {
        public PathFixingPuzzleManager puzzleManager;

        public TileScript up;
        public TileScript down;
        public TileScript right;
        public TileScript left;

        public bool isLit = false;
        public bool isSource = false;
        public bool isEnd = false;

        public Vector2 pointingTo;

        private TileScript significantNeighbor;

        private GameObject flameGO;

        public Vector2 finalGridPosition;
        public void OnPointerClick(PointerEventData eventData)
        {
            puzzleManager.SwapSelectTiles(transform);
        }
        private void Start()
        {
            flameGO = transform.GetChild(0).gameObject;
        }
        public void InitTile()
        {
            UpdatePointingTile();
            LightNeighbor(new HashSet<TileScript>());
            TriggerFlame();
        }
        private void UpdatePointingTile()
        {
            if (pointingTo == Vector2.up)
                significantNeighbor = up;
            else if (pointingTo == Vector2.down)
                significantNeighbor = down;
            else if (pointingTo == Vector2.left)
                significantNeighbor = left;
            else if (pointingTo == Vector2.right)
                significantNeighbor = right;
        }
        public void LightNeighbor(HashSet<TileScript> visited)
        {
            if (!isLit)  return;
            if (visited.Contains(this)) return;

            visited.Add(this);
            TriggerFlame();
            if (significantNeighbor != null)
            {
                if (-1 * significantNeighbor.pointingTo == pointingTo)
                    return;
                significantNeighbor.isLit = true;
                significantNeighbor.LightNeighbor(visited);
            }

        }


        public void UpdateNeighbors(TileScript _up, TileScript _down, TileScript _left, TileScript _right)
        {
            up = _up;
            down = _down;
            left = _left;
            right = _right;
        }

        public void UpdateConnections()
        {
            if (up != null) up.down = this;
            if (down != null) down.up = this;
            if (left != null) left.right = this;
            if (right != null) right.left = this;
           
            UpdatePointingTile();

            foreach (var neighbor in new[] { up, down, left, right })
            {
                if (neighbor != null)
                    neighbor.UpdatePointingTile();
            }

            isLit = false;
            foreach (var neighbor in new[] { up, down, left, right })
            {
                if (neighbor != null && neighbor != significantNeighbor && neighbor.isLit && neighbor.significantNeighbor == this)
                {
                    isLit = true;
                    break;
                }
            }

            LightNeighbor(new HashSet<TileScript>());
        }


        public void TriggerFlame(bool isSkip = false)
        {
            if (flameGO == null)
                flameGO = transform.GetChild(0).gameObject;

            if (isSkip)
            {
                isLit = true;
                flameGO.SetActive(true);
                return;
            }
            flameGO.SetActive(isLit);

            if (isEnd && pointingTo == Vector2.right && isLit)
            {
                puzzleManager.CheckWin();
            }
        }
    }
}
