using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Dummiesman;


#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#else
using SimpleFileBrowser;
using UnityEditor;
#endif
public class RuntimeOBJImporter : MonoBehaviour
{
	GameObject loadedObject;

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

        loadedObject.transform.position = new Vector3(25f, 0, 20f);
        yield break;
    }
#else
    void Start()
    {
		FileBrowser.SetFilters(false, new FileBrowser.Filter("OBJ File", ".obj"));
		FileBrowser.SetDefaultFilter(".obj");
		FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
		FileBrowser.AddQuickLink("Users", "C:\\Users", null);
	}

	IEnumerator ShowLoadDialogCoroutine()
	{
		yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Load the OBJ File", "Load");
		
		if (FileBrowser.Success)
		{
			for (int i = 0; i < FileBrowser.Result.Length; i++)
				Debug.Log("Now Loading: " + FileBrowser.Result[i]);

			loadedObject = new OBJLoader().Load(FileBrowser.Result[0]);
			loadedObject.transform.position = new Vector3(15f, 0, 15f);

			// Sets all objects as NavigationStatic
			GameObjectUtility.SetStaticEditorFlags(loadedObject, StaticEditorFlags.NavigationStatic);
			for (int i = 0; i < loadedObject.transform.childCount; i++)
				if (loadedObject.transform.GetChild(i).TryGetComponent(out MeshRenderer _mesh))
					GameObjectUtility.SetStaticEditorFlags(loadedObject.transform.GetChild(i).gameObject, StaticEditorFlags.NavigationStatic);
		}
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
}
