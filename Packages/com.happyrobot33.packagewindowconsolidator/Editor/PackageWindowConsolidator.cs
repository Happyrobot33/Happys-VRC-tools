﻿using UnityEditor;
using UnityEngine;
using System.IO;

/*
This consolidates other packages MenuItem entrys into a single dropdown menu for organization

VRC Packages
    Package
    AnotherPackagename
*/

namespace HappysTools.PackageWindowConsolidator
{
    //watch for changes to the project
    public class PackageWindowConsolidatorWatcher : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            string[] allImportedAssets = new string[0];
            foreach (var importedAsset in importedAssets)
            {
                //check to make sure it is in the Packages folder
                if (!importedAsset.StartsWith("Packages/"))
                {
                    continue;
                }
                //check to make sure it isnt us
                if (importedAsset.Contains("PackageWindowConsolidator"))
                {
                    continue;
                }
                //check to make sure it is a .cs file
                if (!importedAsset.EndsWith(".cs"))
                {
                    continue;
                }
                //check if the file has already been moved
                if (PackageWindowConsolidator.IsFileConsolidated(importedAsset))
                {
                    continue;
                }

                //save them to a new array
                System.Array.Resize(ref allImportedAssets, allImportedAssets.Length + 1);
                allImportedAssets[allImportedAssets.Length - 1] = importedAsset;
            }

            if (allImportedAssets.Length != 0)
            {
                //ask the user if they want to consolidate the packages
                if (
                    EditorUtility.DisplayDialog(
                        "Consolidate Packages",
                        "Would you like to consolidate the packages you just added/updated?",
                        "Yes",
                        "No",
                        DialogOptOutDecisionType.ForThisSession,
                        "PackageWindowConsolidator.ConsolidatePackages"
                    )
                )
                {
                    //consolidate the packages
                    PackageWindowConsolidator.ConsolidatePackages();
                }
            }
        }
    }

    public class PackageWindowConsolidator : MonoBehaviour
    {
        private const string NewDropdownName = "VRC Packages";

        [MenuItem(NewDropdownName + "/Consolidate Packages", false, -1000)]
        public static void ConsolidatePackages()
        {
            string currentWorkingPackage = "";
            try
            {
                AssetDatabase.StartAssetEditing();

                //get all the normal packages
                string[] packages = Directory.GetDirectories("Packages");

                //get all the tarball packages in the Packages folder
                string[] tarballs = Directory.GetFiles(
                    "Packages",
                    "*.tgz",
                    SearchOption.AllDirectories
                );
                foreach (string tarpackage in tarballs)
                {
                    //get the name of the package
                    string packageName = Path.GetFileNameWithoutExtension(tarpackage);
                    //get the project manifest
                    string manifest = File.ReadAllText("Packages/manifest.json");
                    //find the index containing the package name
                    int packageIndex = manifest.IndexOf(packageName);
                    int lineStart = manifest.LastIndexOf("\n", packageIndex) + 1;
                    //get the entire ine containing the package name
                    string packageLine = manifest.Substring(
                        lineStart,
                        manifest.IndexOf("\n", packageIndex) - lineStart
                    );
                    //find the actual package domain name
                    string packageDomain = packageLine.Substring(packageLine.IndexOf("\"") + 1);
                    packageDomain = packageDomain.Substring(0, packageDomain.IndexOf("\""));

                    //now that we have the package domain, we can search for it in the library folder
                    string[] libraryPackages = Directory.GetDirectories("Library/PackageCache");
                    foreach (string libraryPackage in libraryPackages)
                    {
                        //if the package domain matches the library package domain
                        if (libraryPackage.Contains(packageDomain))
                        {
                            //add the package to the packages list
                            System.Array.Resize(ref packages, packages.Length + 1);
                            packages[packages.Length - 1] = libraryPackage;
                        }
                    }
                }

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
                    string[] files = Directory.GetFiles(
                        package,
                        "*.cs",
                        SearchOption.AllDirectories
                    );
                    //ensure that this script is not included in the list
                    files = System.Array.FindAll(
                        files,
                        s => !s.EndsWith("PackageWindowConsolidator.cs")
                    );

                    //loop through all the files
                    foreach (string file in files)
                    {
                        //get the index of the file in the list
                        int fileIndex = System.Array.IndexOf(files, file);

                        //update the progress bar
                        EditorUtility.DisplayProgressBar(
                            "Consolidating Packages",
                            "Consolidating " + packageName,
                            fileIndex / (float)files.Length
                        );

                        //read the file
                        string[] lines = File.ReadAllLines(file);

                        //loop through all the lines (Stage 1)
                        foreach (string line in lines)
                        {
                            //check to see if the MenuItem is based on a variable. if it is, resolve the variable
                            if (line.Contains("[MenuItem(") && !line.Contains("/"))
                            {
                                //get the variable name
                                string variableName = line.Substring(
                                    line.IndexOf("[MenuItem(") + 10
                                );
                                variableName = variableName.Substring(
                                    0,
                                    variableName.IndexOf(")]")
                                );

                                //check if there is parameters
                                if (variableName.Contains(","))
                                {
                                    variableName = variableName.Substring(
                                        0,
                                        variableName.IndexOf(",")
                                    );
                                }

                                //Debug.Log(variableName);

                                string resolvedPath = "";

                                //find the variable in the file
                                foreach (string variableLine in lines)
                                {
                                    //if the line contains the variable name
                                    if (variableLine.Contains(variableName + " = "))
                                    {
                                        //get the value of the variable
                                        string variableValue = variableLine.Substring(
                                            variableLine.IndexOf(variableName + " = ")
                                                + variableName.Length
                                                + 3
                                        );
                                        //Debug.Log(variableValue);

                                        //check if its already updated
                                        if (variableValue.Contains(NewDropdownName + "/"))
                                        {
                                            continue;
                                        }

                                        string cleanedVariableValue = variableValue;

                                        //check if the value has either Tools or Window in it
                                        if (variableValue.StartsWith("\"Tools/") || variableValue.StartsWith("\"Window/"))
                                        {
                                            //remove the initial Tools/ or Window/
                                            cleanedVariableValue = variableValue.Substring(
                                                variableValue.IndexOf("/") + 1
                                            );

                                            cleanedVariableValue = "\"" + cleanedVariableValue;
                                        }

                                        //Debug.Log(cleanedVariableValue);

                                        /* //remove the quotes and semicolon
                                        resolvedPath = variableValue
                                            .Replace("\"", "")
                                            .Replace(";", ""); */

                                        //update the value of the variable
                                        string newVariableValue = string.Format("\"{0}/\" + {1}", NewDropdownName, cleanedVariableValue);
                                        //Debug.Log(newVariableValue);
                                        File.WriteAllText(
                                            file,
                                            File.ReadAllText(file).Replace(variableLine, variableLine.Replace(variableValue, newVariableValue))
                                        );

                                        //reimport the file
                                        AssetDatabase.ImportAsset(file);

                                        //read the file again
                                        lines = File.ReadAllLines(file);
                                    }
                                }
                            }
                        }

                        //loop through all the lines (Stage 2)
                        foreach (string line in lines)
                        {
                            //if the line contains a MenuItem attribute and isnt already in the VRC Packages dropdown
                            if (line.Contains("[MenuItem") && !line.Contains(NewDropdownName))
                            {
                                //replace the Window/ with VRC Packages/
                                if (line.Contains("[MenuItem(\"Window/"))
                                {
                                    File.WriteAllText(
                                        file,
                                        File.ReadAllText(file)
                                            .Replace(
                                                "[MenuItem(\"Window/",
                                                "[MenuItem(\"" + NewDropdownName + "/"
                                            )
                                    );
                                }
                                else if (line.Contains("[MenuItem(\"Tools/"))
                                {
                                    File.WriteAllText(
                                        file,
                                        File.ReadAllText(file)
                                            .Replace(
                                                "[MenuItem(\"Tools/",
                                                "[MenuItem(\"" + NewDropdownName + "/"
                                            )
                                    );
                                }
                                else if (IsMenuItemMovable(line)) //make sure the menu item is not in some of the other dropdowns
                                {
                                    //Debug.Log(line);
                                    //check if it is a variable
                                    if (!line.Contains("MenuItem(\")"))
                                    {
                                        continue;
                                    }
                                    //get the path of the MenuItem
                                    string path = line.Substring(line.IndexOf("\"") + 1);
                                    path = path.Substring(0, path.IndexOf("\""));

                                    //add the package name to the path
                                    path = NewDropdownName + "/" + path;

                                    //replace the path in the line
                                    string newLine = line.Replace(
                                        line.Substring(
                                            line.IndexOf("\"") + 1,
                                            line.LastIndexOf("\"") - line.IndexOf("\"") - 1
                                        ),
                                        path
                                    );

                                    //write the new line to the file
                                    File.WriteAllText(
                                        file,
                                        File.ReadAllText(file).Replace(line, newLine)
                                    );
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                EditorUtility.DisplayDialog(
                    "Error",
                    "An error occured while consolidating packages. Please check the console for more information. The error occured while consolidating "
                        + currentWorkingPackage
                        + ". You might need to reimport this package",
                    "Ok"
                );
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
            string[] blockedTabs = new string[]
            {
                "File",
                "Edit",
                "Assets",
                "GameObject",
                "Component",
                "VRChat SDK",
                "Help",
                "Jobs"
            };

            foreach (string tab in blockedTabs)
            {
                if (line.Contains("[MenuItem(\"" + tab + "/"))
                {
                    return false;
                }
            }

            return true;
        }

        //all in one function to check if we have already consolidated a file
        public static bool IsFileConsolidated(string file)
        {
            string[] lines = File.ReadAllLines(file);

            bool containsMenuItem = false;

            foreach (string line in lines)
            {
                if (line.Contains("[MenuItem"))
                {
                    containsMenuItem = true;
                }

                if (line.Contains("[MenuItem(\"" + NewDropdownName + "/"))
                {
                    return true;
                }
            }

            //if the file does not contain a MenuItem attribute, we can assume that it is not a script that we need to consolidate
            if (!containsMenuItem)
            {
                return true;
            }

            return false;
        }

        //adds the ability to clear the session preference to always consolidate packages
        [MenuItem(NewDropdownName + "/Disable Automatic Consolidation", false, -1000)]
        public static void DisableAutomaticConsolidation()
        {
            SessionState.SetBool("PackageWindowConsolidator.ConsolidatePackages", false);
        }
    }
}
