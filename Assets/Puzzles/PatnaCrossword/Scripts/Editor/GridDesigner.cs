using UnityEditor;
using UnityEngine;
using System;

namespace com.frameworks.PatnaCrossword
{
    [CustomEditor(typeof(GridData), false)]
    [CanEditMultipleObjects]
    [Serializable]
    public class GridDesigner : Editor
    {
        private GridData levelDataInstance => target as GridData;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ClearBoardButton();
            EditorGUILayout.Space();
            DrawInputFields();
            EditorGUILayout.Space();

            if (levelDataInstance.board != null && levelDataInstance.columns > 0 && levelDataInstance.rows > 0)
                DrawBoardTable();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(levelDataInstance);
        }

        private void ClearBoardButton()
        {
            if (GUILayout.Button("Clear board"))
                levelDataInstance.Clear();
        }

        private void DrawInputFields()
        {
            var columnsTemp = levelDataInstance.columns;
            var rowsTemp = levelDataInstance.rows;
            var startTemp = levelDataInstance.startIndex;
            var endTemp = levelDataInstance.destinationIndex;

            levelDataInstance.rows = EditorGUILayout.IntField("Rows", levelDataInstance.rows);
            levelDataInstance.columns = EditorGUILayout.IntField("Columns", levelDataInstance.columns);
            levelDataInstance.initialTargetPosition = EditorGUILayout.Vector2IntField("Target Position (Top-Left Corner)", levelDataInstance.initialTargetPosition);


            levelDataInstance.defaultSprite = (Sprite)EditorGUILayout.ObjectField(levelDataInstance.defaultSprite, typeof(Sprite), false);

            if ((levelDataInstance.columns != columnsTemp || levelDataInstance.rows != rowsTemp) &&
                levelDataInstance.columns > 0 && levelDataInstance.rows > 0)
            {
                levelDataInstance.CreateLevelBoard();
            }
        }

        private void DrawBoardTable()
        {
            var tableStyle = new GUIStyle("box");
            tableStyle.padding = new RectOffset(10, 10, 10, 10);
            tableStyle.margin.left = 32;

            var headerColumnStyle = new GUIStyle();
            headerColumnStyle.fixedWidth = 65;
            headerColumnStyle.alignment = TextAnchor.MiddleCenter;

            var rowStyle = new GUIStyle();
            rowStyle.fixedWidth = 25;
            rowStyle.alignment = TextAnchor.MiddleCenter;

            var dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
            dataFieldStyle.normal.background = Texture2D.grayTexture;
            dataFieldStyle.onNormal.background = Texture2D.whiteTexture;


            for (var row = 0; row < levelDataInstance.rows; row++)
            {
                EditorGUILayout.BeginHorizontal(headerColumnStyle);

                var rowData = levelDataInstance.board[row];

                for (var column = 0; column < levelDataInstance.columns; column++)
                {
                    EditorGUILayout.BeginVertical(rowStyle);

                    rowData.column[column] = EditorGUILayout.Toggle(rowData.column[column], dataFieldStyle);
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();


            for (var row = 0; row < levelDataInstance.rows; row++)
            {
                EditorGUILayout.BeginHorizontal(headerColumnStyle);

                var rowData = levelDataInstance.board[row];

                if (rowData.column == null || rowData.column.Length != levelDataInstance.columns)
                    rowData.column = new bool[levelDataInstance.columns];
                if (rowData.sprites == null || rowData.sprites.Length != levelDataInstance.columns)
                    rowData.sprites = new Sprite[levelDataInstance.columns];

                for (var column = 0; column < levelDataInstance.columns; column++)
                {
                    EditorGUILayout.BeginVertical(rowStyle);
                  
                    rowData.sprites[column] = (Sprite)EditorGUILayout.ObjectField(rowData.sprites[column],
                                                              typeof(Sprite),
                                                              false);
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();


        }
    }
}


