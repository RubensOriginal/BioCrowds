using Biocrowds.Core.Models;
using UnityEngine;

namespace Biocrowds.Core
{
    public class EnviromentBuilder : MonoBehaviour
    {
        [Header("Files")]
        public TextAsset inputFile;
        
        [Header("Prefabs")]
        public GameObject spawnerPrefab;
        public GameObject goalPrefab;

        [Header("Project Hierarchy")] 
        public Transform spawnersTransform;
        public Transform goalsTransform;

        [Header("BioCrowds References")] 
        public World world;
        
        private Simulation _simConfig;

        void Start()
        {
            _simConfig = JsonUtility.FromJson<Simulation>(inputFile.text);

            int count = 0;
            
            foreach (AgentSim a in _simConfig.data)
            {
                CreateAgent(a);

                // if (count == 9)
                //    break;
                count++;
            }
        }

        private void CreateAgent(AgentSim a)
        {
            float dis = 0.0f;
            for (int i = 1; i < a.goals.Length; i++)
            {
                GoalSim g1 = a.goals[i - 1];
                Vector2 v1 = new Vector2(g1.x, g1.y);
            }

            GameObject spawner = Instantiate(spawnerPrefab, new Vector3(a.spawner.x + 1.0f, 0.0f, a.spawner.y + 1.0f), new Quaternion(0,0,0,1),spawnersTransform);
            spawner.name = "Spawner - " + a.id;
            
            SpawnArea spawnArea = spawner.GetComponent<SpawnArea>();
            spawnArea.initialAgentsGoalList.Clear();
            spawnArea.initialFrame = a.startFrame;
            
            int index = 0;
            foreach (GoalSim g in a.goals)
            {
                GameObject goal = Instantiate(goalPrefab, new Vector3(g.x + 1.0f, .5f, g.y + 1.0f), new Quaternion(0,  0.7071068f,  -0.7071068f, 0),
                    goalsTransform);
                goal.name = "Goal " + index + " - " + a.id;
                goal.SetActive(false);
                
                // GoalArea goalArea = goal.AddComponent<GoalArea>();
                spawnArea.initialAgentsGoalList.Add(goal);
                spawnArea.goalSpeeds.Add(g.speed);
                spawnArea.initialWaitList.Add(0.0f);
                index++;
            }

            world.spawnAreas.Add(spawnArea);
        }
    }
}