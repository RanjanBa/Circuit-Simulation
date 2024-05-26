using CircuitSimulation.Plugins;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public class EditablePinIndicator : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer m_meshRenderer;

        public MouseInteraction<EditablePinIndicator> mouseInteraction;

        private void Awake()
        {
            m_meshRenderer.material = Instantiate(m_meshRenderer.sharedMaterial);
            mouseInteraction = new MouseInteraction<EditablePinIndicator>(gameObject, this);
        }

        public void SetColor(Color _color)
        {
            m_meshRenderer.material.color = _color;
        }
    }
}
