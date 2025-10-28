using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data")]
public class LevelData : ScriptableObject
{
    public int winCondition;

    public GameObject background;
    public Vector2 gridSize;

    public List<ButtonData> buttonData;
    public float buttonScale = 1;
    public Vector2 buttonOffset = Vector2.zero;

    public List<Vector2> barricadeCoordinates;

    [System.Serializable]
    public class ButtonData
    {
        public Vector2 buttonCoordinates;
        public int amount;
        public SpawnDirection spawnDirection;
        public Sprite sprite;
    }
}
