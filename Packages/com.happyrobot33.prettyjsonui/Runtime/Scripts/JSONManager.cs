
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using Koyashiro.UdonJson;

public class JSONManager : UdonSharpBehaviour
{
    UdonJsonValue json;
    public int depthLimit = 10;
    public float indentSpacing = 0.1f;
    public string stringSource = "";
    public TextAsset textAssetSource;
    public GameObject keyValuePairPrefab;

    void Start()
    {
        if (stringSource != "")
        {
            SetJson(stringSource);
        }
        else if (textAssetSource != null)
        {
            SetJson(textAssetSource);
        }

        initializeHierarchy(json);
    }

    // Recursive function to create the hierarchy system
    [RecursiveMethod]
    public void initializeHierarchy(UdonJsonValue curJson, GameObject root = null, int currentDepth = 0)
    {
        if (currentDepth > depthLimit)
        {
            Debug.LogError("Depth limit reached, aborting");
            return;
        }

        if (root == null)
        {
            root = gameObject;
        }

        string[] keys = curJson.Keys();
        foreach (string key in keys)
        {
            // Create the object
            GameObject obj = Instantiate(keyValuePairPrefab, root.transform);
            switch (curJson.GetValue(key).GetKind())
            {
                case UdonJsonValueKind.Array:
                    string arrayString = "";
                    for (int i = 0; i < curJson.GetValue(key).Count(); i++)
                    {
                        UdonJsonValue element = curJson.GetValue(key).GetValue(i);
                        arrayString += element.AsString();
                        if (i != curJson.GetValue(key).Count() - 1)
                        {
                            arrayString += ", ";
                        }
                    }
                    obj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = key + " [" + arrayString + "] ";
                    break;
                case UdonJsonValueKind.String:
                    obj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = key + ": " + curJson.GetValue(key).AsString();
                    break;
                case UdonJsonValueKind.Number:
                    obj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = key + ": " + curJson.GetValue(key).AsNumber();
                    break;
                case UdonJsonValueKind.True:
                case UdonJsonValueKind.False:
                    obj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = key + ": " + curJson.GetValue(key).AsBool();
                    break;
                case UdonJsonValueKind.Null:
                    obj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = key + ": " + curJson.GetValue(key).AsNull();
                    break;
                case UdonJsonValueKind.Object:
                    obj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = key + " {" + curJson.GetValue(key).Keys().Length + "}";
                    initializeHierarchy(curJson.GetValue(key), obj, currentDepth + 1);
                    break;
                default:
                    Debug.LogError("Unknown type: " + curJson.GetValue(key));
                    break;
            }

            obj.name = key;

            int index = System.Array.IndexOf(keys, key);
        }
    }

    public void SetJson(string json)
    {
        var result = UdonJsonDeserializer.TryDeserialize(json, out this.json);
    }

    public void SetJson(TextAsset textAsset)
    {
        SetJson(textAsset.text);
    }
}
