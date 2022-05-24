using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationPrefabManager : MonoBehaviour
{
    public enum AgentPrefabType
    {
        CUBOID,
        CYLINDER,
        CAPSULE,
        VISUAL_AGENT,
        CUSTOM
    }

    public enum AuxinPrefabType
    {
        CUBE,
        SPHERE,
        QUAD,
        CUSTOM
    }

    public enum CellPrefabType
    {
        QUAD_CHECKER,
        QUAD_UI_SPRITE,
        CUSTOM
    }
    public enum SpawnAreaPrefabType
    {
        CUBE_BLUE,
        CUBE_ICON,
        CUSTOM
    }

    public enum GoalPrefabType
    {
        CUBE_LIME,
        CUBE_ICON,
        CYLINDER_ICON,
        CUSTOM
    }

    public enum ObstaclePrefabType
    {
        CUBE_GRAY,
        CUSTOM
    }

    //---------------- Selected Types ----------------//
    public AgentPrefabType agentsPrefabType = AgentPrefabType.CYLINDER;
    public AuxinPrefabType auxinPrefabType = AuxinPrefabType.SPHERE;
    public CellPrefabType cellPrefabType = CellPrefabType.QUAD_UI_SPRITE;
    public SpawnAreaPrefabType spawnAreaPrefabType = SpawnAreaPrefabType.CUBE_BLUE;
    public GoalPrefabType goalPrefabType = GoalPrefabType.CUBE_LIME;
    public ObstaclePrefabType obstaclePrefabType = ObstaclePrefabType.CUBE_GRAY;

    
    //---------------- Preset Prefabs ----------------//
    private GameObject agentCuboidPrefab;
    private GameObject agentCylinderPrefab;
    private GameObject agentCapsulePrefab;
    private List<GameObject> agentVisualAgentPrefab;

    private GameObject auxingCubePrefab;
    private GameObject auxingSpherePrefab;
    private GameObject auxingQuadPrefab;

    private GameObject cellQuadCheckerPrefab;
    private GameObject cellQuadUISpritePrefab;

    private GameObject spawnAreaCubeBluePrefab;
    private GameObject spawnAreaCubeIconPrefab;

    private GameObject goalCubeLimePrefab;
    private GameObject goalCubeIconPrefab;
    private GameObject goalCylinderIconPrefab;

    private GameObject obstacleCubeGrayPrefab;

    //---------------- Custom Prefabs ----------------//
    [Header("Custom Prefabs")]
    [SerializeField] private List<GameObject> agentCustomPrefabs;
    [SerializeField] private List<GameObject> auxinCustomPrefabs;
    [SerializeField] private List<GameObject> cellCustomPrefabs;
    [SerializeField] private List<GameObject> spawnAreaCustomPrefabs;
    [SerializeField] private List<GameObject> goalCustomPrefabs;
    [SerializeField] private List<GameObject> obstacleCustomPrefabs;

    //---------------- Containers ----------------//
    [Header("Containers")]
    public Transform agentsContainer;
    public Transform auxinsContainer;
    public Transform cellsContainer;
    public Transform goalContainer;
    public Transform obstacleContainer;
    public Transform spawnAreaContainer;

    private void Awake()
    {
        agentCuboidPrefab = Resources.Load<GameObject>("Prefabs/Agents/AgentCuboidPrefab");
        agentCylinderPrefab = Resources.Load<GameObject>("Prefabs/Agents/AgentCylinderPrefab");
        agentCapsulePrefab = Resources.Load<GameObject>("Prefabs/Agents/AgentCapsulePrefab");
        agentVisualAgentPrefab = Resources.LoadAll<GameObject>("Prefabs/Agents/VisualAgents").ToList();

        auxingCubePrefab = Resources.Load<GameObject>("Prefabs/Auxins/AuxinCubePrefab");
        auxingSpherePrefab = Resources.Load<GameObject>("Prefabs/Auxins/AuxinSpherePrefab");
        auxingQuadPrefab = Resources.Load<GameObject>("Prefabs/Auxins/AuxinQuadPrefab");

        cellQuadCheckerPrefab = Resources.Load<GameObject>("Prefabs/Cells/CellQuadCheckerPrefab");
        cellQuadUISpritePrefab = Resources.Load<GameObject>("Prefabs/Cells/CellQuadUISpritePrefab");

        spawnAreaCubeBluePrefab = Resources.Load<GameObject>("Prefabs/SpawnAreas/SpawnAreaCubeBluePrefab");
        spawnAreaCubeIconPrefab = Resources.Load<GameObject>("Prefabs/SpawnAreas/SpawnAreaCubeIconPrefab");

        goalCubeLimePrefab = Resources.Load<GameObject>("Prefabs/Goals/GoalCubeLimePrefab");
        goalCubeIconPrefab = Resources.Load<GameObject>("Prefabs/Goals/GoalCubeIconPrefab");
        goalCylinderIconPrefab = Resources.Load<GameObject>("Prefabs/Goals/GoalCylinderIconPrefab");

        obstacleCubeGrayPrefab = Resources.Load<GameObject>("Prefabs/Obstacles/ObstacleCubeGrayPrefab");

    }




    public GameObject GetAgentPrefab()
    {
        switch(agentsPrefabType)
        {
            case AgentPrefabType.CUBOID:
                return agentCuboidPrefab;
            case AgentPrefabType.CYLINDER:
                return agentCylinderPrefab;
            case AgentPrefabType.CAPSULE:
                return agentCapsulePrefab;
            case AgentPrefabType.VISUAL_AGENT:
                return agentVisualAgentPrefab[Random.Range(0, agentVisualAgentPrefab.Count)];
            case AgentPrefabType.CUSTOM:
                return agentCustomPrefabs[Random.Range(0, agentCustomPrefabs.Count)];
            default:
                return agentCylinderPrefab;
        }
    }

    public GameObject GetAuxinPrefab()
    {
        switch(auxinPrefabType)
        {
            case AuxinPrefabType.CUBE:
                return auxingCubePrefab;
            case AuxinPrefabType.SPHERE:
                return auxingSpherePrefab;
            case AuxinPrefabType.QUAD:
                return auxingQuadPrefab;
            case AuxinPrefabType.CUSTOM:
                return auxinCustomPrefabs[Random.Range(0, auxinCustomPrefabs.Count)];
            default:
                return auxingQuadPrefab;
        }
    }

    public GameObject GetCellPrefab()
    {
        switch(cellPrefabType)
        {
            case CellPrefabType.QUAD_CHECKER:
                return cellQuadCheckerPrefab;
            case CellPrefabType.QUAD_UI_SPRITE:
                return cellQuadUISpritePrefab;
            case CellPrefabType.CUSTOM:
                return cellCustomPrefabs[Random.Range(0, cellCustomPrefabs.Count)];
            default:
                return cellQuadUISpritePrefab;
        }
    }

    public GameObject GetSpawnAreaPrefab()
    {
        switch (spawnAreaPrefabType)
        {
            case SpawnAreaPrefabType.CUBE_BLUE:
                return spawnAreaCubeBluePrefab;
            case SpawnAreaPrefabType.CUBE_ICON:
                return spawnAreaCubeIconPrefab;
            case SpawnAreaPrefabType.CUSTOM:
                return spawnAreaCustomPrefabs[Random.Range(0, spawnAreaCustomPrefabs.Count)];
            default:
                return spawnAreaCubeBluePrefab;
        }
    }

    public GameObject GetGoalPrefab()
    {
        switch (goalPrefabType)
        {
            case GoalPrefabType.CUBE_LIME:
                return goalCubeLimePrefab;
            case GoalPrefabType.CUBE_ICON:
                return goalCubeIconPrefab;
            case GoalPrefabType.CYLINDER_ICON:
                return goalCylinderIconPrefab;
            case GoalPrefabType.CUSTOM:
                return goalCustomPrefabs[Random.Range(0, goalCustomPrefabs.Count)];
            default:
                return goalCubeLimePrefab;
        }
    }

    public GameObject GetObstaclePrefab()
    {
        switch (cellPrefabType)
        {
            case CellPrefabType.QUAD_UI_SPRITE:
                return obstacleCubeGrayPrefab;
            case CellPrefabType.CUSTOM:
                return obstacleCustomPrefabs[Random.Range(0, obstacleCustomPrefabs.Count)];
            default:
                return obstacleCubeGrayPrefab;
        }
    }

    
}
