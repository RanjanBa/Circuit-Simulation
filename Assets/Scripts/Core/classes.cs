using System;
using UnityEngine;

namespace CircuitSimulation.Core
{
    [System.Serializable]
    public class ThemeColor
    {
        public string name;
        public Color highColor;
        public Color lowColor;
        public Color triStateColor;
        public int displayPriority;

        public Color GetColor(PinState _state, bool _useTriStateColor = true)
        {
            switch (_state)
            {
                case PinState.High:
                    return highColor;
                case PinState.Low:
                    return lowColor;
                default:
                    return _useTriStateColor ? triStateColor : lowColor;
            }
        }
    }

    public static class DisplaySettings
    {
        public const float PIN_SIZE = 0.2f;
        public const float HIGHLIGHT_PADDING = 0.3f;
    }

    public static class RenderOrder
    {
        public const float LAYER_ABOVE = -0.01f;
        public const float BACKGROUND = 0f;
        public const float BACKGROUND_OUTLINE = BACKGROUND + LAYER_ABOVE;
        public const float WIRE_LOW = BACKGROUND_OUTLINE + LAYER_ABOVE;
        public const float WIRE_HIGH = WIRE_LOW + LAYER_ABOVE;
        public const float BUS_WIRE_LOW = WIRE_HIGH + LAYER_ABOVE;
        public const float BUS_WIRE_HIGH = BUS_WIRE_LOW + LAYER_ABOVE;
        public const float WIRE_EDIT = BUS_WIRE_HIGH + LAYER_ABOVE;
        public const float PIN_NAME_DISPLAY = WIRE_EDIT + LAYER_ABOVE;
        public const float CHIP = PIN_NAME_DISPLAY + LAYER_ABOVE;
        public const float CHIP_PIN = CHIP + LAYER_ABOVE;
        public const float EDITABLE_PIN = CHIP_PIN + LAYER_ABOVE;
        public const float EDITABLE_PIN_HIGH = EDITABLE_PIN + LAYER_ABOVE;
        public const float EDITABLE_PIN_PREVIEW = EDITABLE_PIN_HIGH + LAYER_ABOVE;
        public const float BUS_CONNECTION_DOT = EDITABLE_PIN_PREVIEW + LAYER_ABOVE;
        public const float CHIP_MOVING = BUS_CONNECTION_DOT + LAYER_ABOVE;
    }

    public static class ColorHelper
    {
        private static float Luminance(Color _color)
        {
            return 0.2126f * _color.r + 0.7152f * _color.g + 0.0722f * _color.b;
        }

        public static Color TextBackOrWhite(Color _backGroundColor)
        {
            return Luminance(_backGroundColor) > 0.57f ? Color.black : Color.white;
        }

        public static Color Darken(
            Color _color,
            float _darkenAmount,
            float _desaturationAmount = 0f
        )
        {
            Color.RGBToHSV(_color, out float _h, out float _s, out float _v);
            _v = Mathf.Clamp01(_v - _darkenAmount);
            _s = Mathf.Clamp01(_s - _desaturationAmount);

            return Color.HSVToRGB(_h, _v, _s);
        }
    }
}
