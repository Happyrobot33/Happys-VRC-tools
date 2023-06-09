/*
Created by: Happyrobot33
Date: 4/12/2022
Version: 1.0
*/

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HappysTools.InspectorTweaks
{
    public class CustomTransform : MonoBehaviour { }

    [CustomEditor(typeof(Transform), true)]
    [CanEditMultipleObjects]
    public class CustomTransformInspector : Editor
    {
        //Unity's built-in editor
        Editor defaultEditor;
        Transform transform;

        void OnEnable()
        {
            //When this inspector is created, also create the built-in inspector
            defaultEditor = Editor.CreateEditor(
                targets,
                Type.GetType("UnityEditor.TransformInspector, UnityEditor")
            );
            transform = target as Transform;
        }

        void OnDisable()
        {
            //When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
            //Also, make sure to call any required methods like OnDisable
            MethodInfo disableMethod = defaultEditor
                .GetType()
                .GetMethod(
                    "OnDisable",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
                );
            if (disableMethod != null)
                disableMethod.Invoke(defaultEditor, null);
            DestroyImmediate(defaultEditor);
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Local Space", EditorStyles.boldLabel);
            //create a copy button that is only as long as the label
            if (GUILayout.Button("Copy", GUILayout.Width(50)))
            {
                //copy the transform's local position, rotation, and scale to the clipboard
                string clipboard = "";
                clipboard +=
                    transform.localPosition.x
                    + ","
                    + transform.localPosition.y
                    + ","
                    + transform.localPosition.z
                    + "\n";
                clipboard +=
                    transform.localRotation.x
                    + ","
                    + transform.localRotation.y
                    + ","
                    + transform.localRotation.z
                    + ","
                    + transform.localRotation.w
                    + "\n";
                clipboard +=
                    transform.localScale.x
                    + ","
                    + transform.localScale.y
                    + ","
                    + transform.localScale.z
                    + "\n";
                EditorGUIUtility.systemCopyBuffer = clipboard;
            }
            GUILayout.EndHorizontal();
            defaultEditor.OnInspectorGUI();

            //Show World Space Transform
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("World Space", EditorStyles.boldLabel);
            //create a copy button that is only as long as the label
            if (GUILayout.Button("Copy", GUILayout.Width(50)))
            {
                //copy the transform's world position, rotation, and scale to the clipboard
                string clipboard = "";
                clipboard +=
                    transform.position.x
                    + ","
                    + transform.position.y
                    + ","
                    + transform.position.z
                    + "\n";
                clipboard +=
                    transform.rotation.x
                    + ","
                    + transform.rotation.y
                    + ","
                    + transform.rotation.z
                    + ","
                    + transform.rotation.w
                    + "\n";
                clipboard +=
                    transform.localScale.x
                    + ","
                    + transform.localScale.y
                    + ","
                    + transform.localScale.z
                    + "\n";
                EditorGUIUtility.systemCopyBuffer = clipboard;
            }
            GUILayout.EndHorizontal();

            GUI.enabled = false;
            Vector3 localPosition = transform.localPosition;
            Quaternion localRotation = transform.localRotation;
            Vector3 localScale = transform.localScale;

            //defaultEditor.OnInspectorGUI();
            //draw world space transform
            EditorGUILayout.Vector3Field("Position", transform.position);
            EditorGUILayout.Vector3Field("Rotation", transform.rotation.eulerAngles);
            EditorGUILayout.Vector3Field("Scale", transform.localScale);
            GUI.enabled = true;

            //add a space
            EditorGUILayout.Space();

            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("Mirroring Tools", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Experimental! May not work as expected!",
                EditorStyles.miniLabel
            );
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Local Space");
            //create a button to mirror across an axis, not using scale
            if (GUILayout.Button("X"))
            {
                //undo state
                Undo.RecordObject(transform, "Mirror across X");
                //mirror the transform across the X axis
                transform.localPosition = new Vector3(
                    -transform.localPosition.x,
                    transform.localPosition.y,
                    transform.localPosition.z
                );
                //mirror the angle using purely quaternion calculations
                transform.localRotation = Quaternion.Euler(
                    new Vector3(
                        -transform.localRotation.eulerAngles.x,
                        transform.localRotation.eulerAngles.y,
                        transform.localRotation.eulerAngles.z
                    )
                );
            }
            if (GUILayout.Button("Y"))
            {
                //undo state
                Undo.RecordObject(transform, "Mirror across Y");
                //mirror the transform across the Y axis
                transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    -transform.localPosition.y,
                    transform.localPosition.z
                );
                transform.localRotation = Quaternion.Euler(
                    new Vector3(
                        transform.localRotation.eulerAngles.x,
                        -transform.localRotation.eulerAngles.y,
                        transform.localRotation.eulerAngles.z
                    )
                );
            }
            if (GUILayout.Button("Z"))
            {
                //undo state
                Undo.RecordObject(transform, "Mirror across Z");
                //mirror the transform across the Z axis
                transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    transform.localPosition.y,
                    -transform.localPosition.z
                );
                transform.localRotation = Quaternion.Euler(
                    new Vector3(
                        transform.localRotation.eulerAngles.x,
                        transform.localRotation.eulerAngles.y,
                        -transform.localRotation.eulerAngles.z
                    )
                );
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("World Space");
            //create a button to mirror across an axis, not using scale
            if (GUILayout.Button("X"))
            {
                //undo state
                Undo.RecordObject(transform, "Mirror across X");
                //mirror the transform across the X axis
                transform.position = new Vector3(
                    -transform.position.x,
                    transform.position.y,
                    transform.position.z
                );
                //ensure rotation is mirrored properly too
                transform.rotation = Quaternion.Euler(
                    new Vector3(
                        -transform.rotation.eulerAngles.x,
                        180 - transform.rotation.eulerAngles.y,
                        transform.rotation.eulerAngles.z
                    )
                );
            }
            if (GUILayout.Button("Y"))
            {
                //undo state
                Undo.RecordObject(transform, "Mirror across Y");
                //mirror the transform across the Y axis
                transform.position = new Vector3(
                    transform.position.x,
                    -transform.position.y,
                    transform.position.z
                );
                transform.rotation = Quaternion.Euler(
                    new Vector3(
                        transform.rotation.eulerAngles.x,
                        -transform.rotation.eulerAngles.y,
                        -transform.rotation.eulerAngles.z
                    )
                );
            }
            if (GUILayout.Button("Z"))
            {
                //undo state
                Undo.RecordObject(transform, "Mirror across Z");
                //mirror the transform across the Z axis
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    -transform.position.z
                );
                //ensure rotation is mirrored properly too
                transform.rotation = Quaternion.Euler(
                    new Vector3(
                        -transform.rotation.eulerAngles.x,
                        -transform.rotation.eulerAngles.y,
                        transform.rotation.eulerAngles.z
                    )
                );
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            //add text to the inspector indicating who made these mods
            EditorGUILayout.LabelField(
                "Modified Transform Script by Happyrobot33",
                EditorStyles.wordWrappedLabel
            );
        }
    }
}
