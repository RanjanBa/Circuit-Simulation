using UnityEngine;

namespace CircuitSimulation.Core
{
    public struct ChipDescription
    {
        public string name;
        public string color;
        public PinDescription[] inputPinsDescriptions;
        public PinDescription[] outputPinsDescriptions;
        public ChipInfo[] subChips;
    }

    public struct ChipInfo
    {
        public string name;
        public int id;
        public Point[] points;
        public byte[] data;
    }

    public struct Point
    {
        public float x;
        public float y;

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point ToPoint(Vector2 _p)
        {
            return new Point(_p.x, _p.y);
        }

        public static Vector2 ToVector(Point _p)
        {
            return new Vector2(_p.x, _p.y);
        }
    }

    [System.Serializable]
    public struct PinDebugInfo
    {
        public int numberOfInputs;
        public bool isFloating;
        public bool isCycle;
        public int id;
    }

    public struct PinDescription
    {
        public string name;
        public int id;
        public float positionY;
        public string colorThemeName;
    }

    [System.Serializable]
    public struct ChipOverride
    {
        public string chipName;
        public BaseChip prefab;

        public bool IsValidMatch(string name)
        {
            return string.Equals(name, chipName, System.StringComparison.OrdinalIgnoreCase) && prefab != null;
        }
    }

    public struct DisplayOptions
    {
        public enum PinNameDisplayMode { Always, Hover, Toggle, Never }
        public enum ToggleState { Off, On }

        public PinNameDisplayMode mainChipPinNameDisplayMode;
        public PinNameDisplayMode subChipPinNameDisplayMode;
        public ToggleState showCursorGuide;
    }
}
