/*
Created by: Happyrobot33
Date: 4/12/2022
Version: 1.0
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InspectorCubeRender : EditorWindow
{
    private static int rez = 30;
    private static bool[,] screen = new bool[rez, rez];
    [MenuItem("VRC Packages/Editor Games/Cube Renderer")]
    public static void ShowWindow()
    {
        //create a new window
        EditorWindow window = EditorWindow.GetWindow(typeof(InspectorCubeRender));
        //name the window
        window.titleContent = new GUIContent("Cube Renderer");
        window.Show();
    }

    public void OnGUI()
    {
        GenerateSpinningCube();
        DrawScreen();   
    }

    public void GenerateSpinningCube()
    {
        int cubeSize = rez / 2;
        int cubeCenter = rez / 2;

        for (int x = 0; x < rez; x++)
        {
            for (int y = 0; y < rez; y++)
            {
                if (x < cubeSize && y < cubeSize)
                {
                    screen[x, y] = true;
                }
                else if (x > cubeCenter && y > cubeCenter)
                {
                    screen[x, y] = true;
                }
                else
                {
                    screen[x, y] = false;
                }
            }
        }
    }

    public void DrawScreen()
    {
        //get inspector window width
        float width = Mathf.Max(position.width, position.height);

        //create a container of specified size
        GUILayout.BeginArea(new Rect(0, 0, width, width));

        GUILayout.BeginVertical();
        //do for each collumn
        for (int y = 0; y < screen.GetLength(1); y++)
        {
            GUILayout.BeginHorizontal();
            //do for each row
            for (int x = 0; x < screen.GetLength(0); x++)
            {
                GUI.skin.box.padding = new RectOffset(0, 0, 0, 0);
                GUI.skin.box.margin = new RectOffset(0, 0, 0, 0);
                //render box, white if true, black if false
                Color color = screen[x, y] ? Color.white : Color.black;
                GUILayout.Box(CreatePixelTexture(width / screen.GetLength(0), color), GUILayout.Width(width / screen.GetLength(0)), GUILayout.Height(width / screen.GetLength(0)));
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginVertical();

        //end container
        GUILayout.EndArea();
    }

    //create pixel texture
    public Texture CreatePixelTexture(float insize, Color color)
    {
        Texture2D texture = new Texture2D((int)insize, (int)insize);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        return texture;
    }
}