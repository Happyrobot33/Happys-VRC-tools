using UnityEditor;
using UnityEngine;
using System.IO;

/*
This consolidates other packages MenuItem entrys into a single dropdown menu for organization

VRC Packages
    Package
    AnotherPackagename
*/

public class PackageWindowConsolidator : MonoBehaviour
{
    [MenuItem("VRC Packages/Consolidate Packages", false, -1000)]
    public static void ConsolidatePackages()
    {
        string currentWorkingPackage = "";
        try
        {
            AssetDatabase.StartAssetEditing();

            //get all the packages
            string[] packages = Directory.GetDirectories("Packages");

            //loop through all the packages
            foreach (string package in packages)
            {
                string packageName = Path.GetFileName(package);

                currentWorkingPackage = packageName;

                //filter out all com.vrchat.* packages
                if (packageName.StartsWith("com.vrchat."))
                {
                    continue;
                }

                //find every .cs file in the package
                string[] files = Directory.GetFiles(package, "*.cs", SearchOption.AllDirectories);
                //ensure that this script is not included in the list
                files = System.Array.FindAll(files, s => !s.EndsWith("PackageWindowConsolidator.cs"));

                //loop through all the files
                foreach (string file in files)
                {
                    //get the index of the file in the list
                    int fileIndex = System.Array.IndexOf(files, file);

                    //update the progress bar
                    EditorUtility.DisplayProgressBar("Consolidating Packages", "Consolidating " + packageName, fileIndex / (float)files.Length);

                    //read the file
                    string[] lines = File.ReadAllLines(file);

                    //loop through all the lines (Stage 1)
                    foreach (string line in lines)
                    {
                        //check to see if the MenuItem is based on a variable. if it is, resolve the variable
                        if (line.Contains("[MenuItem(") && !line.Contains("/"))
                        {
                            //get the variable name
                            string variableName = line.Substring(line.IndexOf("[MenuItem(") + 10);
                            variableName = variableName.Substring(0, variableName.IndexOf(")]"));

                            string resolvedPath = "";

                            //find the variable in the file
                            foreach (string variableLine in lines)
                            {
                                //if the line contains the variable name
                                if (variableLine.Contains(variableName + " = "))
                                {
                                    //get the value of the variable
                                    string variableValue = variableLine.Substring(variableLine.IndexOf(variableName + " = ") + variableName.Length + 3);

                                    //remove the quotes and semicolon
                                    resolvedPath = variableValue.Replace("\"", "").Replace(";", "");
                                }
                            }

                            //if the path was resolved
                            if (resolvedPath != "")
                            {
                                //replace the variable with the resolved path
                                File.WriteAllText(file, File.ReadAllText(file).Replace("[MenuItem(" + variableName + ")]", "[MenuItem(\"" + resolvedPath + "\")]"));

                                //reimport the file
                                AssetDatabase.ImportAsset(file);

                                //read the file again
                                lines = File.ReadAllLines(file);
                            }
                        }
                    }

                    //loop through all the lines (Stage 2)
                    foreach (string line in lines)
                    {
                        //if the line contains a MenuItem attribute and isnt already in the VRC Packages dropdown
                        if (line.Contains("[MenuItem") && !line.Contains("VRC Packages"))
                        {
                            //replace the Window/ with VRC Packages/
                            if (line.Contains("[MenuItem(\"Window/"))
                            {
                                File.WriteAllText(file, File.ReadAllText(file).Replace("[MenuItem(\"Window/", "[MenuItem(\"VRC Packages/"));
                            }
                            else if (IsMenuItemMovable(line)) //make sure the menu item is not in some of the other dropdowns
                            {
                                //get the path of the MenuItem
                                string path = line.Substring(line.IndexOf("\"") + 1);
                                path = path.Substring(0, path.IndexOf("\""));

                                //add the package name to the path
                                path = "VRC Packages/" + path;

                                //replace the path in the line
                                string newLine = line.Replace(line.Substring(line.IndexOf("\"") + 1, line.LastIndexOf("\"") - line.IndexOf("\"") - 1), path);

                                //write the new line to the file
                                File.WriteAllText(file, File.ReadAllText(file).Replace(line, newLine));
                            }
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            EditorUtility.DisplayDialog("Error", "An error occured while consolidating packages. Please check the console for more information. The error occured while consolidating " + currentWorkingPackage + ". You might need to reimport this package", "Ok");
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            EditorUtility.ClearProgressBar();
        }
    }

    //as long as the menu item is either at the top level or is in Window, we can move it
    public static bool IsMenuItemMovable(string line)
    {
        string[] blockedTabs = new string[] { "File", "Edit", "Assets", "GameObject", "Component", "VRChat SDK", "Help", "Jobs" };

        foreach (string tab in blockedTabs)
        {
            if (line.Contains("[MenuItem(\"" + tab + "/"))
            {
                return false;
            }
        }

        return true;
    }
}
