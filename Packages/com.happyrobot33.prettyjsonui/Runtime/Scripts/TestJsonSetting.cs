using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TestJsonSetting : UdonSharpBehaviour
{
    public JSONManager jsonManager;
    public string stringSource = "";
    public TextAsset textAssetSource;

    public void sendSetString()
    {
        jsonManager.stringSource = stringSource;
    }

    public void sendSetTextAsset()
    {
        jsonManager.textAssetSource = textAssetSource;
    }
}
