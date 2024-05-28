namespace Biocrowds.Core.Models
{
    [System.Serializable]
    public class Simulation
    {
        public AgentSim[] data;
    }

    [System.Serializable]
    public class AgentSim
    {
        public int id;
        public int startFrame;
        public SpawnerSim spawner;
        public GoalSim[] goals;
    }

    [System.Serializable]
    public class SpawnerSim
    {
        public float x;
        public float y;
    }

    [System.Serializable]
    public class GoalSim
    {
        public float x;
        public float y;
        public float speed;
    }
}