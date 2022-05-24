using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Dummiesman;
using SFB;


#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class RuntimeOBJImporter : MonoBehaviour
{
    public Material objectsMaterial;
	GameObject loadedObject;
	public Transform loadedModelsContainer;
    public List<LoadedOBJData> loadedModels;
    public ExtensionFilter[] extensions;

#if UNITY_WEBGL && !UNITY_EDITOR
    
	// WebGL
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(OutputRoutine(url));
    }

	private IEnumerator OutputRoutine(string url)
    {
        var loader = new WWW(url);

        yield return loader;

        var textStream = new MemoryStream(Encoding.UTF8.GetBytes(loader.text));
        loadedObject = new OBJLoader().Load(textStream);
        loadedObject.transform.SetParent(loadedModelsContainer);
        loadedObject.transform.position = new Vector3(25f, 0, 20f);
        foreach(Transform child in loadedObject.transform)
            {
                if (child.TryGetComponent(out Renderer renderer))
                {
                    renderer.material = objectsMaterial;
                }
            }
        yield break;
    }
#else
    void Start()
    {
        extensions = new[] {
            new ExtensionFilter("OBJ Files", "obj"),
        };
    }

	IEnumerator ShowLoadDialogCoroutine()
    {
        yield return new WaitForEndOfFrame(); // To re-enable button color
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);
        if (paths.Length > 0)
        {
			for (int i = 0; i < paths.Length; i++)
				Debug.Log("Now Loading: " + " " + paths[0]);

            loadedObject = new OBJLoader().Load(paths[0]);
            var data = loadedObject.AddComponent<LoadedOBJData>();
            using (var fs = new FileStream(paths[0], FileMode.Open))
            {
                data.objData = new StreamReader(fs).ReadToEnd();
            }
            loadedModels.Add(data);
            loadedObject.transform.SetParent(loadedModelsContainer);
			loadedObject.transform.position = new Vector3(15f, 0, 15f);
            foreach(Transform child in loadedObject.transform)
            {
                if (child.TryGetComponent(out Renderer renderer))
                {
                    renderer.material = objectsMaterial;
                }
            }
		}
        yield return null;
        
	}

    
#endif

    public void Awake()
    {
        if (!loadedModelsContainer)
            loadedModelsContainer = GameObject.Find("ImportedModels").GetComponent<Transform>();
    }

    public void LoadOBJ()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        UploadFile(gameObject.name, "OnFileUpload", ".obj", false);
        #else
        StartCoroutine(ShowLoadDialogCoroutine());
        #endif
    }

    public LoadedOBJData LoadOBJFromString(string data)
    {
        var textStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        loadedObject = new OBJLoader().Load(textStream);

        var newObjData = loadedObject.AddComponent<LoadedOBJData>();
        newObjData.objData = data;
        loadedModels.Add(newObjData);
        loadedObject.transform.SetParent(loadedModelsContainer);
        foreach (Transform child in loadedObject.transform)
        {
            if (child.TryGetComponent(out Renderer renderer))
            {
                renderer.material = objectsMaterial;
            }
        }
        return newObjData;
    }

    public void ClearLoadedModels()
    {
        foreach (Transform child in loadedModelsContainer.transform)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        loadedModels = new List<LoadedOBJData>();
    }

}
