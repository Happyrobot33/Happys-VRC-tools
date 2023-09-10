using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.SDKBase;
using VRC.SDKBase.Editor;
using VRC.SDK3A.Editor;
using VRC.SDKBase.Editor.BuildPipeline;
using VRC.Core;
using System.Threading.Tasks;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase.Editor.Api;
using System;

namespace HappysTools.AvatarBulkUploader
{
    public class BulkUploader : MonoBehaviour
    {
        [MenuItem("VRC Packages/Happys Tools/Bulk Uploader")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(BulkUploaderWindow), false, "Bulk Uploader");
        }
    }

    //editor window
    public class BulkUploaderWindow : EditorWindow
    {
        //list of avatars to upload
        public GameObject[] avatars;
        SerializedObject serializedObject;
        void OnEnable()
        {
            //get reference 
            serializedObject = new SerializedObject(this);
            //initialize avatars
            avatars = new GameObject[0];
        }
        void OnGUI()
        {
            GUILayout.Label("Bulk Uploader", EditorStyles.boldLabel);

            //show a list of avatars
            EditorGUILayout.PropertyField(serializedObject.FindProperty("avatars"), true);
            serializedObject.ApplyModifiedProperties();

            string errorReason = "";

            //disable GUI if no avatars are selected
            if (avatars.Length == 0)
            {
                GUI.enabled = false;
            }
            else
            {
                //check that all avatars are valid
                foreach (GameObject avatar in avatars)
                {
                    if (avatar == null || !CheckIfAvatarIsLive(avatar))
                    {
                        //disable GUI
                        GUI.enabled = false;
                        string avatarName = "null";
                        if (avatar != null)
                        {
                            avatarName = avatar.name;
                        }
                        //display error reason
                        if (avatar == null)
                        {
                            errorReason = "One of the avatar slots is null";
                        }
                        else if (!CheckIfAvatarIsLive(avatar))
                        {
                            errorReason = avatarName + " is not associated with a ID";
                        }
                        break;
                    }
                }
            }


            //display error reason
            if(errorReason != "")
            {
                bool prevGUIEnabled = GUI.enabled;
                GUI.enabled = true;

                EditorGUILayout.HelpBox(errorReason, MessageType.Error);

                GUI.enabled = prevGUIEnabled;
            }

            if (GUILayout.Button("Upload Avatars"))
            {
                //get the build panel
                if (!VRCSdkControlPanel.TryGetBuilder<IVRCSdkAvatarBuilderApi>(out var builder))
                {
                    Debug.LogError("Could not get avatar builder");
                    return;
                }
                //upload avatars
                UploadAvatars(builder);
            }
        }

        async void UploadAvatars(IVRCSdkAvatarBuilderApi builder)
        {
            //register complex progress bar
            ComplexProgressBar complexProgressBar = ScriptableObject.CreateInstance<ComplexProgressBar>();

            ProgressBar[] progressBars = new ProgressBar[avatars.Length];
            ProgressBar totalProgressBar = complexProgressBar.RegisterNewProgressBar("Total", "Total Progress");

            //register a progress bar for each avatar
            for (int i = 0; i < avatars.Length; i++)
            {
                GameObject avatar = avatars[i];
                progressBars[i] = complexProgressBar.RegisterNewProgressBar(avatar.name, "Avatar Queued");
            }

            //tell it to show
            complexProgressBar.Show();

            for (int i = 0; i < avatars.Length; i++)
            {
                GameObject avatar = avatars[i];

                //create avatar info
                VRCAvatar vrcavatar = new VRCAvatar();
                //set ID
                vrcavatar.ID = avatar.GetComponent<PipelineManager>().blueprintId;
                //update progress bar
                progressBars[i].description = "Building Avatar";
                progressBars[i].SetProgress(0.5f);
                await builder.BuildAndUpload(avatar, vrcavatar);
                //update progress bar
                progressBars[i].description = "Avatar Uploaded";
                progressBars[i].SetProgress(1f);

                //update the total progress bar
                totalProgressBar.SetProgress((float)(i + 1) / (float)avatars.Length);
            }

            //close progress bar
            complexProgressBar.Close();
        }

        
        /// <summary>
        /// This will check if a avatar is already associated with a ID
        /// </summary>
        /// <returns></returns>
        bool CheckIfAvatarIsLive(GameObject avatar)
        {
            //check to see if the pipeline manager has a ID
            if (avatar.GetComponent<PipelineManager>() != null)
            {
                if (avatar.GetComponent<PipelineManager>().blueprintId != "")
                {
                    return true;
                }
            }
            return false;
        }


    }
}
