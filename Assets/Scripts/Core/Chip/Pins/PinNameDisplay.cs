using TMPro;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public class PinNameDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_nameText;

        [SerializeField]
        private Transform m_nameDisplayHolder;

        [SerializeField]
        private Transform m_nameDisplayBackground;

        [SerializeField]
        private Vector2 m_backgroundPadding;

        [SerializeField]
        private float m_spacingAfterPin;

        public void SetNameVisibility(bool _show)
        {
            m_nameDisplayHolder.gameObject.SetActive(_show);
        }

        public bool GetNameVisibility()
        {
            return m_nameDisplayHolder.gameObject.activeSelf;
        }

        public void SetText(string _text, bool _isDiplayToRight)
        {
            m_nameText.text = _text;

            Vector2 _size = m_nameText.GetPreferredValues();

            m_nameDisplayBackground.localScale = new Vector3(
                _size.x + m_backgroundPadding.x,
                _size.y + m_backgroundPadding.y,
                1f
            );

            float _posX =
                ((_size.x + m_backgroundPadding.x) / 2 + m_spacingAfterPin)
                * (_isDiplayToRight ? 1 : -1);
            m_nameDisplayHolder.localPosition = new Vector3(_posX, 0f, 0f);
            m_nameDisplayHolder.position = new Vector3(
                m_nameDisplayHolder.position.x,
                m_nameDisplayHolder.position.y,
                RenderOrder.PIN_NAME_DISPLAY
            );
        }
    }
}
