using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    public enum PinType
    {
        ChipInput,
        ChipOutput
    }

    public static readonly float RADIUS = 0.1075f;
    public static readonly float INTERACT_RADIUS = RADIUS * 1.1f;

    [SerializeField]
    private PinType m_pinType;

    private Chip m_chip;
    private string m_pinName;
    private int m_pinIndex;
    private bool m_isCyclic;
    private Pin m_parentPin;
    private List<Pin> m_childrenPins;
    private int m_currentPinState;
    private Color m_defaultColor = Color.black;
    private Color m_interactColor = new Color(0.7f, 0.7f, 0.7f);
    private Material m_material;

    public int PinIndex
    {
        get { return m_pinIndex; }
        set
        {
            if (value < 0)
            {
                Debug.LogWarning("Pin Index Can't be negative number.");
                return;
            }
            m_pinIndex = value;
        }
    }

    public bool IsCyclic
    {
        get { return m_isCyclic; }
    }

    public int PinState
    {
        get { return m_currentPinState; }
    }

    public bool HasParent
    {
        get { return m_parentPin != null || m_pinType == PinType.ChipOutput; }
    }

    private void Awake()
    {
        m_material = GetComponent<MeshRenderer>().material;
        m_material.color = m_defaultColor;
    }

    private void Start()
    {
        transform.localScale = 2 * RADIUS * Vector3.one;
    }

    public void ReceiveSignal(int _signal)
    {
        m_currentPinState = _signal;

        if (m_pinType == PinType.ChipInput && !m_isCyclic)
        {
            m_chip.ReceiveInputSignal(this);
        }
        else if (m_pinType == PinType.ChipOutput)
        {
            for (int i = 0; i < m_childrenPins.Count; i++)
            {
                m_childrenPins[i].ReceiveSignal(_signal);
            }
        }
    }

    public static bool IsVaidConnection(Pin _pinA, Pin _pinB)
    {
        return _pinA.m_pinType != _pinB.m_pinType;
    }

    public static void MakeConnection(Pin _pinA, Pin _pinB)
    {
        if (!IsVaidConnection(_pinA, _pinB))
        {
            return;
        }

        Pin _parentPin = _pinA.m_pinType == PinType.ChipOutput ? _pinA : _pinB;
        Pin _childPin = _pinA.m_pinType == PinType.ChipInput ? _pinA : _pinB;

        _parentPin.m_childrenPins.Add(_childPin);
        _childPin.m_parentPin = _parentPin;
    }

    public static void RemoveConnection(Pin _pinA, Pin _pinB)
    {
        Pin _parentPin = _pinA.m_pinType == PinType.ChipOutput ? _pinA : _pinB;
        Pin _childPin = _pinA.m_pinType == PinType.ChipInput ? _pinA : _pinB;

        _parentPin.m_childrenPins.Remove(_childPin);
        _childPin.m_parentPin = null;
    }

    public static bool TryConnect(Pin _pinA, Pin _pinB)
    {
        if (_pinA.m_pinType == _pinB.m_pinType)
        {
            return false;
        }

        Pin _parentPin = _pinA.m_pinType == PinType.ChipOutput ? _pinA : _pinB;
        Pin _childPin = _pinA.m_pinType == PinType.ChipInput ? _pinA : _pinB;
        _parentPin.m_childrenPins.Add(_childPin);
        _childPin.m_parentPin = _parentPin;
        return true;
    }
}
