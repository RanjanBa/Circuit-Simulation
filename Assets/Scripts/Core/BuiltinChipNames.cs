using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public static class BuiltinChipNames
    {
        public const string AndChip = "AND";
        public const string NotChip = "NOT";
        public const string TriStateBufferName = "TRI-STATE BUFFER";
        public const string SevenSegmentDisplayName = "7-SEGMENT DISPLAY";
        public const string BusName = "BUS";
        public const string ClockName = "CLOCK";

        private static readonly string[] _allNames = new string[] {
            AndChip,
            NotChip,
            TriStateBufferName,
            SevenSegmentDisplayName,
            BusName,
            ClockName
        };

        public static bool IsBuiltinName(string _chipName, bool _ignoreCase = true)
        {
            for (int i = 0; i < _allNames.Length; i++)
            {
                if (Compare(_chipName, _allNames[i], _ignoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Compare(string _a, string _b, bool _ignoreCase = true)
        {
            System.StringComparison comparison = _ignoreCase ? System.StringComparison.OrdinalIgnoreCase : System.StringComparison.Ordinal;
            return string.Equals(_a, _b, comparison);
        }
    }
}
