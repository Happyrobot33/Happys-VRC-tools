using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HappysTools.AtlasGenerator
{
    class AtlasGenerator : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            //gather up all changed textures
            List<Texture2D> changedTextures = new List<Texture2D>();
            foreach (string assetPath in importedAssets)
            {
                //load the asset
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                //check if the asset is a texture
                if (asset is Texture2D)
                {
                    //add the texture to the list
                    changedTextures.Add((Texture2D)asset);
                }
            }

            //if none, return
            if (changedTextures.Count == 0)
            {
                return;
            }

            List<AtlasInformation> atlasInformationList = AtlasInformation.GetAllAssociatedAtlas(changedTextures.ToArray());
            //rebuild them
            foreach (AtlasInformation atlasInformation in atlasInformationList)
            {
                atlasInformation.Rebuild();
            }
        }
    }
}
