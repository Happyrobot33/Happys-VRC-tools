using UnityEditor;
using UnityEngine;

public class JSONUIWindow : EditorWindow
{
    [MenuItem("VRC Packages/Pretty JSON UI/Prefabs/JSON Manager")]
    public static void OpenFlightBasic()
    {
        // Open the Lite prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Packages/com.happyrobot33.prettyjsonui/Runtime/Prefabs/JSON Manager.prefab", typeof(GameObject)) as GameObject;
        AddPrefabToScene(prefab);
    }


    static void AddPrefabToScene(GameObject prefab)
    {
        // Check to see if the prefab is already in the scene
        if (GameObject.Find(prefab.name))
        {
            EditorUtility.DisplayDialog("Prefab already in scene", "The prefab is already in the scene, so it wont be added", "OK");
            return;
        }
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
    }
}