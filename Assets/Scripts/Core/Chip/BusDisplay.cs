using System.Drawing;
using System.Linq;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public class BusDisplay : BaseChip
    {
        [Header("References")]
        [SerializeField]
        private Pin m_standalonePinPrefab;
        [SerializeField]
        private Wire m_wirePrefab;
        [SerializeField]
        private Palette m_palette;
        [SerializeField]
        private MeshRenderer m_highlight;

        private Pin m_pinA;
        private Pin m_pinB;

        public PlacementState CurrentPlacementState { get; private set; }
        public Wire Wire { get; private set; }

        private Pin CreatePin(Vector3 _position, bool _isInputPin)
        {
            Pin pin = Instantiate(m_standalonePinPrefab, _position, Quaternion.identity, parent: transform);
            pin.IsBusPin = true;
            pin.transform.localScale = Vector3.one * DisplaySettings.PIN_SIZE;
            PinDescription pinDescription = new PinDescription()
            {
                name = "Pin",
                id = _isInputPin ? 0 : 1
            };
            pin.SetUp(this, pinDescription, _isInputPin ? PinType.SubChipInputPin : PinType.SubChipOutputPin, m_palette.GetDefaultVoltageColors());
            return pin;
        }

        private Wire CreateWire()
        {
            return Instantiate(m_wirePrefab, transform.position, Quaternion.identity, parent: transform);
        }

        public override ChipInfo GetChipInfo()
        {
            return new ChipInfo()
            {
                name = NameOfChip,
                id = ID,
                points = Wire.AnchorPoints.Select(_p => Point.ToPoint(_p)).ToArray()
            };
        }

        public override void Load(ChipDescription _description, ChipInfo _chipInfo)
        {
            transform.position = Point.ToVector(_chipInfo.points[0]);
            Vector3[] _points = _chipInfo.points.Select(_p => Point.ToVector(_p).WithZ(RenderOrder.CHIP_MOVING)).Reverse().ToArray();
            StartPlacing(_description, _chipInfo.id);
            m_pinA.transform.position = _points[0];
            PlacePin();
            for (int i = 0; i < _points.Length - 1; i++)
            {
                Wire.AddAnchorPoint(_points[i]);
            }

            m_pinB.transform.position = _points[_points.Length - 1];
            PlacePin();
            FinishPlacing();
        }

        public override void StartPlacing(ChipDescription _description, int _id)
        {
            base.StartPlacing(_description, _id);
            m_pinA = CreatePin(transform.position, true);
            CurrentPlacementState = PlacementState.PlacingFirstPin;
            m_highlight.transform.localScale = (Vector2.one * DisplaySettings.PIN_SIZE).WithZ(1);
        }

        public override void SetHighlightState(bool _isHighlighted)
        {
            m_highlight.gameObject.SetActive(_isHighlighted);
        }

        public override Bounds GetBounds()
        {
            if (CurrentPlacementState == PlacementState.PlacingFirstPin)
            {
                return new Bounds(m_pinA.transform.position, Vector3.one * 0.1f);
            }
            else if (CurrentPlacementState == PlacementState.PlacingWire)
            {
                return new Bounds(m_pinB.transform.position, Vector3.one * 0.1f);
            }
            return new Bounds(Vector3.zero, Vector3.zero);
        }

        public override void FinishPlacing()
        {
            base.FinishPlacing();
            Wire.transform.position = Wire.transform.position.WithZ(RenderOrder.WIRE_LOW);
            m_pinA.transform.position = m_pinA.transform.position.WithZ(RenderOrder.CHIP_PIN);
            m_pinB.transform.position = m_pinB.transform.position.WithZ(RenderOrder.CHIP_PIN);
        }

        public void PlacePin()
        {
            if (CurrentPlacementState == PlacementState.PlacingFirstPin)
            {
                CurrentPlacementState = PlacementState.PlacingWire;
                Wire = CreateWire();
                Wire.AddAnchorPoint(m_pinA.transform.position);
                m_pinB = CreatePin(m_pinA.transform.position, false);
            }
            else if (CurrentPlacementState == PlacementState.PlacingWire)
            {
                CurrentPlacementState = PlacementState.Finished;
                Wire.AddAnchorPoint(m_pinB.transform.position);
                Wire.ConnectWireToPins(m_pinA, m_pinB);
                Wire.wireDeleted += (w) => Delete();
                SetPins(new Pin[] { m_pinB }, new Pin[] { m_pinA });
            }
        }

        public void UpdateWirePlacementPreview(Vector3 _position)
        {
            Wire.DrawToPoint(_position.WithZ(RenderOrder.WIRE_EDIT));
            m_pinB.transform.position = _position.WithZ(RenderOrder.CHIP_MOVING);
            m_highlight.transform.position = _position.WithZ(m_highlight.transform.position.z);
        }

        public void UpdatePrevBusPoint(Vector2 _point)
        {
            int index = Wire.AnchorPoints.Count - 1;
            Wire.UpdateAnchorPoint(index, _point);
            if (index == 0)
            {
                m_pinA.transform.position = _point.WithZ(RenderOrder.CHIP_MOVING);
            }
        }

        public void AddWireAnchor()
        {
            Wire.AddAnchorPoint(m_pinB.transform.position);
        }
    }
}