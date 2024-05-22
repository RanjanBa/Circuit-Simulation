using UnityEngine;

public class Simulation : MonoBehaviour
{
    public static int simulationFrame { get; private set; }

    private static Simulation m_instance;

    [SerializeField]
    private float m_minStepTime = 0.075f;

    private float m_lastStepTime;

    public static Simulation Instance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindObjectOfType<Simulation>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        simulationFrame = 0;
    }

    private void Update() {
        if(Time.time - m_lastStepTime > m_minStepTime) {
            m_lastStepTime = Time.time;
            StepSimulation();
        }
    }

    private void StepSimulation() {
        simulationFrame++;
    }
}
