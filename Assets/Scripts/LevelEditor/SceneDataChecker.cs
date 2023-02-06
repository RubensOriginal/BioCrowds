using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.LevelEditor
{
    class SceneDataChecker
    {
        // Check if Number of Agents in a Scene is the same of others
        public SceneDataChecker CheckNumberAgentsInTestLevels(List<GameObject> testLevels)
        {
            int lastAgentPerTestLevel = -1;


            foreach (GameObject testLevel in testLevels)
            {
                List<SpawnArea> spawnAreas = testLevel.GetComponentsInChildren<SpawnArea>().ToList();

                int numberAgents = 0;

                spawnAreas.ForEach(spawnArea => numberAgents += spawnArea.initialNumberOfAgents);

                if (lastAgentPerTestLevel != -1 && lastAgentPerTestLevel != numberAgents)
                    throw new Exception("Number of Agents is Different");
                else
                    lastAgentPerTestLevel = numberAgents;

            }

            return this;
        }

        public SceneDataChecker CheckNumberOfGoals(List<GameObject> testLevels)
        {
            int lastNumberOfGoals = -1; // -1 means null

            foreach (GameObject testLevel in testLevels)
            {
                int numberGoals = testLevel.GetComponentsInChildren<Goal>().ToList().Count;

                Debug.Log("Number of Goals" + numberGoals);

                if (lastNumberOfGoals != -1 && lastNumberOfGoals != numberGoals)
                    throw new Exception("Number of Goals is Different");
                else
                    lastNumberOfGoals = numberGoals;
            }

            return this;

        }
    }
}
