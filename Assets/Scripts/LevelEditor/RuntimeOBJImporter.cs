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
    public LevelEditorUIController editorUIController;
    public List<TextAsset> presets;
    public Material objectsMaterial;
	GameObject newObj;
    public List<Transform> loadedModelContainers = new List<Transform>();
    public List<LoadedOBJData> loadedModels;
    public List<string> loadedData;
    public ExtensionFilter[] extensions;

    public void Start()
    {
        
        extensions = new[] {
            new ExtensionFilter("OBJ Files", "obj"),
        };
        UpdateImportedModelContainers();
    }

    public void UpdateImportedModelContainers()
    {
        loadedModelContainers = new List<Transform>();
        foreach(SimulationScenario scenario in editorUIController.simulationScenarios)
        {
            loadedModelContainers.Add(scenario.importedModelsContainer.transform);
        }
    }



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
        LoadOBJFromString(loader.text);
        yield break;
    }
#else


    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return new WaitForEndOfFrame(); // To re-enable button color
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);
        if (paths.Length > 0)
        {
			for (int i = 0; i < paths.Length; i++)
				Debug.Log("Now Loading: " + " " + paths[0]);

            newObj = new OBJLoader().Load(paths[0]);
            var data = newObj.AddComponent<LoadedOBJData>();
            using (var fs = new FileStream(paths[0], FileMode.Open))
            {
                data.objData = new StreamReader(fs).ReadToEnd();
            }
            loadedModels.Add(data);
            newObj.transform.SetParent(loadedModelContainers[0]);
            foreach(Transform child in newObj.transform)
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


    public void LoadOBJ()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        UploadFile(gameObject.name, "OnFileUpload", ".obj", false);
        #else
        StartCoroutine(ShowLoadDialogCoroutine());
        #endif
    }

    public LoadedOBJData LoadPreset(int index)
    {
        return LoadOBJFromString(presets[index].text);
    }

    public LoadedOBJData LoadOBJFromString(string data)
    {
        UpdateImportedModelContainers();

        // Creates a new GameObject from the data
        var textStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        newObj = new OBJLoader().Load(textStream);
        var newObjData = newObj.AddComponent<LoadedOBJData>();
        newObjData.objData = data;
        loadedModels.Add(newObjData);
        loadedData.Add(data);

        // Adjust object position and scale
        newObj.transform.SetParent(loadedModelContainers[0]);
        newObj.transform.localScale = new Vector3(-1f, 0.1f, 1f);

        // Add materials, colliders and tags
        foreach (Transform child in newObj.transform)
        {
            if (child.TryGetComponent(out Renderer renderer))
            {
                renderer.material = objectsMaterial;
            }

            if (child.TryGetComponent(out MeshFilter _))
            {
                child.gameObject.AddComponent<BoxCollider>();
                child.tag = "OBJCollider";
            }
        }

        // Create copies for each alternative
        for (int i = 1; i < loadedModelContainers.Count; i ++)
        {
            GameObject copyObj = Instantiate(newObj);
            copyObj.transform.SetParent(loadedModelContainers[i]);
            copyObj.transform.localPosition = Vector3.zero;
            copyObj.transform.localScale = new Vector3(-1f, 0.1f, 1f);
            loadedModels.Add(copyObj.GetComponent<LoadedOBJData>());
        }
        //loadedObject.transform.position = new Vector3(15f, 0f, 15f);
        return newObjData;
    }

    public void ClearAllLoadedModels()
    {
        UpdateImportedModelContainers();
        foreach (Transform scenario in loadedModelContainers)
        {
            foreach (Transform child in scenario)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
        }
        loadedModels = new List<LoadedOBJData>();
        loadedData = new List<string>();
    }

}
