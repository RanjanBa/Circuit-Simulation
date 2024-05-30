using UnityEngine;

namespace CircuitSimulation.Core
{
    public class ChipPlacer : ControllerBase
    {
        public bool IsValidPlacement(BaseChip _chip) {
            Bounds _bounds = _chip.GetBounds();

            // foreach (BaseChip _chip in m_chipCreator.All)
            // {
                
            // }

            return true;
        }
    }
}
