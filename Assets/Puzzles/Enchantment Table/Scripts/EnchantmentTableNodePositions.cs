using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enchantment Table Position Data")]
public class EnchantmentTableNodePositions : ScriptableObject
{
    public List<HexagonPositions> hexagonPositions;
    public List<TriangleData> triangleData;

    public List<PathPositions> pathPositions1;      //angle 0
    public List<PathPositions> pathPositions2;      //angle 60
    public List<PathPositions> pathPositions3;      //angle -60



    [System.Serializable]
    public class HexagonPositions
    {
        public float yPos;
        public List<float> xPos;
        public List<int> trianglePos;
    }



    [System.Serializable]
    public class PathPositions
    {
        public float yPos;
        public List<float> xPos;
    }


    [System.Serializable]
    public class TriangleData
    {
        public int[] position;
    }
}
