using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.VersionControl;
using UnityEngine.UI;

namespace HappysTools.AtlasGenerator
{
    [CreateAssetMenu(fileName = "AtlasInformation", menuName = "VRC Packages/Atlas Generator/Atlas Definition")]
    public class AtlasInformation : ScriptableObject
    {
        public List<Texture2D> Textures;
        public Vector2Int Grid;
        public Texture2D GeneratedTexture;

        public Vector2Int AtlasSize
        {
            get
            {
                return new Vector2Int(Grid.x * MaxTextureSize.x, Grid.y * MaxTextureSize.y);
            }
        }

        public Vector2Int MaxTextureSize
        {
            get
            {
                //get the maximum texture size
                int maxSize = 0;
                foreach (Texture2D texture in Textures)
                {
                    if (texture.width > maxSize)
                    {
                        maxSize = texture.width;
                    }

                    if (texture.height > maxSize)
                    {
                        maxSize = texture.height;
                    }
                }

                return new Vector2Int(maxSize, maxSize);
            }
        }

        //menu item to create a new atlas information asset based on selected textures
        [MenuItem("Assets/Create/VRC Packages/Atlas Generator/Create Atlas From Selected Textures")]
        public static void CreateAtlasInformation()
        {
            //get the selected textures
            Object[] selectedTextures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);

            //create a new atlas information asset
            AtlasInformation atlasInformation = CreateInstance<AtlasInformation>();

            //set the textures
            atlasInformation.Textures = new List<Texture2D>();
            foreach (Object selectedTexture in selectedTextures)
            {
                atlasInformation.Textures.Add((Texture2D)selectedTexture);
            }

            //set the grid, prefering a square grid
            int gridX = Mathf.CeilToInt(Mathf.Sqrt(atlasInformation.Textures.Count));
            int gridY = Mathf.CeilToInt((float)atlasInformation.Textures.Count / gridX);
            atlasInformation.Grid = new Vector2Int(gridX, gridY);

            //save the asset
            string assetPath = AssetDatabase.GetAssetPath(selectedTextures[0]);
            string newAssetPath = Path.ChangeExtension(assetPath, "asset");
            AssetDatabase.CreateAsset(atlasInformation, newAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Create/VRC Packages/Atlas Generator/Create Atlas From Selected Textures", true)]
        public static bool CreateAtlasInformationValidation()
        {
            //get the selected textures
            Object[] selectedTextures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);

            //check if we have any textures selected
            return selectedTextures.Length > 0;
        }

        /// <summary>
        /// Check if the texture is in the atlas
        /// </summary>
        /// <param name="texture">The texture to check</param>
        /// <returns>True if the texture is in the atlas, false otherwise</returns>
        public bool IsInAtlas(params Texture2D[] texture)
        {
            //loop through all the textures
            foreach (Texture2D tex in texture)
            {
                //check if the texture is in the atlas
                if (Textures.Contains(tex))
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateTexture(Texture2D tex)
        {
            //get our GUID
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));

            //convert to png
            byte[] bytes = tex.EncodeToPNG();

            //get the path of this asset
            string assetPath = AssetDatabase.GetAssetPath(this);
            //string newAssetPath = Path.ChangeExtension(assetPath, "png");
            string newAssetPath = Path.Combine(Path.GetDirectoryName(assetPath), guid + ".png");

            //if we already have a endpoint, use that path instead
            if (GeneratedTexture != null)
            {
                newAssetPath = AssetDatabase.GetAssetPath(GeneratedTexture);
            }

            //write the texture
            System.IO.File.WriteAllBytes(newAssetPath, bytes);
            //import the asset
            AssetDatabase.ImportAsset(assetPath);
            AssetDatabase.Refresh();

            //make sure we save it to the asset
            GeneratedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(newAssetPath);
        }

        public void Rebuild()
        {
            Debug.Log("Rebuilding atlas: " + name);

            //ensure the textures are readable
            foreach (Texture2D texture in Textures)
            {
                //check if its valid
                if (texture == null)
                {
                    continue;
                }

                //get the path of the asset
                string assetPath = AssetDatabase.GetAssetPath(texture);

                //make sure the texture is readable
                TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (!textureImporter.isReadable)
                {
                    Debug.Log("Setting texture to readable: " + assetPath);
                    textureImporter.isReadable = true;
                    AssetDatabase.ImportAsset(assetPath);
                }
            }

            Texture2D atlas = new Texture2D(AtlasSize.x, AtlasSize.y);
            //make the default color black
            for (int y = 0; y < atlas.height; y++)
            {
                for (int x = 0; x < atlas.width; x++)
                {
                    atlas.SetPixel(x, y, Color.black);
                }
            }

            for (int y = 0; y < Grid.y; y++)
            {
                for (int x = 0; x < Grid.x; x++)
                {
                    //get the texture
                    Texture2D texture = GetTexture(y, x);

                    //check if its valid
                    if (texture == null)
                    {
                        continue;
                    }

                    RenderTexture rt = RenderTexture.GetTemporary(MaxTextureSize.x, MaxTextureSize.y);
                    Graphics.Blit(texture, rt);
                    RenderTexture active = RenderTexture.active;
                    RenderTexture.active = rt;

                    atlas.ReadPixels(new Rect(0, 0, MaxTextureSize.x, MaxTextureSize.y), x * MaxTextureSize.x, y * MaxTextureSize.y);

                    RenderTexture.active = active;
                    RenderTexture.ReleaseTemporary(rt);
                }
            }
            atlas.Apply();

            //GENERATION PROCESSING
            UpdateTexture(atlas);
        }

        private Texture2D GetTexture(int y, int x)
        {
            //reference y from top to bottom
            int newY = Grid.y - y - 1;
            int index = newY * Grid.x + x;
            //check if the index is valid
            if (index >= Textures.Count)
            {
                return null;
            }
            return Textures[index];
        }

        public static List<AtlasInformation> GetAllAssociatedAtlas(params Texture2D[] texture)
        {
            //get all the atlas information assets
            List<AtlasInformation> atlasInformationList = GetAllAtlasInformation();

            //create a list to store the associated atlas information
            List<AtlasInformation> associatedAtlasInformationList = new List<AtlasInformation>();

            //loop through all the atlas information assets
            foreach (AtlasInformation atlasInformation in atlasInformationList)
            {
                //check if the texture is in the atlas
                if (atlasInformation.IsInAtlas(texture))
                {
                    //add the atlas information to the list
                    associatedAtlasInformationList.Add(atlasInformation);
                }
            }

            return associatedAtlasInformationList;
        }

        public static List<AtlasInformation> GetAllAtlasInformation()
        {
            //get all the atlas information assets
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(AtlasInformation).Name));

            //create a list to store the atlas information
            List<AtlasInformation> atlasInformationList = new List<AtlasInformation>();

            //loop through all the atlas information assets
            foreach (string guid in guids)
            {
                //get the asset path
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                //load the asset
                AtlasInformation atlasInformation = AssetDatabase.LoadAssetAtPath<AtlasInformation>(assetPath);

                //add the atlas information to the list
                atlasInformationList.Add(atlasInformation);
            }

            return atlasInformationList;
        }
    }

    //custom inspector for the atlas information
    [CustomEditor(typeof(AtlasInformation))]
    public class AtlasInformationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            AtlasInformation atlasInformation = (AtlasInformation)target;

            //draw the default inspector
            DrawDefaultInspector();

            //display the total size of the atlas
            EditorGUILayout.LabelField("Total Raw Size", string.Format("{0}x{1}", atlasInformation.AtlasSize.x, atlasInformation.AtlasSize.y));


            //draw the textures in the grid
            GUIStyle imageStyle = new GUIStyle(GUI.skin.box);
            imageStyle.alignment = TextAnchor.MiddleCenter;
            //disable padding and margins
            imageStyle.padding = new RectOffset(0, 0, 0, 0);
            imageStyle.margin = new RectOffset(0, 0, 0, 0);

            //calculate the aspect ratio
            float aspectRatio = (float)atlasInformation.Grid.x / atlasInformation.Grid.y;

            //make sure the aspect ratio is valid
            if (aspectRatio == 0 || float.IsNaN(aspectRatio) || float.IsInfinity(aspectRatio))
            {
                aspectRatio = 1;
            }

            //get a rectangle to draw the images in
            Rect rect = GUILayoutUtility.GetAspectRect(aspectRatio);

            //split out the rectangle into the grid
            for (int y = 0; y < atlasInformation.Grid.y; y++)
            {
                for (int x = 0; x < atlasInformation.Grid.x; x++)
                {
                    //calculate the position of the image
                    Rect imageRect = new Rect(rect.x + x * rect.width / atlasInformation.Grid.x, rect.y + y * rect.height / atlasInformation.Grid.y, rect.width / atlasInformation.Grid.x, rect.height / atlasInformation.Grid.y);

                    //draw the image
                    if (atlasInformation.Textures.Count > y * atlasInformation.Grid.x + x)
                    {
                        GUI.Box(imageRect, atlasInformation.Textures[y * atlasInformation.Grid.x + x], imageStyle);
                    }
                }
            }

            //add a button to rebuild the atlas
            if (GUILayout.Button("Force Rebuild Atlas"))
            {
                atlasInformation.Rebuild();
            }
        }
    }
}