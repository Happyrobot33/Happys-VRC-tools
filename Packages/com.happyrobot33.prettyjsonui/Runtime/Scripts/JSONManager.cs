using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Data;
using System.Reflection;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UdonSharpEditor;

//display a enum dropdown for all the public variables on the udonbehaviour script if it is attached
[CustomEditor(typeof(JSONManager))]
public class JSONManagerInspector : Editor {
    int selectedVariable = 0;
    public override void OnInspectorGUI() {
        JSONManager jsonManager = (JSONManager)target;
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
        DrawDefaultInspector();
        if (jsonManager.watchBehaviour != null) {
            FieldInfo[] fields = jsonManager.watchBehaviour.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            string[] variables = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++) {
                variables[i] = fields[i].Name;
            }
            //find our current variable index
            for (int i = 0; i < variables.Length; i++) {
                if (variables[i] == jsonManager.watchVariable) {
                    selectedVariable = i;
                    break;
                }
            }

            selectedVariable = EditorGUILayout.Popup("Watch Variable", selectedVariable, variables);
            jsonManager.watchVariable = variables[selectedVariable];
        }
    }
}
#endif

public class JSONManager : UdonSharpBehaviour
{
    DataDictionary json;
    public int depthLimit = 10;
    public float indentSpacing = 0.1f;

    [FieldChangeCallback(nameof(stringSource))]
    public string _stringSource = "";

    [FieldChangeCallback(nameof(textAssetSource))]
    public TextAsset _textAssetSource;

    /// <summary>
    /// The JSON string to parse. This is a string. Upon setting this value, the hierarchy will be cleared and rebuilt.
    /// </summary>
    public string stringSource
    {
        set
        {
            _stringSource = value;
            // Clear the hierarchy
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            SetJson(value);

            initializeHierarchy(json);
        }
        get { return _stringSource; }
    }

    /// <summary>
    /// The JSON string to parse. This is text asset. Upon setting this value, the hierarchy will be cleared and rebuilt.
    /// </summary>
    public TextAsset textAssetSource
    {
        set
        {
            _textAssetSource = value;
            // Clear the hierarchy
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            SetJson(value);

            initializeHierarchy(json);
        }
        get { return _textAssetSource; }
    }

    /// <summary>
    /// This is a variable to watch for updates. This script will automatically update the hierarchy when this variable changes.
    /// </summary>
    [HideInInspector]
    public string watchVariable = "";

    /// <summary>
    /// The behaviour to watch for updates. This script will automatically update the hierarchy when this behaviour's variable changes.
    /// </summary>
    [Tooltip(
        "The behaviour to watch for updates. This script will automatically update the hierarchy when this behaviour's variable changes."
    )]
    public UdonSharpBehaviour watchBehaviour;

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

    void Update()
    {
        if (watchBehaviour != null)
        {
            //check if the variable has changed
            if ((string)watchBehaviour.GetProgramVariable(watchVariable) != stringSource)
            {
                SetJson((string)watchBehaviour.GetProgramVariable(watchVariable));
            }
        }
    }

    // Recursive function to create the hierarchy system
    [RecursiveMethod]
    public void initializeHierarchy(
        DataDictionary curJson,
        GameObject root = null,
        int currentDepth = 0
    )
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

        DataToken[] keys = curJson.GetKeys().ToArray();
        foreach (DataToken key in keys)
        {
            // Create the object
            GameObject obj = Instantiate(keyValuePairPrefab, root.transform);
            curJson.TryGetValue(key, out DataToken value);
            Debug.Log(value.TokenType);
            switch (value.TokenType)
            {
                case TokenType.DataList:
                    string arrayString = "";
                    for (int i = 0; i < value.DataList.Count; i++)
                    {
                        value.DataList.TryGetValue(i, out DataToken element);
                        //DataToken element = key.DataList[i];
                        arrayString += element.ToString();
                        if (i != value.DataList.Count - 1)
                        {
                            arrayString += ", ";
                        }
                    }
                    obj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
                        key.ToString() + ": [" + arrayString + "] ";
                    break;
                case TokenType.DataDictionary:
                    obj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
                        key.ToString() + " {" + value.DataDictionary.Count + "} ";
                    initializeHierarchy(value.DataDictionary, obj, currentDepth + 1);
                    break;
                default:
                    obj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
                        key.ToString() + ": " + value.ToString();
                    break;
            }

            obj.name = key.ToString();

            int index = System.Array.IndexOf(keys, key);
        }
    }

    void SetJson(string jsonIN)
    {
        DataToken jsonDataToken;
        bool success = VRCJson.TryDeserializeFromJson(jsonIN, out jsonDataToken);
        if (success)
        {
            json = jsonDataToken.DataDictionary;
        }
        else
        {
            Debug.LogError("Failed to deserialize JSON");
        }
    }

    void SetJson(TextAsset textAsset)
    {
        SetJson(textAsset.text);
    }
}
