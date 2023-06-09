using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Numerics;
using System;

//simple unity editor window to control the framerate of the editor
namespace HappysTools.FPSController
{
    public class FrameRateController : EditorWindow
    {
        private static Color _DefaultBackgroundColor;
        public static Color DefaultBackgroundColor
        {
            get
            {
                if (_DefaultBackgroundColor.a == 0)
                {
                    var method = typeof(EditorGUIUtility).GetMethod(
                        "GetDefaultBackgroundColor",
                        BindingFlags.NonPublic | BindingFlags.Static
                    );
                    _DefaultBackgroundColor = (Color)method.Invoke(null, null);
                }
                return _DefaultBackgroundColor;
            }
        }

        [MenuItem("VRC Packages/Happys Tools/FPS Controller")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(FrameRateController));
        }

        /// <summary> The target frame rate for the editor. </summary>
        int targetFrameRate = 60;

        /// <summary> The target refresh rate for the headset. </summary>
        /// <remarks> This is useful for worlds as it actually controls what fixedUpdate is </remarks>
        int targetHeadsetHZ = 90;

        struct Headset
        {
            public string name;
            public int hz;
        }

        Headset[] HeadsetPresets = new Headset[]
        {
            new Headset { name = "Oculus Rift", hz = 90 },
            new Headset { name = "Oculus Rift S", hz = 80 },
            new Headset { name = "Oculus Quest", hz = 72 },
            new Headset { name = "Oculus Quest Pro", hz = 90 },
            new Headset { name = "Oculus Quest 2 72Hz", hz = 72 },
            new Headset { name = "Oculus Quest 2 90Hz", hz = 90 },
            new Headset { name = "Oculus Quest 2 120Hz", hz = 120 },
            new Headset { name = "HTC Vive", hz = 90 },
            new Headset { name = "HTC Vive Pro", hz = 90 },
            new Headset { name = "HTC Vive Pro 2", hz = 120 },
            new Headset { name = "HTC Vive Cosmos", hz = 90 },
            new Headset { name = "Valve Index 120Hz", hz = 120 },
            new Headset { name = "Valve Index 144Hz", hz = 144 },
            new Headset { name = "Windows Mixed Reality 60Hz", hz = 60 },
            new Headset { name = "Windows Mixed Reality 90Hz", hz = 90 },
            new Headset { name = "Pimax 5K", hz = 90 },
            new Headset { name = "Pimax 8K", hz = 80 },
            new Headset { name = "Pico 4 72Hz", hz = 72 },
            new Headset { name = "Pico 4 90Hz", hz = 90 },
            new Headset { name = "Pico 4 Pro", hz = 90 },
        };

        void OnGUI()
        {
            GUILayout.Label("Frame Rate Controller", EditorStyles.boldLabel);
            Divider("FPS");

            GUILayout.Label(
                "Set the target frame rate for the editor. This will not affect builds.",
                EditorStyles.wordWrappedLabel
            );
            // if the target frame rate is -1, then it is unlocked
            var actualCurrentFrameRate =
                Application.targetFrameRate != -1
                    ? Application.targetFrameRate.ToString()
                    : "Un-Capped";
            GUILayout.Label(
                "Current Frame Rate: " + actualCurrentFrameRate,
                EditorStyles.wordWrappedLabel
            );
            targetFrameRate = EditorGUILayout.IntSlider(
                "Target Frame Rate",
                targetFrameRate,
                1,
                120
            );
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Frame Rate"))
            {
                Application.targetFrameRate = targetFrameRate;
            }
            if (GUILayout.Button("Un-Cap Frame Rate"))
            {
                Application.targetFrameRate = -1;
            }
            GUILayout.EndHorizontal();

            Divider("Headset");

            GUILayout.Label(
                "Set the target refresh rate for the headset. This will not affect builds. This is useful for worlds as it actually controls what fixedUpdate is in Udon",
                EditorStyles.wordWrappedLabel
            );
            GUILayout.Label(
                "Current Refresh Rate: "
                    + Time.fixedDeltaTime
                    + " ("
                    + (1f / Time.fixedDeltaTime)
                    + " Hz)",
                EditorStyles.wordWrappedLabel
            );
            targetHeadsetHZ = EditorGUILayout.IntSlider(
                "Target Refresh Rate",
                targetHeadsetHZ,
                1,
                144
            );
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Refresh Rate"))
            {
                Time.fixedDeltaTime = 1f / targetHeadsetHZ;
            }
            if (GUILayout.Button("Reset Refresh Rate"))
            {
                Time.fixedDeltaTime = 1f / 50f;
            }
            GUILayout.EndHorizontal();
            //display the headset presets
            GUILayout.Label("Headset Presets", EditorStyles.boldLabel);
            //determine what the preset count is divisible by
            int presetCount = HeadsetPresets.Length;
            int presetDivisibleBy = Smallest(presetCount);
            //Create a collumn for each presetDivisibleBy
            int collumnCount = presetCount / presetDivisibleBy;
            hover = false;
            for (int i = 0; i < collumnCount; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < presetDivisibleBy; j++)
                {
                    int index = i * presetDivisibleBy + j;
                    if (index < presetCount)
                    {
                        if (
                            hoverButton(
                                HeadsetPresets[index].name,
                                "Sets the refresh rate to " + HeadsetPresets[index].hz + "Hz"
                            )
                        )
                        {
                            Time.fixedDeltaTime = 1f / HeadsetPresets[index].hz;
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            drawHover();
        }

        string hoverText = "";
        bool hover = false;

        bool hoverButton(string text, string hoverTextIn)
        {
            GUILayout.BeginVertical();
            //create the button
            bool result = GUILayout.Button(text);
            //get the rect of the button
            Rect buttonRect = GUILayoutUtility.GetLastRect();
            //if the mouse is over the button
            if (buttonRect.Contains(Event.current.mousePosition))
            {
                //save the current hover text
                hoverText = hoverTextIn;
                hover = true;
            }
            GUILayout.EndVertical();

            return result;
        }

        void drawHover()
        {
            if (hover)
            {
                //draw a rect under the mouse
                GUIContent hoverContent = new GUIContent(hoverText);
                Rect hoverRect = new Rect(
                    Event.current.mousePosition.x - GUI.skin.label.CalcSize(hoverContent).x / 2,
                    Event.current.mousePosition.y + GUI.skin.label.CalcSize(hoverContent).y,
                    GUI.skin.label.CalcSize(hoverContent).x,
                    GUI.skin.label.CalcSize(hoverContent).y
                );
                EditorGUI.DrawRect(hoverRect, DefaultBackgroundColor);
                GUI.Label(hoverRect, hoverContent);

                //we need to repaint the inspector to update the hover location every frame
                Repaint();
            }
        }

        void Divider(string text = "")
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (text != "")
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;

                //get a rect based on the last rect and the text length
                lastRect = new Rect(
                    lastRect.x + lastRect.width / 2 - style.CalcSize(new GUIContent(text)).x / 2,
                    lastRect.y,
                    style.CalcSize(new GUIContent(text)).x,
                    lastRect.height
                );

                //create a filled rect of the inspector background color
                EditorGUI.DrawRect(lastRect, DefaultBackgroundColor);
                //overlap a label ontop of it
                GUI.Label(
                    new Rect(lastRect.x, lastRect.y, lastRect.width, lastRect.height),
                    text,
                    style
                );
            }
        }

        /// <summary> Returns the lowest divisible number </summary>
        public static int Smallest(int n)
        {
            // if divisible by 2
            if (n % 2 == 0)
                return 2;

            // iterate from 3 to sqrt(n)
            for (int i = 3; i * i <= n; i += 2)
            {
                if (n % i == 0)
                    return i;
            }

            return n;
        }
    }
}
