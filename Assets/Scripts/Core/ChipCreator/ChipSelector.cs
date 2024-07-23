using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CircuitSimulation.Core
{
    public class ChipSelector : ControllerBase
    {
        public ReadOnlyCollection<BaseChip> SelectedChips
        {
            get
            {
                return new ReadOnlyCollection<BaseChip>(m_selectedChips);
            }
        }

        private List<BaseChip> m_selectedChips;
    }
}