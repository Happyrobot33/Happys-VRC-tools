/*
Created by: Happyrobot33
Date: 4/12/2022
Version: 1.0
*/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Linq;

namespace HappysTools.ShaderFinder
{
    public class ShaderFinder : EditorWindow
    {
        public GameObject ObjectToSearch;

        /// <summary>
        /// Dictionary of all the shaders on the object, and the gameobjects that use them
        /// </summary>
        /// <typeparam name="string">Shader name/path</typeparam>
        /// <typeparam name="List<GameObject>">List of gameobjects that use the shader</typeparam>
        internal Dictionary<String, List<GameObject>> ObjectsWithShader =
            new Dictionary<string, List<GameObject>>();

        int childCount = 0;

        int currentShaderIndex = 0;

        [MenuItem("VRC Packages/Happys Tools/Shader Finder")]
        public static void ShowWindow()
        {
            //create a new window
            EditorWindow window = EditorWindow.GetWindow(typeof(ShaderFinder));
            //set name of window
            window.titleContent = new GUIContent("Shader Finder");
            window.Show();
        }

        private void OnGUI()
        {
            //create a new label
            GUILayout.Label("Shader Finder", EditorStyles.boldLabel);
            //create a description label
            GUILayout.Label(
                "This tool will find all the shaders in the scene, and select the relevant gameobjects. If you are using a shader that locks itself into a 'optimized' mode, then you will need to search for it under Hidden, IE for Poiyomi it will be under Hidden -> Locked",
                EditorStyles.wordWrappedLabel
            );
            //create a object input field
            EditorGUI.BeginChangeCheck();
            ObjectToSearch = (GameObject)
                EditorGUILayout.ObjectField(
                    "Object to Search",
                    ObjectToSearch,
                    typeof(GameObject),
                    true
                );

            //if object exists, run processing on it once the user has changed the input field
            if (ObjectToSearch != null && EditorGUI.EndChangeCheck())
            {
                //clear the dictionary
                ObjectsWithShader.Clear();
                //get all the children objects including the object itself by getting all transforms
                Transform[] allChildrenTransforms =
                    ObjectToSearch.GetComponentsInChildren<Transform>(true);
                //get the gameobjects themselves
                GameObject[] allChildren = new GameObject[allChildrenTransforms.Length];
                for (int i = 0; i < allChildrenTransforms.Length; i++)
                {
                    allChildren[i] = allChildrenTransforms[i].gameObject;
                }
                childCount = allChildren.Length;

                //process each child into the dictionary
                foreach (GameObject obj in allChildren)
                {
                    /*
                    This covers
                    - MeshRenderer
                    - SkinnedMeshRenderer
                    - ParticleSystemRenderer
                    - TrailRenderer
                    - LineRenderer
                    - SpriteRenderer
                    */
                    if (obj.GetComponent<Renderer>() != null)
                    {
                        for (
                            int i = 0;
                            i < obj.GetComponent<Renderer>().sharedMaterials.Length;
                            i++
                        )
                        {
                            //check to make sure the material isnt null
                            if (obj.GetComponent<Renderer>().sharedMaterials[i] != null)
                            {
                                AddToDictionary(
                                    obj,
                                    obj.GetComponent<Renderer>().sharedMaterials[i]
                                );
                            }
                        }
                    }

                    //we need a dedicated exception for text and image components since they dont share the renderer component
                    if (obj.GetComponent<Text>() != null)
                    {
                        //check to make sure the material isnt null
                        if (obj.GetComponent<Text>().material != null)
                        {
                            AddToDictionary(obj, obj.GetComponent<Text>().material);
                        }
                    }
                    
                    if (obj.GetComponent<Image>() != null)
                    {
                        //check to make sure the material isnt null
                        if (obj.GetComponent<Image>().material != null)
                        {
                            AddToDictionary(obj, obj.GetComponent<Image>().material);
                        }
                    }
                }
            }

            GUI.enabled = ObjectToSearch != null;
            EditorGUILayout.LabelField("Total objects searched: " + childCount);
            EditorGUILayout.LabelField("Total shaders in-use: " + ObjectsWithShader.Count);

            //display a dropdown for the shaders
            if (ObjectsWithShader.Count > 0)
            {
                //create a new label
                GUILayout.Label("Shaders", EditorStyles.boldLabel);
                //create a new label
                GUILayout.Label(
                    "Select a shader to select all the objects that use it",
                    EditorStyles.wordWrappedLabel
                );
                //create a new dropdown
                currentShaderIndex = EditorGUILayout.Popup(
                    "Shader",
                    currentShaderIndex,
                    ObjectsWithShader.Keys.ToArray()
                );
                //create a new button
                if (GUILayout.Button("Select Objects"))
                {
                    //select all the objects that use the shader
                    Selection.objects = ObjectsWithShader[
                        ObjectsWithShader.Keys.ToArray()[currentShaderIndex]
                    ].ToArray();
                }
            }
        }

        /// <summary>
        /// Adds the object to the dictionary entry, or creates a new list if the shader is not in the dictionary yet
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="mat"></param>
        private void AddToDictionary(GameObject obj, Material mat)
        {
            //check if the shader is already in the dictionary
            if (ObjectsWithShader.ContainsKey(mat.shader.name))
            {
                //add the object to the list
                ObjectsWithShader[mat.shader.name].Add(obj);
            }
            else
            {
                //create a new list
                List<GameObject> newList = new List<GameObject>();
                //add the object to the list
                newList.Add(obj);
                //add the list to the dictionary
                ObjectsWithShader.Add(mat.shader.name, newList);
            }
        }
    }
}
