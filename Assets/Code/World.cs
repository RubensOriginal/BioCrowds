/// ---------------------------------------------
/// Contact: Henry Braun
/// Brief: Defines the world environment
/// Thanks to VHLab for original implementation
/// Date: November 2017 
/// ---------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using System.Linq;
using Unity.AI.Navigation;

namespace Biocrowds.Core
{
    public class World : MonoBehaviour
    {
        [Header("Simulation Configuration")]
        public SimulationConfiguration.MarkerSpawnMethod markerSpawnMethod;

        [SerializeField] private float SIMULATION_TIME_STEP = 0.02f;

        [SerializeField] private float MAX_AGENTS = 0;
        //agent radius
        [SerializeField] private float AGENT_RADIUS = 1.00f;

        //radius for auxin collide
        [SerializeField] private float AUXIN_RADIUS = 0.1f;

        //density
        [SerializeField] private float AUXIN_DENSITY = 0.50f;

        [SerializeField] private float GOAL_DISTANCE_THRESHOLD = 1.0f;


        [Header("Terrain Setting")]
        public MeshFilter planeMeshFilter;

        [SerializeField]
        private Terrain _terrain;

        [SerializeField]
        private NavMeshSurface surface;

        [SerializeField] 
        private NavMeshData navMeshData;

        public NavMeshDataInstance navMeshDataInstance;

        [SerializeField]
        private Vector2 _dimension = new Vector2(30.0f, 20.0f);
        public Vector2 Dimension
        {
            get { return _dimension; }
        }

        [SerializeField]
        private Vector2 _offset = new Vector2(0.0f, 0.0f);
        public Vector2 Offset
        {
            get { return _offset; }
        }

        [Header("Agents Settings and Data")]
        private int _newAgentID = 0;
        [SerializeField] private int _maxAgents = 30;
        public List<Agent> agents = new List<Agent>();


        [Header("Cells Settings and Data")]
        List<Cell> _cells = new List<Cell>();

        [Header("Auxins Settings and Data")]
        List<Auxin> _auxins = new List<Auxin>();

        public List<SpawnArea> spawnAreas;



        public List<Cell> Cells
        {
            get { return _cells; }
        }

        public List<Auxin> Auxins
        {
            get { return _auxins; }
        }

        public MarkerSpawner _markerSpawner = null;
        public SimulationPrefabManager prefabManager;

        //max auxins on the ground
        public bool _isReady;

        private void Awake()
        {
            _newAgentID = 0;
            if (spawnAreas.Count == 0)
                spawnAreas = FindObjectsOfType<SpawnArea>().ToList();

            if (planeMeshFilter != null)
            {
                if (planeMeshFilter.name != "Plane")
                    Debug.LogWarning("PlaneMeshFilter Mesh isn't a Plane. " +
                        "The difference in scale may cause unintended behavior.");

                _dimension = new Vector2(Mathf.Ceil(planeMeshFilter.transform.localScale.x * 10f),
                    Mathf.Ceil(planeMeshFilter.transform.localScale.z * 10f));
                _dimension.x += _dimension.x % 2;
                _dimension.y += _dimension.y % 2;

                _offset = new Vector2(Mathf.Round(planeMeshFilter.transform.position.x),
                    Mathf.Round(planeMeshFilter.transform.position.z));
                _offset.x -= (_dimension.x / 2f);
                _offset.y -= (_dimension.y / 2f);

                planeMeshFilter.gameObject.SetActive(false);
            }

            if (navMeshData == null)
            {
                Debug.Log("Creatin NavMesh Data");
                navMeshData = new NavMeshData();
                navMeshDataInstance = NavMesh.AddNavMeshData(navMeshData);
            }

        }

        public void LoadWorld()
        {
            _isReady = false;
            SetMarkerSpawner();

            StartCoroutine(SetupWorld());
        }

        public void SetMarkerSpawner()
        {
            var markerSpawnerMethods = transform.GetComponentsInChildren<MarkerSpawner>();
            _markerSpawner = markerSpawnerMethods.First(p => p.spawnMethod == markerSpawnMethod);
        }

        public void ClearWorld(bool clearSpawnAreaAndGoals)
        {
            _newAgentID = 0;
            
            foreach (Transform child in prefabManager.agentsContainer)
                Destroy(child.gameObject);
            foreach (Transform child in prefabManager.cellsContainer)
                Destroy(child.gameObject);
            foreach (Transform child in prefabManager.auxinsContainer)
                Destroy(child.gameObject);
            agents = new List<Agent>();
            _cells = new List<Cell>();
            _auxins = new List<Auxin>();

            if (clearSpawnAreaAndGoals)
            {
                foreach (Transform child in prefabManager.spawnAreaContainer)
                    Destroy(child.gameObject);
                foreach (Transform child in prefabManager.goalContainer)
                    Destroy(child.gameObject);
            }
        }
        public void CreateNavMesh()
        {
            var defaultBuildSettings = NavMesh.GetSettingsByID(0);
            surface.BuildNavMesh();
        }
        // Use this for initialization
        IEnumerator SetupWorld()
        {
            //change terrain size according informed
            _terrain.terrainData.size = new Vector3(_dimension.x, _terrain.terrainData.size.y, _dimension.y);
            _terrain.transform.position = new Vector3(_offset.x, _terrain.transform.position.y, _offset.y);

            //GameObjectUtility.SetStaticEditorFlags(_terrain.gameObject, StaticEditorFlags.NavigationStatic);
            CreateNavMesh();
            yield return new WaitForSeconds(1.0f);

            //create all cells based on dimension
            CreateCells();

            //populate cells with auxins
            yield return StartCoroutine(_markerSpawner.CreateMarkers(prefabManager.GetAuxinPrefab(), prefabManager.auxinsContainer, _cells, _auxins));
            Debug.Log(_auxins.Count/_cells.Count);

            //create our agents
            yield return StartCoroutine(CreateAgents());

            //wait a little bit to start moving
            yield return new WaitForSeconds(1.0f);

            _isReady = true;
            //Debug.Break();
        }

        public void CreateCells()
        {
            Vector3 _spawnPos = new Vector3();

            for (int i = 0; i < _dimension.x / 2; i++) //i + agentRadius * 2
            {
                for (int j = 0; j < _dimension.y / 2; j++) // j + agentRadius * 2
                {
                    //instantiante a new cell
                    _spawnPos.x = (1.0f + (i * 2.0f)) + _offset.x;
                    _spawnPos.z = (1.0f + (j * 2.0f)) + _offset.y;

                    GameObject newCell = Instantiate(prefabManager.GetCellPrefab(), _spawnPos, Quaternion.identity, prefabManager.cellsContainer);
                    Cell _cell = newCell.GetComponent<Cell>();
                    //change its name
                    newCell.name = "Cell [" + i + "][" + j + "]";

                    //metadata for optimization
                    _cell.X = i;
                    _cell.Z = j;

                    _cell.ShowMesh(SceneController.ShowCells);

                    _cells.Add(_cell);

                }
            }
        }

        private IEnumerator CreateAgents()
        {
            //instantiate agents
            foreach (SpawnArea _area in spawnAreas)
            {
                _area.ResetSpawner();
                for (int i = 0; i < _area.initialNumberOfAgents; i ++)
                {
                    if (MAX_AGENTS == 0 || agents.Count < MAX_AGENTS)
                        SpawnNewAgentInArea(_area, true);
                    yield return null;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            //TODO: Modificar de time-deltatime para fixed frame
            if (!_isReady)
                return;
            foreach (SpawnArea _area in spawnAreas)
            {
                _area.UpdateSpawnCounter(SIMULATION_TIME_STEP);
                if (_area.CycleReady)
                {
                    for (int i = 0; i < _area.quantitySpawnedEachCycle; i++)
                    {
                        if (MAX_AGENTS == 0 || agents.Count < MAX_AGENTS)
                            SpawnNewAgentInArea(_area, false);
                    }
                }
                _area.ResetCycleReady();
            }

            // Update de Navmesh for each agent 
            for (int i = 0; i < agents.Count; i++)
                agents[i].UpdateVisualAgent();

            //reset auxins
            for (int i = 0; i < _cells.Count; i++)
                for (int j = 0; j < _cells[i].Auxins.Count; j++)
                    _cells[i].Auxins[j].ResetAuxin();



            //find nearest auxins for each agent
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].FindNearAuxins();
            }

            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].auxinCount = agents[i].Auxins.Count;
            }
            /*
             * to find where the agent must move, we need to get the vectors from the agent to each auxin he has, and compare with 
             * the vector from agent to goal, generating a angle which must lie between 0 (best case) and 180 (worst case)
             * The calculation formula was taken from the Bicho´s master thesis and from Paravisi OSG implementation.
            */
            /*for each agent:
            1 - for each auxin near him, find the distance vector between it and the agent
            2 - calculate the movement vector 
            3 - calculate speed vector 
            4 - step
            */

            List<Agent> _agentsToRemove = new List<Agent>();
            bool _showAgentAuxingVector = SceneController.ShowAuxinVectors;
            //for (int i = 0; i < _maxAgents; i++)
            for (int i = 0; i < agents.Count; i++)
            {
                //find the agent
                List<Auxin> agentAuxins = agents[i].Auxins;

                //vector for each auxin
                for (int j = 0; j < agentAuxins.Count; j++)
                {
                    //add the distance vector between it and the agent
                    agents[i]._distAuxin.Add(agentAuxins[j].Position - agents[i].transform.position);

                    //just draw the lines to each auxin
                    if (_showAgentAuxingVector)
                        Debug.DrawLine(agentAuxins[j].Position, agents[i].transform.position, Color.green);
                }

                //calculate the movement vector
                agents[i].CalculateDirection();
                //calculate speed vector
                agents[i].CalculateVelocity();
                //step
                if (!agents[i].isWaiting)
                    agents[i].MovementStep(SIMULATION_TIME_STEP);

                agents[i].WaitStep(SIMULATION_TIME_STEP);
                //if (_agents[i].IsAtCurrentGoal() && !_agents[i].isWaiting)


                if (agents[i].removeWhenGoalReached && agents[i].IsAtFinalGoal())
                    _agentsToRemove.Add(agents[i]);
            }

            foreach(Agent a in _agentsToRemove)
            {
                agents.Remove(a);
                Destroy(a.gameObject);
            }
            _agentsToRemove.Clear();

            // Update de Navmesh for each agent 
            for (int i = 0; i < agents.Count; i++)
                agents[i].NavmeshStep(SIMULATION_TIME_STEP);

            
        }

        public void PrepareAgent(Agent _agent)
        {
            _agent.name = "Agent [" + GetNewAgentID() + "]";  //name
            _agent.CurrentCell = GetClosestCellToPoint(_agent.transform.position); // cell
            _agent.agentRadius = AGENT_RADIUS;  //agent radius
            _agent.goalDistThreshold = GOAL_DISTANCE_THRESHOLD;
            _agent.World = this;
            agents.Add(_agent);
        }

        private Cell GetClosestCellToPoint (Vector3 point)
        {
            float _minDist = Vector3.Distance(point, _cells[0].transform.position);
            int _minIndex = 0;
            for (int i = 1; i < _cells.Count; i ++)
            {
                if (Vector3.Distance(point, _cells[i].transform.position) < _minDist)
                {
                    _minDist = Vector3.Distance(point, _cells[i].transform.position);
                    _minIndex = i;
                }
            }

            return _cells[_minIndex];
        }

        public Agent SpawnNewAgent(Vector3 _pos, bool _removeWhenGoalReached, List<GameObject> _goalList)
        {
            GameObject newAgent = Instantiate(prefabManager.GetAgentPrefab(), _pos, Quaternion.identity, prefabManager.agentsContainer);
            Agent _agent = newAgent.GetComponent<Agent>();

            PrepareAgent(_agent);
            _agent.Goal = _goalList[0];  //agent goal
            _agent.goalsList = _goalList;
            _agent.removeWhenGoalReached = _removeWhenGoalReached;
            return _agent;
        }

        public Agent SpawnNewAgentInArea(SpawnArea _area, bool _isInitialSpawn)
        {
            Vector3 _pos = _area.GetRandomPoint();
            GameObject newAgent = Instantiate(prefabManager.GetAgentPrefab(),  _pos, Quaternion.identity, prefabManager.agentsContainer);
            Agent _agent = newAgent.GetComponent<Agent>();
            PrepareAgent(_agent);
            if (_isInitialSpawn)
            {
                _agent.Goal = _area.initialAgentsGoalList[0];  //agent goal
                _agent.goalsList = _area.initialAgentsGoalList;
                _agent.removeWhenGoalReached = _area.initialRemoveWhenGoalReached;
                _agent.goalsWaitList = _area.initialWaitList;
            }
            else
            {
                _agent.Goal = _area.repeatingGoalList[0];  //agent goal
                _agent.goalsList = _area.repeatingGoalList;
                _agent.removeWhenGoalReached = _area.repeatingRemoveWhenGoalReached;
                _agent.goalsWaitList = _area.repeatingWaitList;
            }
            return _agent;
        }

        public int GetNewAgentID()
        {
            _newAgentID++;
            return _newAgentID - 1;
        }

        public void ShowAuxinMeshes(bool p_enable)
        {
            foreach (Auxin _a in Auxins)
                _a.ShowMesh(p_enable);
        }
        public void ShowCellMeshes(bool p_enable)
        {
            foreach (Cell _c in Cells)
                _c.ShowMesh(p_enable);
        }

        public Terrain GetTerrain()
        {
            return _terrain;
        }

    }
}