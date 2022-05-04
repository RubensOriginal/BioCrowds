using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using UnityEngine.SceneManagement;
using Biocrowds.Core;
using System.Linq;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

public class SceneController : MonoBehaviour
{
    public World world;
    private bool initialized = false;

    public static bool ShowCells { get; protected set; } = true;
    public static bool ShowAuxins { get; protected set; } = true;
    public static bool ShowSpawnAreas { get; protected set; } = true;
    public static bool ShowAuxinVectors { get; protected set; } = true;

    public static bool ShowNavMeshCorners { get; protected set; } = true;

    [Header("Render Settings")]
    public bool _showCells;
    public bool _showAuxins;
    public bool _showAuxinVector;
    public bool _showSpawnAreas;
    public bool _showNavMeshCorners;

    public NavMeshSurface surface;
    public NavMeshData m_NavMesh;
    public AsyncOperation m_Operation;
    public NavMeshDataInstance m_Instance;
    public List<NavMeshBuildSource> m_Sources = new List<NavMeshBuildSource>();

    private void Awake()
    {
        ShowCells = _showCells;
        ShowAuxins = _showAuxins;
        ShowSpawnAreas = _showSpawnAreas;
        ShowAuxinVectors = _showAuxinVector;
        ShowNavMeshCorners = _showNavMeshCorners;
    }
    void Start()
    {
        Debug.Log("Press 1 to load world");
    }

    void Update()
    {
        if (_showCells != ShowCells)
        {
            ShowCells = _showCells;
            world.ShowCellMeshes(_showCells);
        }
        if (_showAuxins != ShowAuxins)
        {
            ShowAuxins = _showAuxins;
            world.ShowAuxinMeshes(_showAuxins);
        }
        if (_showSpawnAreas != ShowSpawnAreas)
        {
            ShowSpawnAreas = _showSpawnAreas;
            List<SpawnArea> _spawners = FindObjectsOfType<SpawnArea>().ToList();
            foreach (SpawnArea s in _spawners)
                s.ShowMesh(_showSpawnAreas);
        }
        if (_showAuxinVector != ShowAuxinVectors) ShowAuxinVectors = _showAuxinVector;
        if (_showNavMeshCorners != ShowNavMeshCorners) ShowNavMeshCorners = _showNavMeshCorners;

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Reloading Scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && !initialized)
        {
            Debug.Log("Loading World");

            List<SpawnArea> _spawners = FindObjectsOfType<SpawnArea>().ToList();
            foreach (SpawnArea s in _spawners)
                s.ShowMesh(ShowSpawnAreas);

            initialized = true;
            world.LoadWorld();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var defaultBuildSettings = NavMesh.GetSettingsByID(0);
            surface.BuildNavMesh();
        }

    }

    private void OnEnable()
    {
        if (m_NavMesh == null)
        {
            Debug.Log("Creatin NavMesh Data");
            m_NavMesh = new NavMeshData();
            m_Instance = NavMesh.AddNavMeshData(m_NavMesh);
        }
    }
}
