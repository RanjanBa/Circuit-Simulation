using System.Collections.Generic;
using UnityEngine;
using CircuitSimulation.Utilities;

namespace CircuitSimulation.Core
{
    public class ChipInteraction : InteractionHandler
    {
        private const float DRAG_DEPTH = -50.0f;
        private const float CHIP_DEPTH = -0.2f;
        private const float CHIP_MOVE_THRESHOLD = 0.001f;

        public enum ChipState
        {
            None,
            PlacingNewChips,
            MovingOldChips,
            SelectingChips
        }

        [SerializeField]
        private event System.Action<Chip> m_onDeleteChip;

        [SerializeField]
        private BoxCollider2D m_chipArea;

        [SerializeField]
        private Transform m_chipHolder;

        [SerializeField]
        private LayerMask m_chipLayerMask;

        [SerializeField]
        private Material m_selectionBoxMaterial;

        [SerializeField]
        private float m_chipStackSpacing = 0.15f;

        [SerializeField]
        private float m_selectionBorderPadding = 0.1f;

        [SerializeField]
        private Color m_selectionBoxColor;

        [SerializeField]
        private Color m_invalidPlacementColor;

        private ChipState m_currentChipState;
        private List<Chip> m_newChipsToPlace;
        private List<Chip> m_selectedChips;
        private Vector2 m_selectionBoxStartPos;
        private Mesh m_selectionMesh;
        private Vector3[] m_selectedChipsOriginalPos;

        public List<Chip> AllChips { get; private set; }

        private void Awake()
        {
            m_newChipsToPlace = new List<Chip>();
            m_selectedChips = new List<Chip>();
            AllChips = new List<Chip>();
            MeshCreator.CreateQuadMesh(ref m_selectionMesh);
        }

        public override void OrderedUpdate()
        {
            switch (m_currentChipState)
            {
                case ChipState.None:
                    HandleSelection();
                    HandleDeletion();
                    break;
                case ChipState.PlacingNewChips:
                    HandleNewChipPlacement();
                    break;
                case ChipState.SelectingChips:
                    HandleSelectionBox();
                    break;
                case ChipState.MovingOldChips:
                    HandleChipMovement();
                    break;
            }
        }

        private void HandleSelection()
        {
            Vector2 _mouseWorldPos = InputHandler.MousePosToWorldPos;

            if (Input.GetMouseButtonDown(0) && !InputHandler.IsMouseOverUI)
            {
                RequestFocus();
                if (HasFocus)
                {
                    m_selectionBoxStartPos = _mouseWorldPos;
                    GameObject _gmOnMouse = InputHandler.GetObjectOnMousePos(m_chipLayerMask);

                    if (_gmOnMouse == null)
                    {
                        m_currentChipState = ChipState.SelectingChips;
                        m_selectedChips.Clear();
                    }
                    else
                    {
                        m_currentChipState = ChipState.MovingOldChips;
                        Chip _chipOnMouse = _gmOnMouse.GetComponent<Chip>();

                        if (!m_selectedChips.Contains(_chipOnMouse))
                        {
                            m_selectedChips.Clear();
                            m_selectedChips.Add(_chipOnMouse);
                        }

                        m_selectedChipsOriginalPos = new Vector3[m_selectedChips.Count];
                        for (int i = 0; i < m_selectedChips.Count; i++)
                        {
                            m_selectedChipsOriginalPos[i] = m_selectedChips[i].transform.position;
                        }
                    }
                }
            }
        }

        private void HandleDeletion()
        {
            if (InputHandler.AnyOfGivenKeysDown(KeyCode.Backspace, KeyCode.Delete))
            {
                for (int i = m_selectedChips.Count - 1; i >= 0; i--)
                {
                    DeleteChip(m_selectedChips[i]);
                    m_selectedChips.RemoveAt(i);
                }

                m_newChipsToPlace.Clear();
            }
        }

        private void HandleNewChipPlacement()
        {
            if (
                InputHandler.AnyOfGivenKeysDown(KeyCode.Escape, KeyCode.Backspace, KeyCode.Delete)
                || Input.GetMouseButtonDown(1)
            )
            {
                CancelNewChipPlacement();
            }
            else
            {
                Vector2 _mouseWorldPos = InputHandler.MousePosToWorldPos;
                float _offsetY = 0.0f;

                for (int i = 0; i < m_newChipsToPlace.Count; i++)
                {
                    Chip _newChip = m_newChipsToPlace[i];
                    _newChip.transform.position = _mouseWorldPos + Vector2.down * _offsetY;
                    SetChipDepth(m_newChipsToPlace[i], DRAG_DEPTH);
                    _offsetY += _newChip.BoundsSize.y + m_chipStackSpacing;
                }

                if (Input.GetMouseButtonDown(0) && IsSelectedChipsWithinPlacementArea())
                {
                    PlaceNewChips();
                }
            }
        }

        private void HandleSelectionBox()
        {
            Vector2 _mouseWorldPos = InputHandler.MousePosToWorldPos;

            if (Input.GetMouseButton(0))
            {
                Vector3 _pos =
                    (Vector3)(m_selectionBoxStartPos + _mouseWorldPos) / 2 + Vector3.back * 0.5f;
                Vector3 _scale = new Vector3(
                    Mathf.Abs(_mouseWorldPos.x - m_selectionBoxStartPos.x),
                    Mathf.Abs(_mouseWorldPos.y - m_selectionBoxStartPos.y),
                    1
                );
                m_selectionBoxMaterial.color = m_selectionBoxColor;
                Graphics.DrawMesh(
                    m_selectionMesh,
                    Matrix4x4.TRS(_pos, Quaternion.identity, _scale),
                    m_selectionBoxMaterial,
                    0
                );
            }

            if (Input.GetMouseButtonUp(0))
            {
                m_currentChipState = ChipState.None;

                Vector2 _boxSize = new Vector2(
                    Mathf.Abs(_mouseWorldPos.x - m_selectionBoxStartPos.x),
                    Mathf.Abs(_mouseWorldPos.y - m_selectionBoxStartPos.y)
                );

                Collider2D[] _allOverlapColliders = Physics2D.OverlapBoxAll(
                    (m_selectionBoxStartPos + _mouseWorldPos) / 2,
                    _boxSize,
                    0f,
                    m_chipLayerMask
                );

                m_selectedChips.Clear();

                foreach (Collider2D _col in _allOverlapColliders)
                {
                    Chip _chip = _col.GetComponent<Chip>();
                    if (_chip)
                    {
                        m_selectedChips.Add(_chip);
                    }
                }
            }
        }

        private void HandleChipMovement()
        {
            Vector2 _mouseWorldPos = InputHandler.MousePosToWorldPos;

            if (Input.GetMouseButton(0))
            {
                Vector2 _deltaMove = _mouseWorldPos - m_selectionBoxStartPos;

                for (int i = 0; i < m_selectedChips.Count; i++)
                {
                    m_selectedChips[i].transform.position =
                        (Vector2)m_selectedChipsOriginalPos[i] + _deltaMove;
                    SetChipDepth(m_selectedChips[i], DRAG_DEPTH + m_selectedChipsOriginalPos[i].z);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                m_currentChipState = ChipState.None;
                if (IsSelectedChipsWithinPlacementArea())
                {
                    Vector2 _deltaMove = _mouseWorldPos - m_selectionBoxStartPos;

                    if (m_selectedChips.Count > 1 && _deltaMove.magnitude < CHIP_MOVE_THRESHOLD)
                    {
                        GameObject _gmOnMouse = InputHandler.GetObjectOnMousePos(m_chipLayerMask);
                        Chip _chip = _gmOnMouse?.GetComponent<Chip>();
                        if (_chip)
                        {
                            m_selectedChips.Clear();
                            m_selectedChips.Add(_chip);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < m_selectedChips.Count; i++)
                        {
                            SetChipDepth(m_selectedChips[i], m_selectedChipsOriginalPos[i].z);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < m_selectedChips.Count; i++)
                    {
                        m_selectedChips[i].transform.position = m_selectedChipsOriginalPos[i];
                    }
                }
            }
        }

        private void DeleteChip(Chip _chip)
        {
            m_onDeleteChip?.Invoke(_chip);

            AllChips.Remove(_chip);
            Destroy(_chip.gameObject);
        }

        private void CancelNewChipPlacement()
        {
            for (int i = m_newChipsToPlace.Count - 1; i >= 0; i--)
            {
                Destroy(m_newChipsToPlace[i].gameObject);
            }

            m_newChipsToPlace.Clear();
            m_selectedChips.Clear();
            m_currentChipState = ChipState.None;
        }

        private bool IsSelectedChipsWithinPlacementArea()
        {
            float _bufferX = Pin.RADIUS + m_selectionBorderPadding * 0.75f;
            float _bufferY = m_selectionBorderPadding;
            Bounds _bounds = m_chipArea.bounds;

            for (int i = 0; i < m_selectedChips.Count; i++)
            {
                Chip _chip = m_selectedChips[i];
                float _left = _chip.transform.position.x - (_chip.BoundsSize.x + _bufferX) / 2;
                float _right = _chip.transform.position.x + (_chip.BoundsSize.x + _bufferX) / 2;
                float _top = _chip.transform.position.y + (_chip.BoundsSize.y + _bufferY) / 2;
                float _bottom = _chip.transform.position.y - (_chip.BoundsSize.y + _bufferY) / 2;

                if (
                    _left < _bounds.min.x
                    || _right > _bounds.max.x
                    || _top > _bounds.max.y
                    || _bottom < _bounds.min.y
                )
                {
                    return false;
                }
            }

            return true;
        }

        private void PlaceNewChips()
        {
            float _startDepth =
                AllChips.Count > 0 ? AllChips[AllChips.Count - 1].transform.position.z : 0f;

            for (int i = 0; i < m_newChipsToPlace.Count; i++)
            {
                SetChipDepth(
                    m_newChipsToPlace[i],
                    _startDepth + (m_newChipsToPlace.Count - i) * CHIP_DEPTH
                );
            }

            AllChips.AddRange(m_newChipsToPlace);
            m_selectedChips.Clear();
            m_newChipsToPlace.Clear();
            m_currentChipState = ChipState.None;
        }

        private void SetChipDepth(Chip _chip, float _depth)
        {
            _chip.transform.position = new Vector3(
                _chip.transform.position.x,
                _chip.transform.position.y,
                _depth
            );
        }
    }
}