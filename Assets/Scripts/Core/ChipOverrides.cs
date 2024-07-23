using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public class ChipOverrides : ScriptableObject
    {
        [SerializeField]
        private bool m_useChipOverrides;
        [SerializeField]
        private bool m_useVideoChipOverrides;
        [SerializeField]
        private bool m_useVideoChipsInBuild;
        [SerializeField]
        private ChipOverride[] m_chipOverrides;
        [SerializeField]
        private ChipOverride[] m_videoChipOverrides;

        public Dictionary<string, BaseChip> CreateLookup()
        {
            Dictionary<string, BaseChip> lookup = new Dictionary<string, BaseChip>(System.StringComparer.OrdinalIgnoreCase);

            if (m_chipOverrides != null && m_useChipOverrides)
            {
                foreach (var entry in m_chipOverrides.Where(x => x.prefab != null))
                {
                    lookup.Add(entry.chipName, entry.prefab);
                }
            }

            if (m_videoChipOverrides != null && m_useVideoChipOverrides && (m_useVideoChipsInBuild || Application.isEditor))
            {
                foreach (var entry in m_videoChipOverrides.Where(x => x.prefab != null))
                {
                    lookup.Add(entry.chipName, entry.prefab);
                }
            }

            return lookup;
        }
    }
}