/*
Created by: Happyrobot33
Date: 4/12/2022
Version: 1.0
*/

using UnityEngine;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

public class Shader_Finder : EditorWindow
{
    //create a shader list
    public static Shader[] shaderList = new Shader[0];
    public static string[] shaderNameList = new string[0];
    public int index = 0;
    public GameObject ObjectToSearch;
    public int objectCountFound = 0;

    [MenuItem("VRC Packages/Shader Finder")]
    public static void ShowWindow()
    {
        //create a new window
        EditorWindow window = EditorWindow.GetWindow(typeof(Shader_Finder));
        //set name of window
        window.titleContent = new GUIContent("Shader Finder");
        window.Show();
    }

    private void OnGUI(){
        //create a new label
        GUILayout.Label("Shader Finder", EditorStyles.boldLabel);
        //create a description label
        GUILayout.Label("This tool will find all the shaders in the scene, and select the relevant gameobjects. If you are using a shader that locks itself into a 'optimized' mode, then you will need to search for it under Hidden, IE for Poiyomi it will be under Hidden -> Locked", EditorStyles.wordWrappedLabel);
        //create a new dropdown menu
        index = EditorGUILayout.Popup(index, shaderNameList);

        //find every shader in the project and add it to the list
        if (shaderList.Length == 0)
        {
            shaderList = Resources.FindObjectsOfTypeAll<Shader>();
            shaderNameList = new string[shaderList.Length];
            for (int i = 0; i < shaderList.Length; i++)
            {
                shaderNameList[i] = shaderList[i].name;
            }
        }

        //add a field to input a gameobject from the scene
        ObjectToSearch = EditorGUILayout.ObjectField("Object to search", ObjectToSearch, typeof(GameObject), true) as GameObject;

        if(shaderList.Length > 0 && ObjectToSearch != null)
        {
            //get every object in the scene
            //GameObject[] objects = (GameObject[])FindObjectsOfTypeAll(typeof(GameObject));
            Transform[] objectsTRANSFORMS = ObjectToSearch.GetComponentsInChildren<Transform>(true);

            //turn the objects array into an array of gameobjects, not transforms
            GameObject[] objects = new GameObject[objectsTRANSFORMS.Length];
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i] = objectsTRANSFORMS[i].gameObject;
            }

            //list the ammount of objects with the selected shader
            EditorGUILayout.LabelField("Total Objects to search: " + objects.Length);
            EditorGUILayout.LabelField("Objects with the selected shader on last search: " + objectCountFound);

            //create a new button
            if(GUILayout.Button("Select objects with specified Shader")){
                //clear selection
                Selection.activeGameObject = null;
                //get every object in the list with a renderer of some kind
                foreach (GameObject obj in objects)
                {
                    if (obj.GetComponent<MeshRenderer>() != null)
                    {
                        //check if the object has multiple materials
                        if (obj.GetComponent<Renderer>().sharedMaterials.Length > 1)
                        {
                            //check if the object has the selected shader
                            for (int i = 0; i < obj.GetComponent<Renderer>().sharedMaterials.Length; i++)
                            {
                                //check to make sure the material isnt null
                                if (obj.GetComponent<Renderer>().sharedMaterials[i] != null)
                                {
                                    //check if the material is the selected shader
                                    if (obj.GetComponent<Renderer>().sharedMaterials[i].shader == shaderList[index])
                                    {
                                        //select the object
                                        Selection.objects = addToArray(Selection.objects, obj);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //check if the object has the selected shader
                            if (obj.GetComponent<Renderer>().sharedMaterial.shader == shaderList[index])
                            {
                                //select the object
                                Selection.objects = addToArray(Selection.objects, obj);
                            }
                        }
                    }
                    else if (obj.GetComponent<SkinnedMeshRenderer>() != null)
                    {
                        //check if the object has multiple materials
                        if (obj.GetComponent<Renderer>().sharedMaterials.Length > 1)
                        {
                            //check if the object has the selected shader
                            for (int i = 0; i < obj.GetComponent<Renderer>().sharedMaterials.Length; i++)
                            {
                                //check to make sure the material isnt null
                                if (obj.GetComponent<Renderer>().sharedMaterials[i] != null)
                                {
                                    if (obj.GetComponent<Renderer>().sharedMaterials[i].shader == shaderList[index])
                                    {
                                        //select the object
                                        Selection.objects = addToArray(Selection.objects, obj);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //check if the object has the selected shader
                            if (obj.GetComponent<Renderer>().sharedMaterial.shader == shaderList[index])
                            {
                                //select the object
                                Selection.objects = addToArray(Selection.objects, obj);
                            }
                        }
                    }
                }
            }
        }

        objectCountFound = Selection.objects.Length;
    }

    private UnityEngine.Object[] addToArray(UnityEngine.Object[] array, GameObject obj)
    {
        //create a new array
        UnityEngine.Object[] newArray = new UnityEngine.Object[array.Length + 1];
        //add the old array to the new array
        for (int i = 0; i < array.Length; i++)
        {
            newArray[i] = array[i];
        }
        //add the new object to the new array
        newArray[array.Length] = obj;
        //return the new array
        return newArray;
    }
}