using com.puzzles.Soul_Boss_Minigame;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SavePositionsToGameData
{
    [MenuItem("Tools/Save Child LocalPositions To GameData")]
    private static void SaveChildPositions()
    {
        // selektuj parent
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("Nema selektovanog parent objekta!");
            return;
        }

        // nađi GameData asset (možeš ručno referencirati, ili pretražiti po tipu)
        string[] guids = AssetDatabase.FindAssets("t:MinigameData");
        if (guids.Length == 0)
        {
            Debug.LogError("Nisam našao GameData.asset u projektu!");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        MinigameData gameData = AssetDatabase.LoadAssetAtPath<MinigameData>(path);

        if (gameData == null)
        {
            Debug.LogError("Nisam mogao učitati GameData asset!");
            return;
        }

        // očisti listu prije dodavanja
        gameData.waypointLocations = new List<Vector2>();

        // pokupi sve child transform-e (bez parenta)
        Transform[] children = selected.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child == selected.transform) continue; // preskoči parent
            gameData.waypointLocations.Add(child.localPosition);
        }

        EditorUtility.SetDirty(gameData);
        AssetDatabase.SaveAssets();

        Debug.Log("Snimljeno " + gameData.waypointLocations.Count + " local positions u GameData.asset");
    }
}