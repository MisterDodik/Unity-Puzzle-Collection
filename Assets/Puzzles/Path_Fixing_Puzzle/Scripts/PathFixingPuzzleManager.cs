using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.puzzles.Path_Fixing_Puzzle
{
    public class PathFixingPuzzleManager : MonoBehaviour
    {
        [SerializeField] private PathFixingData gameData;
        [SerializeField] private Transform tileParent;
        [SerializeField] private GameObject endFlame;

        [SerializeField] private float swapDuration = 0.5f;

        [HideInInspector] public Transform selectedTile;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color selectedColor;


        private List<TileScript> spawnedTiles = new();
        private List<(Vector2Int gridCoord, Vector2 worldPos)> coordMap = new();
        private Dictionary<Vector2Int, TileScript> gridCoordToTile = new();

        private bool gameOver = false;
        private void Start()
        {
            InitGame();
        }
        public void ResetGame()
        {
            InitGame();
        }
        public void Skip()
        {
            //Disable mouse events
            eventHandler.SetActive(false);

            Sequence sequence = DOTween.Sequence();

            foreach (TileScript item in spawnedTiles)
            {
                Vector2 finalPos = gameData.gridOrigin + gameData.tileOffset * item.finalGridPosition;
                sequence.Join(item.transform.DOLocalMove(finalPos, swapDuration));
                item.TriggerFlame(true);
            }
            sequence.Play().OnComplete(() =>
            {
                //Enable mouse events
                eventHandler.SetActive(true);

                Completed();
            });
        }

        public void Completed()
        {
            print("Win");
            gameOver = true;
            endFlame.SetActive(true);

            EndGamePanel();
        }

        private void InitGame()
        {
            gameOver = false;

            if (coordMap.Count == 0)
            {
                for (int i = 0; i<gameData.gridSize.x; i++)
                {
                    for (int j = 0; j<gameData.gridSize.y; j++)
                    {
                        if (i == gameData.emptyCell.y && j == gameData.emptyCell.x)
                            continue;
                        Vector2 newPos = gameData.gridOrigin + new Vector2(gameData.tileOffset.x * j, gameData.tileOffset.y * i);
                        
                        coordMap.Add((new Vector2Int(j, i), newPos));
                    }
                }

                shuffleList(coordMap);
            }

            int tileIndex = 0;
            foreach (PathFixingData.TileData tileData in gameData.tileData)
            {
                for (int i = tileIndex; i < tileIndex + tileData.amountToSpawn; i++)
                {
                    TileScript tile;
                    if (i < spawnedTiles.Count)
                        tile = spawnedTiles[i];
                    else
                    {
                        tile = Instantiate(tileData.tilePrefab, tileParent).GetComponent<TileScript>();
                        tile.puzzleManager = this;
                        spawnedTiles.Add(tile);
                    }

                    tile.transform.localPosition = coordMap[i].worldPos;
                    tile.pointingTo = tileData.pointingTo;

                    Vector2Int gridCoord = coordMap[i].gridCoord;

                    TileScript tileScript = tile;
                    gridCoordToTile[gridCoord] = tileScript;

                    tile.finalGridPosition = gameData.solutions[i].finalGridPosition;

                    if(gridCoord == Vector2.zero)
                    {
                        tile.isLit = true;
                        tile.isSource = true;
                    }
                    else if(gridCoord == new Vector2(5, 3))
                    {
                        tile.isEnd = true;
                    }
                }
                tileIndex += tileData.amountToSpawn;
            }
            AssignNeighbors();
            ReLightTiles();
        }

        private void AssignNeighbors()
        {
            foreach (var kvp in gridCoordToTile)
            {
                Vector2Int coord = kvp.Key;
                TileScript tile = kvp.Value;

                if (coord == new Vector2Int(0, 0))
                {
                    tile.isSource = true;
                    tile.isLit = true;
                    tile.isEnd = false;
                }
                else if (coord == new Vector2Int(5, 3))
                {
                    tile.isSource = false;
                    tile.isLit = false;
                    tile.isEnd = true;
                }
                else
                {
                    tile.isSource = false;
                    tile.isEnd = false;
                    tile.isLit = false;
                }

                gridCoordToTile.TryGetValue(coord + Vector2Int.up, out tile.up);
                gridCoordToTile.TryGetValue(coord + Vector2Int.down, out tile.down);
                gridCoordToTile.TryGetValue(coord + Vector2Int.left, out tile.left);
                gridCoordToTile.TryGetValue(coord + Vector2Int.right, out tile.right);

                tile.InitTile();
            }
        }
        private void shuffleList(List<(Vector2Int gridCoord, Vector2 worldPos)> list)
        {
            var count = list.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = Random.Range(i, count);
                var tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }
        }



        public void SwapSelectTiles(Transform tile)
        {
            if (tile == selectedTile)
            {
                selectedTile.GetComponent<SpriteRenderer>().color = defaultColor;
                selectedTile = null;
                return;
            }
            if (selectedTile == null)
            {
                selectedTile = tile;
                selectedTile.GetComponent<SpriteRenderer>().color = selectedColor;
                return;
            }

            selectedTile.GetComponent<SpriteRenderer>().color = defaultColor;
            SwapTiles(tile, selectedTile);
        }

        private void SwapTiles(Transform tile1, Transform tile2, Vector2? customPosition = null)
        {
            // Disable click events
            eventHandler.SetActive(false);

            Vector2 pos1 = tile1.localPosition;
            Vector2 pos2 = customPosition ?? tile2.localPosition;

            tile1.DOKill();
            tile2?.DOKill();

            tile1.DOLocalMove(pos2, swapDuration).SetEase(Ease.InOutQuad);
            tile2?.DOLocalMove(pos1, swapDuration).SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    selectedTile = null;

                    SwapTileNeighbors(tile1.GetComponent<TileScript>(), tile2.GetComponent<TileScript>());

                    // Enable click events
                    eventHandler.SetActive(true);
                });
        }

        private void SwapTileNeighbors(TileScript a, TileScript b)
        {
            SwapBoolFlags(ref a.isSource, ref b.isSource);
            SwapBoolFlags(ref a.isEnd, ref b.isEnd);

            var aNeighbors = new TileScript[] { a.up, a.down, a.left, a.right };
            var bNeighbors = new TileScript[] { b.up, b.down, b.left, b.right };

            for (int i = 0; i < 4; i++)
            {
                if (aNeighbors[i] == b) aNeighbors[i] = a;
                if (bNeighbors[i] == a) bNeighbors[i] = b;
            }

            a.UpdateNeighbors(bNeighbors[0], bNeighbors[1], bNeighbors[2], bNeighbors[3]);
            b.UpdateNeighbors(aNeighbors[0], aNeighbors[1], aNeighbors[2], aNeighbors[3]);

            a.UpdateConnections();
            b.UpdateConnections();
            
            ReLightTiles();
        }
        private void SwapBoolFlags(ref bool flagA, ref bool flagB)
        {
            if (flagA)
            {
                flagA = false;
                flagB = true;
            }
            else if (flagB)
            {
                flagB = false;
                flagA = true;
            }
        }
        private void ReLightTiles()
        {
            TileScript sourceTile = null;
            foreach(var item in gridCoordToTile)
            {
                if (item.Value.isSource)
                {
                    sourceTile = item.Value;
                }

                item.Value.isLit = false;
                item.Value.TriggerFlame();
            }

            if (sourceTile == null)
                return;
            sourceTile.isLit = true;
            sourceTile.LightNeighbor(new HashSet<TileScript>());
        }


        public void CheckWin()
        {
            if (gameOver)
                return;
            int counter = 0;
            foreach (var item in gridCoordToTile)
            {
                if (item.Value.isLit)
                    counter++;
            }
            if (counter >= gridCoordToTile.Count)
                Completed();
            else
                ResetGame();
        }






        //---test functions
        public GameObject endgamePanel;
        public GameObject eventHandler;
        public void EndGamePanel()
        {
            endgamePanel.SetActive(true);
        }
        public void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

