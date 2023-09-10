#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace HappysTools.AvatarBulkUploader
{
    public class ComplexProgressBar : EditorWindow
    {
        private readonly List<ProgressBar> progressBars = new List<ProgressBar>();

        public void OnGUI()
        {
            float height = 0;
            foreach (ProgressBar progressBar in progressBars)
            {
                height += progressBar.Render();
            }

            //Auto close the window if there are no progress bars left
            if (progressBars.Count == 0)
            {
                Close();
                DestroyImmediate(this);
            }

            //this is hardcoded because I don't know how to get the height of the title bar
            height += 20;

            this.maxSize = new Vector2(500, height);
            this.minSize = new Vector2(500, height);
        }

        /// <summary> Registers a new progress bar and returns it </summary>
        /// <param name="title">The title of the progress bar</param>
        /// <param name="description">The description of the progress bar</param>
        /// <returns>The progress bar that was just created</returns>
        public ProgressBar RegisterNewProgressBar(string title, string description)
        {
            ProgressBar progressBar = new ProgressBar()
            {
                title = title,
                description = description,
                progress = 0,
                parent = this
            };
            this.progressBars.Add(progressBar);
            Repaint();
            return progressBar;
        }

        /// <summary> Removes a progress bar from the list of progress bars </summary>
        /// <param name="progressBar">The progress bar to remove</param>
        public void RemoveProgressBar(ProgressBar progressBar)
        {
            progressBars.Remove(progressBar);
        }
    }

    public class ProgressBar
    {
        public string title;
        public string description;
        public float height;
        public float progress;
        internal ComplexProgressBar parent;

        public void Finish()
        {
            parent.RemoveProgressBar(this);
        }

        public float Render()
        {
            GUIContent titleContent = new GUIContent(title);
            GUIContent descriptionContent = new GUIContent(description);
            progress = Mathf.Round(progress * 100) / 100;
            GUIContent progressContent = new GUIContent((progress * 100).ToString() + "%");
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
            GUIStyle descriptionStyle = new GUIStyle(EditorStyles.label);
            GUIStyle progressStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter
            };
            progressStyle.normal.textColor = Color.black;

            EditorGUILayout.LabelField(titleContent, titleStyle);
            if (description != "")
            {
                EditorGUILayout.LabelField(descriptionContent, descriptionStyle);
            }

            //create the container rect
            Rect containerRect = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            //create the fill rect
            Rect fillRect = new Rect(containerRect);
            fillRect.width *= progress;

            //draw the empty rect
            EditorGUI.DrawRect(containerRect, Color.gray);
            //draw the fill rect
            EditorGUI.DrawRect(fillRect, Color.green);

            //end the horizontal layout
            EditorGUILayout.LabelField(progressContent, progressStyle);
            EditorGUILayout.EndHorizontal();
            float height = titleStyle.CalcHeight(titleContent, containerRect.width);
            if (description != "")
            {
                height += descriptionStyle.CalcHeight(descriptionContent, containerRect.width);
            }
            height += progressStyle.CalcHeight(progressContent, containerRect.width);
            height += 10;
            TrySetHeight(height);
            return this.height;
        }

        private void TrySetHeight(float height)
        {
            //check to see if layout event
            if (Event.current.type == EventType.Layout)
            {
                this.height = height;
            }
        }

        public void SetProgress(float progress)
        {
            this.progress = progress;
            parent.Repaint();
        }
    }
}

#endif
