using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipSignal : Chip
{
    [SerializeField]
    private int m_currentChipState;

    [SerializeField]
    private Palette m_palette;

    [SerializeField]
    private MeshRenderer m_indicatorRenderer;

    [SerializeField]
    private MeshRenderer m_pinRenderer;

    [SerializeField]
    private MeshRenderer m_wireRenderer;

    private int m_groudId = -1;
    private string m_signalName;

    protected bool m_interactable = true;

    public string SignalName
    {
        get { return m_signalName; }
    }

    public int GroupID
    {
        get { return m_groudId; }
        set { m_groudId = value; }
    }

    public virtual void SetInteractable(bool _interactable)
    {
        m_interactable = _interactable;

        if (!m_interactable)
        {
            m_indicatorRenderer.material.color = m_palette.nonInteractableColor;
            m_pinRenderer.material.color = m_palette.nonInteractableColor;
            m_wireRenderer.material.color = m_palette.nonInteractableColor;
        }
    }

    public virtual void SetDisplayState(int _state)
    {
        if (m_indicatorRenderer && m_interactable)
        {
            m_indicatorRenderer.material.color =
                (_state == 1) ? m_palette.onColor : m_palette.offColor;
        }
    }

    public virtual void UpdateSignalName(string _name)
    {
        m_signalName = _name;
    }
}
