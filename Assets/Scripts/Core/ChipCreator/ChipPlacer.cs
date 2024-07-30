using System.Collections.Generic;
using System.Linq;
using CircuitSimulation.Plugins;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CircuitSimulation.Core
{
    public class ChipPlacer : ControllerBase
    {
        private const float m_MULTI_CHIP_SPACING = 0.1f;

        [SerializeField]
        private ChipOverrides m_overrides;
        [SerializeField]
        private BaseChip m_chipPrefab;
        [SerializeField]
        private Transform m_childChipHolder;

        private List<BaseChip> m_activeChips;
        private ChipDescription lastCreatedChipDescription;
        private Dictionary<string, BaseChip> m_chipOverrideLookup;
        private List<Vector2> m_busPlacementPoints;
        private System.Random m_random;

        public bool IsPlacingChip => m_activeChips.Count > 0;
        public override bool IsBusy() => IsPlacingChip;

        public event System.Action<BaseChip, bool> startedPlacingOrLoadingChip;
        public event System.Action<BaseChip, bool> finishedPlacingOrLoadingChip;

        public BaseChip[] AllChipsInPlacementMode => m_activeChips.ToArray();

        private void Update()
        {
            if (!IsPlacingChip) return;

            bool _isPlacingBus = m_activeChips[0].NameOfChip is BuiltinChipNames.BusName;

            if (_isPlacingBus)
            {
                HandleBusPlacementInput();
            }
            else
            {
                HandleChipPlacementInput();
            }

            HandleChipCancellationInput();
        }

        private void HandleBusPlacementInput()
        {
            BusDisplay[] _busChips = m_activeChips.Select(c => c as BusDisplay).ToArray();
            PlacementState _placementState = _busChips[0].CurrentPlacementState;
            bool _shiftKeyDown = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;

            bool _snapping = _shiftKeyDown && m_busPlacementPoints != null && m_busPlacementPoints.Count > 0;
            Vector2 _snapOrigin = _snapping ? m_busPlacementPoints[^1] : Vector2.zero;

            Vector2 _mousePos = MouseHelper.CalculateAxisSnappedMousePosition(_snapOrigin, _snapping);

            // Update position while placing first pin
            if (_placementState == PlacementState.PlacingFirstPin)
            {
                SetActiveChipsPosition(_mousePos);
            }

            // Placing wire
            if (_placementState == PlacementState.PlacingWire)
            {
                Vector2 _offset = _mousePos - m_busPlacementPoints[^1];
                Vector2 _dir;
                if (_offset.sqrMagnitude > 0)
                {
                    _dir = _offset.normalized;
                }
                else
                {
                    _dir = m_busPlacementPoints.Count > 1 ? (m_busPlacementPoints[^1] - m_busPlacementPoints[^2]).normalized : Vector2.right;
                }

                float _spacing = m_MULTI_CHIP_SPACING + _busChips[0].GetBounds().size.y;

                for (int i = 0; i < _busChips.Length; i++)
                {
                    float _ti = i - (_busChips.Length - 1) / 2f;
                    Vector2 _offsetFromLast = _mousePos - m_busPlacementPoints[^1];
                    Vector2 _dirFromLast = _offsetFromLast.normalized;

                    Vector2 _busPoint = _mousePos + new Vector2(_dir.y, -_dir.x) * _ti * _spacing;
                    Vector2 _prevBusPointDesired = m_busPlacementPoints[^1] + new Vector2(_dir.y, -_dir.x) * _ti * _spacing;
                    if (m_busPlacementPoints.Count > 1)
                    {
                        Vector2 _dirOld = (m_busPlacementPoints[^1] - m_busPlacementPoints[^2]).normalized;
                        Vector2 _prevBusPoint = m_busPlacementPoints[^1] + new Vector2(_dirOld.y, -_dirOld.x) * _ti * _spacing;
                        var (_isIntersect, _intersectionPoint) = MathUtility.LineIntersectLine(_prevBusPoint, _prevBusPoint + _dirOld, _busPoint, _busPoint + _dir);
                        if (_isIntersect)
                        {
                            _prevBusPointDesired = _intersectionPoint;
                        }
                        // Handle moving back on self (todo, write a comment that makes sense...)
                        float _xt = Mathf.InverseLerp(-0.5f, -0.95f, Vector2.Dot(_dirOld, _dir));
                        _prevBusPointDesired = Vector2.Lerp(_prevBusPointDesired, _prevBusPoint, _xt);

                        // Flip order of points if moving sharply back on self
                        if (Vector2.Dot(_dirOld, _dir) < -0.707f)
                        {
                            _busPoint = _mousePos + new Vector2(_dir.y, -_dir.x) * (-_ti) * _spacing;
                        }
                    }
                    _busChips[i].UpdatePrevBusPoint(_prevBusPointDesired);


                    Wire wire = _busChips[i].Wire;


                    _busChips[i].UpdateWirePlacementPreview(_busPoint);
                }

                // Shift left click to add wire points when placing bus
                if (Mouse.current.leftButton.wasPressedThisFrame && _shiftKeyDown)
                {
                    m_busPlacementPoints.Add(_mousePos);
                    foreach (var b in _busChips)
                    {
                        b.AddWireAnchor();
                    }
                }
            }

            if ((Mouse.current.leftButton.wasPressedThisFrame && !_shiftKeyDown) || Keyboard.current.enterKey.wasPressedThisFrame)
            {
                if (CanPlaceActiveChips())
                {
                    for (int i = 0; i < _busChips.Length; i++)
                    {
                        _busChips[i].PlacePin();
                    }
                    if (_placementState == PlacementState.PlacingWire)
                    {
                        FinishPlacingActiveChips();
                    }
                    m_busPlacementPoints = new List<Vector2>() { _mousePos };
                }
            }
        }

        private void HandleChipPlacementInput()
        {
            Vector3 _chipPos = MouseHelper.GetMouseWorldPosition(RenderOrder.CHIP_MOVING);
            SetActiveChipsPosition(_chipPos);

            if (Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
            {
                if (CanPlaceActiveChips())
                {
                    FinishPlacingActiveChips();

                    if (Keyboard.current.leftShiftKey.isPressed)
                    {
                        StartPlacingChips(lastCreatedChipDescription);
                        SetActiveChipsPosition(_chipPos);
                    }
                }
            }
        }

        private void HandleChipCancellationInput()
        {
            if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                DestroyActiveChips();
            }
        }

        private bool CanPlaceActiveChips()
        {
            foreach (var _chip in m_activeChips)
            {
                if (!IsValidPlacement(_chip)) return false;
            }

            return true;
        }

        private void StartPlacingChips(ChipDescription _description, int _id)
        {
            if (IsPlacingChip && m_activeChips[0].NameOfChip != _description.name)
            {
                DestroyActiveChips();
            }

            BaseChip _newChip = InstantiateChip(_description);
            m_activeChips.Add(_newChip);
            _newChip.chipDeleted += OnChipDeletedBeforePlacement;
            _newChip.StartPlacing(_description, _id);
            lastCreatedChipDescription = _description;

            OnStartedPlacingOrLoadingChip(_newChip, false);
        }

        private void OnStartedPlacingOrLoadingChip(BaseChip _chip, bool _isLoading)
        {
            startedPlacingOrLoadingChip?.Invoke(_chip, _isLoading);
        }

        private void OnFinishedPlacingOrLoadingChip(BaseChip _chip, bool _wasLoaded)
        {
            finishedPlacingOrLoadingChip?.Invoke(_chip, _wasLoaded);
        }

        private void OnChipDeletedBeforePlacement(BaseChip _chip)
        {
            if (m_activeChips.Contains(_chip))
            {
                m_activeChips.Remove(_chip);
            }
        }

        private void DestroyActiveChips()
        {
            if (IsPlacingChip)
            {
                foreach (var chip in m_activeChips)
                {
                    chip.chipDeleted -= OnChipDeletedBeforePlacement;
                    chip.Delete();
                }

                m_activeChips.Clear();
            }
        }

        private void SetActiveChipsPosition(Vector2 _center)
        {
            float boundsSize = m_activeChips[0].GetBounds().size.y;

            for (int i = 0; i < m_activeChips.Count; i++)
            {
                Vector3 pos = _center.WithZ(RenderOrder.CHIP_MOVING) + Vector3.down * CalculateSpacing(i, m_activeChips.Count, boundsSize);
                m_activeChips[i].transform.position = pos;
            }
        }

        private void FinishPlacingActiveChips()
        {
            if (!IsPlacingChip) return;

            foreach (var _chip in m_activeChips)
            {
                _chip.chipDeleted -= OnChipDeletedBeforePlacement;
                _chip.transform.position = _chip.transform.position.WithZ(RenderOrder.CHIP);
                _chip.FinishPlacing();
                OnFinishedPlacingOrLoadingChip(_chip, false);
            }

            m_activeChips.Clear();
        }

        private float CalculateSpacing(int _idx, int _count, float _boundsSize)
        {
            return (_boundsSize + m_MULTI_CHIP_SPACING) * (_idx - (_count - 1) / 2f);
        }

        private int GenerateID()
        {
            int id;

            while (true)
            {
                id = m_random.Next();

                if (!m_chipCreator.allSubChips.Any(_subChip => _subChip.ID == id))
                {
                    break;
                }
            }

            return id;
        }

        private BaseChip InstantiateChip(ChipDescription _description)
        {
            BaseChip prefab;
            if (!m_chipOverrideLookup.TryGetValue(_description.name, out prefab))
            {
                prefab = m_chipPrefab;
            }

            BaseChip chip = Instantiate(prefab, m_childChipHolder);
            return chip;
        }

        private bool BoundsOverlap2D(Bounds _a, Bounds _b)
        {
            if (_a.size.x * _a.size.y == 0 || _b.size.x * _b.size.y == 0)
            {
                return false;
            }
            bool overlapX = _b.min.x < _a.max.x && _b.max.x > _a.min.x;
            bool overlapY = _b.min.y < _a.max.y && _b.max.y > _a.min.y;
            return overlapX && overlapY;

        }

        public override void SetUp(ChipCreator _creator)
        {
            base.SetUp(_creator);
            m_random = new System.Random();
            m_chipOverrideLookup = m_overrides.CreateLookup();
            m_activeChips = new List<BaseChip>();
        }

        public void Load(ChipDescription _description, ChipInfo _instanceData)
        {
            BaseChip loadedChip = InstantiateChip(_description);
            loadedChip.Load(_description, _instanceData);
            OnStartedPlacingOrLoadingChip(loadedChip, true);
            OnFinishedPlacingOrLoadingChip(loadedChip, true);
        }

        public void StartPlacingChips(ChipDescription _description)
        {
            StartPlacingChips(_description, GenerateID());
        }

        public bool IsValidPlacement(BaseChip _chip)
        {
            Bounds _bounds = _chip.GetBounds();

            foreach (BaseChip _otherChip in m_chipCreator.allSubChips)
            {
                if (_chip != _otherChip && BoundsOverlap2D(_otherChip.GetBounds(), _bounds))
                {
                    return false;
                }
            }

            return !m_chipCreator.WorkArea.OutOfBounds(_bounds);
        }
    }
}
