using UnityEngine;

public class Chip : MonoBehaviour
{
    [SerializeField]
    private string m_chipName = "Untitle";

    [SerializeField]
    private Pin[] m_inputPins;

    [SerializeField]
    private Pin[] m_outputPins;

    [SerializeField]
    private BoxCollider2D m_bounds;

    private int m_numberOfInputSignalReceived;
    private int m_lastSimulatedFrame;
    private int m_lastSimulationInitializeFrame;

    public Vector2 BoundsSize
    {
        get { return m_bounds.size; }
    }

    private void Awake()
    {
        m_bounds = GetComponent<BoxCollider2D>();
    }

    private void SetPinIndices()
    {
        for (int i = 0; i < m_inputPins.Length; i++)
        {
            m_inputPins[i].PinIndex = i;
        }

        for (int i = 0; i < m_outputPins.Length; i++)
        {
            m_outputPins[i].PinIndex = i;
        }
    }

    private void InitializeSimulationFrame()
    {
        if (m_lastSimulationInitializeFrame != Simulation.simulationFrame)
        {
            m_lastSimulationInitializeFrame = Simulation.simulationFrame;
            ProcessUnconnectedInputsAndCycle();
        }
    }

    private void ProcessUnconnectedInputsAndCycle()
    {
        for (int i = 0; i < m_inputPins.Length; i++)
        {
            if (m_inputPins[i].IsCyclic)
            {
                ReceiveInputSignal(m_inputPins[i]);
            }
            else
            {
                m_inputPins[i].ReceiveSignal(0);
            }
        }
    }

    protected virtual void Start()
    {
        SetPinIndices();
    }

    protected virtual void ProcessInput() { }

    public void ReceiveInputSignal(Pin _pin)
    {
        if (m_lastSimulatedFrame != Simulation.simulationFrame)
        {
            m_lastSimulatedFrame = Simulation.simulationFrame;
            m_numberOfInputSignalReceived = 0;
            InitializeSimulationFrame();
        }

        m_numberOfInputSignalReceived++;

        if (m_numberOfInputSignalReceived == m_inputPins.Length)
        {
            ProcessInput();
        }
    }
}
