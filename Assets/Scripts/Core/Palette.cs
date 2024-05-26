using System.Collections.ObjectModel;
using CircuitSimulation.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Circuit/Palette")]
public class Palette : ScriptableObject
{
    [SerializeField]
    private int m_defaultColorIndex;

    [SerializeField]
    private ThemeColor[] m_themeColors;

    public ThemeColor GetVoltageColor() => m_themeColors[m_defaultColorIndex];

    public ReadOnlyCollection<ThemeColor> VoltageColors =>
        new ReadOnlyCollection<ThemeColor>(m_themeColors);

    public ThemeColor GetTheme(string _themeName)
    {
        if (string.IsNullOrEmpty(_themeName))
        {
            return m_themeColors[0];
        }

        foreach (var _theme in m_themeColors)
        {
            if (_theme.name == _themeName)
            {
                return _theme;
            }
        }

        Debug.LogWarning(string.Format("Could not find theme with name : {0}", _themeName));
        return m_themeColors[0];
    }
}
