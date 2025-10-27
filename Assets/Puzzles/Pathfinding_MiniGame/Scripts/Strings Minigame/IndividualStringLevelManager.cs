using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.puzzles.PathfindingMiniGame
{
    public class IndividualStringLevelManager : MonoBehaviour
    {
        List<LineRenderer> edges;
        Material defaultMaterial;
        Material outlineMaterial;
        StringPuzzleManager gameManager;
        public void Init(List<LineRenderer> lineRenderers, Material defaultMat, Material outlineMat, StringPuzzleManager manager)
        {
            edges = new List<LineRenderer>(lineRenderers);
            defaultMaterial = defaultMat;
            outlineMaterial = outlineMat;
            gameManager = manager;

            CheckIntersections();
        }

        public void CheckIntersections()
        {
            HashSet<LineRenderer> intersectingLines = new HashSet<LineRenderer>();
            for (int i = 0; i < edges.Count; i++)
            {
                for (int j = i + 1; j < edges.Count; j++)
                {
                    Vector3[] positions1 = new Vector3[2];
                    Vector3[] positions2 = new Vector3[2];

                    edges[i].GetPositions(positions1);     
                    edges[j].GetPositions(positions2);

                    if (DoLinesIntersect(positions1[0], positions1[1], positions2[0], positions2[1]))
                    {
                        intersectingLines.Add(edges[i]);
                        intersectingLines.Add(edges[j]);
                    }

                }
            }

            foreach (var line in edges)
            {
                if (intersectingLines.Contains(line))
                {
                    ChangeMaterials(line, outlineMaterial);
                }
                else
                {
                    ChangeMaterials(line, defaultMaterial);
                }
            }

            if (intersectingLines.Count == 0)
                LevelFinished();

        }

        void LevelFinished()
        {
            gameManager.LoadNextLevel();
        }
        void ChangeMaterials(LineRenderer line, Material material)
        {
            Material[] newMaterials = line.materials;
            newMaterials[0] = defaultMaterial;
            newMaterials[1] = material;
            line.materials = newMaterials;
        }
        bool DoLinesIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            float cross1 = CrossProduct(q1 - p1, p2 - p1);
            float cross2 = CrossProduct(q2 - p1, p2 - p1);
            float cross3 = CrossProduct(p1 - q1, q2 - q1);
            float cross4 = CrossProduct(p2 - q1, q2 - q1);

            if ((cross1 == 0 && cross2 == 0) || (cross3 == 0 && cross4 == 0))
            {
                return false;
            }

            return (cross1 * cross2 < 0) && (cross3 * cross4 < 0);
        }

        float CrossProduct(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

    }

}
