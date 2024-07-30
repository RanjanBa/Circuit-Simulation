using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.InputSystem;
using CircuitSimulation.Plugins;

namespace CircuitSimulation.Core
{
    public class ChipSelector : ControllerBase
    {
        [SerializeField]
        private Transform m_selectionBox;

        private List<BaseChip> m_selectedChips;
        private Vector2 m_selectionStartPos;
        private bool m_isBoxSelecting;
        private bool m_isChipSelectedThisFrame;

        public event System.Action<BaseChip> chipSelected;
        public int NumberOfSelectedChips => m_selectedChips.Count;
        public bool IsBoxSelecting => m_isBoxSelecting;
        public ReadOnlyCollection<BaseChip> SelectedChips => new ReadOnlyCollection<BaseChip>(m_selectedChips);

        public override bool IsBusy() => m_isBoxSelecting;

        private void LateUpdate()
        {
            HandleInput();
            DrawBoxSelection();
        }

        private void HandleInput()
        {
            Mouse _mouse = Mouse.current;
            Keyboard _keyboard = Keyboard.current;

            // A catch-all for deselecting chips when clicking anywhere (other than on a chip)
            if (_mouse.leftButton.wasPressedThisFrame && !m_isChipSelectedThisFrame && !m_chipCreator.ChipPlacer.IsBusy())
            {
                DeselectAll();
            }

            if (m_isBoxSelecting)
            {
                if (_mouse.leftButton.wasReleasedThisFrame)
                {
                    FinishBoxSelection();
                }
                else if (_mouse.rightButton.wasPressedThisFrame || _keyboard.escapeKey.wasPressedThisFrame || m_chipCreator.WirePlacer.IsCreatingWire)
                {
                    CancelBoxSelection();
                }
            }

            m_isChipSelectedThisFrame = false;
        }

        private void DrawBoxSelection()
        {
            if (!m_isBoxSelecting) return;

            Vector2 selectionBoxEndPos = MouseHelper.GetMouseWorldPosition();
            Vector2 selectionBoxSize = m_selectionStartPos - selectionBoxEndPos;
            Vector2 centre = (m_selectionStartPos + selectionBoxEndPos) / 2;
            m_selectionBox.position = new Vector3(centre.x, centre.y, RenderOrder.CHIP_MOVING);
            m_selectionBox.localScale = new Vector3(Mathf.Abs(selectionBoxSize.x), Mathf.Abs(selectionBoxSize.y), 1);
        }

        private void FinishBoxSelection()
        {
            Vector2 _selectionBoxSize = m_selectionBox.localScale;

            if (_selectionBoxSize.magnitude > 0.01f)
            {
                Vector2 selectionBoxMin = (Vector2)m_selectionBox.transform.position - _selectionBoxSize / 2;
                Vector2 selectionBoxMax = (Vector2)m_selectionBox.transform.position + _selectionBoxSize / 2;

                foreach (BaseChip _chip in m_chipCreator.allSubChips)
                {
                    if (_chip is BusDisplay)
                    {
                        continue;
                    }
                    Vector2 chipBoundsMin = (Vector2)_chip.transform.position - _chip.Size / 2;
                    Vector2 chipBoundsMax = (Vector2)_chip.transform.position + _chip.Size / 2;

                    if (BoundsOverlap2D(selectionBoxMin, selectionBoxMax, chipBoundsMin, chipBoundsMax))
                    {
                        AddChipToSelection(_chip);
                    }

                }
            }
            CancelBoxSelection();

            bool BoundsOverlap2D(Vector2 minA, Vector2 maxA, Vector2 minB, Vector2 maxB)
            {
                return minA.x < maxB.x && minB.x < maxA.x && minA.y < maxB.y && minB.y < maxA.y;
            }
        }

        private void Deselect(BaseChip _chip)
        {
            _chip.SetHighlightState(false);
            if (m_selectedChips.Contains(_chip))
            {
                m_selectedChips.Remove(_chip);
            }
        }

        private void DeselectAll()
        {
            foreach (var _chip in m_selectedChips)
            {
                _chip.SetHighlightState(false);
            }
            m_selectedChips.Clear();
        }

        private void CancelBoxSelection()
        {
            m_selectionBox.gameObject.SetActive(false);
            m_selectionBox.localScale = Vector3.zero;
            m_isBoxSelecting = false;
        }

        private void LeftMouseDownInWorkArea(WorkArea workArea)
        {
            if (!m_chipCreator.ChipPlacer.IsBusy())
            {
                DeselectAll();
                StartBoxSelection();
            }
        }

        private void StartBoxSelection()
        {
            m_isBoxSelecting = true;
            m_selectionStartPos = MouseHelper.GetMouseWorldPosition();
            m_selectionBox.gameObject.SetActive(true);
        }

        private void AddAllChipsToSelection(BaseChip[] _chips)
        {
            foreach (BaseChip _chip in _chips)
            {
                AddChipToSelection(_chip);
            }
        }

        private void AddChipToSelection(BaseChip _chip)
        {
            m_isChipSelectedThisFrame = true;
            _chip.SetHighlightState(true);
            if (!m_selectedChips.Contains(_chip))
            {
                m_selectedChips.Add(_chip);
                chipSelected?.Invoke(_chip);
            }
        }

        private bool IsSelected(BaseChip _chip)
        {
            return m_selectedChips.Contains(_chip);
        }

        private void OnSubChipAdded(BaseChip _chip)
        {
            _chip.chipDeleted -= OnChipDeleted;
            _chip.chipDeleted += OnChipDeleted;
            if (_chip.MouseInteraction is not null)
            {
                _chip.MouseInteraction.leftMouseDown += OnChipPressedLeftMouse;
                _chip.MouseInteraction.rightMouseDown += OnChipPressedRightMouse;
            }
            Deselect(_chip);
        }

        private void OnChipCreated(BaseChip _chip, bool _loadingFromFile)
        {
            if (!_loadingFromFile)
            {
                DeselectAll();
                AddAllChipsToSelection(m_chipCreator.ChipPlacer.AllChipsInPlacementMode);
            }
            _chip.chipDeleted += OnChipDeleted;
        }

        void OnChipDeleted(BaseChip _chip)
        {
            Deselect(_chip);
        }

        private void OnChipPressedLeftMouse(BaseChip _chip)
        {
            if (!IsSelected(_chip))
            {
                DeselectAll();
                AddChipToSelection(_chip);
            }
            m_isChipSelectedThisFrame = true;
        }

        void OnChipPressedRightMouse(BaseChip _chip)
        {
            DeselectAll();
            AddChipToSelection(_chip);
            m_isChipSelectedThisFrame = true;
        }

        public override void SetUp(ChipCreator _creator)
        {
            base.SetUp(_creator);

            m_chipCreator.subChipAdded += OnSubChipAdded;
            m_chipCreator.ChipPlacer.startedPlacingOrLoadingChip += OnChipCreated;
            m_chipCreator.WorkArea.workAreaMouseInteraction.leftMouseDown += LeftMouseDownInWorkArea;
            m_selectedChips = new List<BaseChip>();
            m_selectionBox.gameObject.SetActive(false);
        }
    }
}