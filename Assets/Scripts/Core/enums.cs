namespace CircuitSimulation.Core
{
    public enum PinState
    {
        Low,
        High,
        Floating
    }

    public enum HighlightState
    {
        None,
        Highlighted,
        HighlightedInvalid
    }

    public enum PinType
    {
        Unassigned,
        ChipInputPin,
        ChipOutputPin,
        SubChipInputPin,
        SubChipOutputPin
    }

    public enum PinNameDisplayMode
    {
        Always,
        Hover,
        Toggle,
        Never
    }
}
