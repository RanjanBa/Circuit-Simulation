using UnityEngine;

namespace CircuitSimulation.Core
{
    public abstract class ControllerBase : MonoBehaviour
    {
        protected ChipCreator m_chipCreator;

        public virtual void SetUp(ChipCreator _creator)
        {
            m_chipCreator = _creator;
        }

        public virtual bool IsBusy()
        {
            return false;
        }
    }
}
