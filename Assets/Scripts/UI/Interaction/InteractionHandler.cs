using UnityEngine;

public abstract class InteractionHandler : MonoBehaviour
{
    private bool m_hasFocus;
    private InteractionHandler[] m_handlers;

    protected bool HasFocus
    {
        get { return m_hasFocus; }
    }

    protected virtual void FocusLost() { }

    protected virtual bool CanReleaseFocus()
    {
        return true;
    }

    protected virtual void RequestFocus()
    {
        if (!m_hasFocus)
        {
            bool _haveAnyHandlerFocus = false;

            foreach (var _handler in m_handlers)
            {
                if (_handler.m_hasFocus)
                {
                    _haveAnyHandlerFocus = true;
                    if (_handler.CanReleaseFocus())
                    {
                        _handler.m_hasFocus = false;
                        _handler.FocusLost();
                        m_hasFocus = true;
                        break;
                    }
                }
            }

            if (!_haveAnyHandlerFocus)
            {
                m_hasFocus = true;
            }
        }
    }

    public void InitializeAllHandlers(InteractionHandler[] _handlers)
    {
        m_handlers = _handlers;
    }

    public abstract void OrderedUpdate();
}
