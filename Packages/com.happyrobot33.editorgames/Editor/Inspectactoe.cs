/*
Created by: Happyrobot33
Date: 4/12/2022
Version: 1.0
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace HappysTools.EditorGames.Inspecttactoe
{
    public class Inspectactoe : EditorWindow
    {
        public int[,] board = new int[3, 3]; //0 is empty, 1 is X, 2 is O
        public bool isPlayersTurn = true;

        [MenuItem("VRC Packages/Happys Tools/Editor Games/Inspectactoe")]
        public static void ShowWindow()
        {
            //create a new window
            EditorWindow window = EditorWindow.GetWindow(typeof(Inspectactoe));
            window.Show();
        }

        private void OnGUI()
        {
            var centeredStyle = new GUIStyle(GUI.skin.GetStyle("BoldLabel"));
            centeredStyle.alignment = TextAnchor.UpperCenter;
            centeredStyle.fontSize = 30;
            GUILayout.Label("Inspectactoe v1.0", centeredStyle);
            GUILayout.Label(
                "Yes! You are seeing this right, completely functional tic-tac-toe in an editor window! Dont ask why, just play!",
                EditorStyles.wordWrappedLabel
            );
            GUILayout.Label("Credit: Happyrobot33", EditorStyles.wordWrappedLabel);

            //button to reset / start a new game
            if (GUILayout.Button("Reset / Start New Game"))
            {
                ResetBoard();
            }

            if (checkForWin() == "Player")
            {
                GUILayout.Label("Player wins!");
                drawBoard();
            }
            else if (checkForWin() == "Computer")
            {
                GUILayout.Label("Computer wins!");
                drawBoard();
            }
            else if (checkForWin() == "Draw")
            {
                GUILayout.Label("Draw!");
                drawBoard();
            }
            else
            {
                drawBoard();
            }
        }

        public string checkForWin()
        {
            //create a array with all the possible winning patterns
            bool[,,] winningPositions = new bool[9, 3, 3];

            //horizontal
            winningPositions[0, 0, 0] = true;
            winningPositions[0, 0, 1] = true;
            winningPositions[0, 0, 2] = true;
            winningPositions[1, 1, 0] = true;
            winningPositions[1, 1, 1] = true;
            winningPositions[1, 1, 2] = true;
            winningPositions[2, 2, 0] = true;
            winningPositions[2, 2, 1] = true;
            winningPositions[2, 2, 2] = true;

            //vertical
            winningPositions[3, 0, 0] = true;
            winningPositions[3, 1, 0] = true;
            winningPositions[3, 2, 0] = true;
            winningPositions[4, 0, 1] = true;
            winningPositions[4, 1, 1] = true;
            winningPositions[4, 2, 1] = true;
            winningPositions[5, 0, 2] = true;
            winningPositions[5, 1, 2] = true;
            winningPositions[5, 2, 2] = true;

            //diagonal
            //top right to bottom left
            winningPositions[6, 0, 0] = true;
            winningPositions[6, 1, 1] = true;
            winningPositions[6, 2, 2] = true;
            //top left to bottom right
            winningPositions[7, 0, 2] = true;
            winningPositions[7, 1, 1] = true;
            winningPositions[7, 2, 0] = true;

            //check for a win
            for (int i = 0; i < 9; i++)
            {
                int count = 0;
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (winningPositions[i, j, k] == true)
                        {
                            if (board[j, k] == 1)
                            {
                                count++;
                            }
                            else if (board[j, k] == 2)
                            {
                                count--;
                            }
                        }
                    }
                }

                if (count == 3)
                {
                    return "Player";
                }
                else if (count == -3)
                {
                    return "Computer";
                }
            }

            bool boardFull = true;
            //check for draw
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == 0)
                    {
                        boardFull = false;
                    }
                }
            }

            if (boardFull)
            {
                return "Draw";
            }
            else
            {
                return "";
            }
        }

        public void ResetBoard()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    board[i, j] = 0;
                }
            }

            isPlayersTurn = true;
        }

        public void drawBoard()
        {
            if (isPlayersTurn)
            {
                GUILayout.Label("Player's Turn", EditorStyles.boldLabel);
            }
            else
            {
                GUILayout.Label("Computer's Turn", EditorStyles.boldLabel);
            }

            //draw a 3 x 3 grid of boolean buttons
            for (int i = 0; i < 3; i++)
            {
                //start row
                GUILayout.BeginHorizontal();
                for (int j = 0; j < 3; j++)
                {
                    string cellStatus = "";

                    //determine the status of the cell
                    switch (board[i, j])
                    {
                        case 0:
                            cellStatus = "_";
                            break;
                        case 1:
                            cellStatus = "X";
                            break;
                        case 2:
                            cellStatus = "O";
                            break;
                    }

                    //check if the cell is empty
                    if (board[i, j] == 0)
                    {
                        //coloring
                        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                        //set the button background
                        buttonStyle.normal.background = MakeBackgroundTexture(1, 1, Color.white);
                        buttonStyle.padding = new RectOffset(0, 0, 0, 0);
                        buttonStyle.margin = new RectOffset(0, 0, 0, 0);
                        buttonStyle.border = new RectOffset(0, 0, 0, 0);

                        if (GUILayout.Button(cellStatus, buttonStyle) && checkForWin() == "")
                        {
                            board[i, j] = 1;
                            isPlayersTurn = false;
                        }
                        else if (
                            Random.value < 0.5 && isPlayersTurn == false && checkForWin() == ""
                        )
                        {
                            board[i, j] = 2;
                            isPlayersTurn = true;
                        }
                    }
                    else
                    {
                        //turn the GUI interaction off, draw the button, and turn the GUI interaction back on
                        GUI.enabled = false;

                        //coloring
                        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                        Texture2D color = new Texture2D(1, 1);
                        if (board[i, j] == 1)
                        {
                            color = MakeBackgroundTexture(1, 1, Color.red);
                        }
                        else if (board[i, j] == 2)
                        {
                            color = MakeBackgroundTexture(1, 1, Color.blue);
                        }
                        //set the button background
                        buttonStyle.normal.background = color;
                        buttonStyle.padding = new RectOffset(0, 0, 0, 0);
                        buttonStyle.margin = new RectOffset(0, 0, 0, 0);
                        buttonStyle.border = new RectOffset(0, 0, 0, 0);

                        GUILayout.Button(cellStatus, buttonStyle);
                        GUI.enabled = true;
                    }
                }
                GUILayout.EndHorizontal();
            }
        }

        private Texture2D MakeBackgroundTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D backgroundTexture = new Texture2D(width, height);

            backgroundTexture.SetPixels(pixels);
            backgroundTexture.Apply();

            return backgroundTexture;
        }
    }
}
