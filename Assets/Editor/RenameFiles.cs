using UnityEngine;
using UnityEditor;
using System.IO;

public class RemovePrefix : EditorWindow
{
    private string prefix = "";

    [MenuItem("Tools/Remove Prefix From Selected Assets")]
    static void Init()
    {
        GetWindow<RemovePrefix>("Remove Prefix");
    }

    void OnGUI()
    {
        prefix = EditorGUILayout.TextField("Prefix to remove:", prefix);

        if (GUILayout.Button("Remove Prefix from Selected"))
        {
            foreach (var obj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                string filename = Path.GetFileNameWithoutExtension(path);
                string ext = Path.GetExtension(path);
                string dir = Path.GetDirectoryName(path);

                if (filename.StartsWith(prefix))
                {
                    string newName = filename.Substring(prefix.Length);
                    string newPath = Path.Combine(dir, newName + ext);
                    AssetDatabase.RenameAsset(path, newName);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Prefixes removed successfully!");
        }
    }
}