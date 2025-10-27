using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.puzzles.PathfindingMiniGame
{
    public class StringPuzzleManager : MonoBehaviour
    {
        [SerializeField] private PathFindingMiniGameData puzzleData;
        private int currentLevel = 0;
        private GameObject currentLevelBg;

        public Sprite stationarySprite;

        public GameObject vertexPrefab; 
        public GameObject linePrefab; 
        private Dictionary<int, GemScript> vertices = new Dictionary<int, GemScript>();
        private List<Transform> puzzleParents = new List<Transform>();

        public Material defaultMaterial;
        public Material outlineMaterial;

        [SerializeField] private PathFindingManager gameManager;


        private void Start()
        {
            InitLevels();
        }
        void OnStringPuzzleComplete()
        {
            gameManager.LoadNextPart();
        }

        public void LoadNextLevel()
        {
            StartCoroutine(LoadingDelay());
        }

        IEnumerator LoadingDelay()
        {
            yield return new WaitForSeconds(0.5f);
            if (currentLevelBg != null)
                currentLevelBg.SetActive(false);
            if (currentLevel >= puzzleData.stringPuzzles.Count)
            {
                OnStringPuzzleComplete();
            }
            else
            {
                currentLevelBg = puzzleParents[currentLevel].gameObject;
                currentLevelBg.SetActive(true);
                currentLevel++;
            }
        }

        public void InitLevels()
        {
            for(int i=0; i<puzzleData.stringPuzzles.Count; i++)
            {
                SpawnGraph(i);
            }
            LoadNextLevel();
        }
        public void SpawnGraph(int currentLevel)
        {
            Transform currentParent = new GameObject().transform;
            IndividualStringLevelManager currentLevelManager = currentParent.gameObject.AddComponent<IndividualStringLevelManager>();
            currentParent.parent = transform;
            currentParent.gameObject.SetActive(false);
            puzzleParents.Add(currentParent);

            vertices.Clear();

            Vector3[] positions = puzzleData.stringPuzzles[currentLevel].positions;
            List<Vector2Int> edges = puzzleData.stringPuzzles[currentLevel].edges;
            bool[] isStationary = puzzleData.stringPuzzles[currentLevel].isStationary;

            // Spawn vertices
            for (int i = 0; i < positions.Length; i++)
            {
                GameObject vertex = Instantiate(vertexPrefab, positions[i], Quaternion.identity, currentParent);
                if (isStationary[i])
                {
                    vertex.GetComponent<BoxCollider2D>().enabled = false;
                    vertex.GetComponent<SpriteRenderer>().sprite = stationarySprite;
                }
                else
                {
                }
                vertex.name = "Vertex " + (i + 1);
                vertices[i + 1] = vertex.GetComponent<GemScript>();
            }

            List<LineRenderer> spawnedLineRenderers = new List<LineRenderer>();
            // Spawn edges 
            for (int i = 0; i < edges.Count; i++)
            {
                int v1 = edges[i].x;
                int v2 = edges[i].y;

                if (vertices.ContainsKey(v1) && vertices.ContainsKey(v2))
                {
                    GameObject lineObj = Instantiate(linePrefab, currentParent);
                    LineRenderer lr = lineObj.GetComponent<LineRenderer>();
                    spawnedLineRenderers.Add(lr);

                    lr.positionCount = 2;

                    lr.SetPosition(0, vertices[v1].transform.position);
                    lr.SetPosition(1, vertices[v2].transform.position);

                    vertices[v1].InitGem(0, lr, currentLevelManager);
                    vertices[v2].InitGem(1, lr, currentLevelManager);

                }
            }
            currentLevelManager.Init(spawnedLineRenderers, defaultMaterial, outlineMaterial, this);
        }
    }

}
