using System;
using System.Linq;
using CircuitSimulation.Plugins;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CircuitSimulation.Core
{
    public class ChipMover : ControllerBase
    {
        private bool m_readyToStartMovingChips;
        private bool m_isMovingChips;
        private BaseChip[] m_chipsToMove;
        private Vector2[] m_chipStartPositions;
        private Vector2 m_mouseDragStartPos;

        public event Action chipsMoved;

        private void LateUpdate()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            Mouse _mouse = Mouse.current;
            Vector2 _delta = MouseHelper.GetMouseWorldPosition() - m_mouseDragStartPos;

            if (_delta.magnitude > 0.0001f)
            {
                if (m_readyToStartMovingChips)
                {
                    m_readyToStartMovingChips = false;
                    m_isMovingChips = true;
                    InitializeChipsToMove();
                }

                if (m_isMovingChips)
                {
                    for (int i = 0; i < m_chipsToMove.Length; i++)
                    {
                        Vector2 _targetPos = m_chipStartPositions[i] + _delta;
                        m_chipsToMove[i].transform.position = _targetPos.WithZ(RenderOrder.CHIP_MOVING);
                    }
                    OnChipsMoved(m_chipsToMove);
                }
            }

            if (_mouse.leftButton.wasReleasedThisFrame)
            {
                m_readyToStartMovingChips = false;
                if (m_isMovingChips)
                {
                    if (IsValidPostionForMovingChips())
                    {
                        StopMoving();
                    }
                    else
                    {
                        CancelMove();
                    }
                }
            }

            if (_mouse.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CancelMove();
            }
        }

        private void InitializeChipsToMove()
        {
            m_chipsToMove = m_chipCreator.ChipSelector.SelectedChips.ToArray();
            if (m_chipsToMove.Length > 0)
            {
                m_chipsToMove[0].chipDeleted += ChipDeletedWhileMoving;
                m_chipStartPositions = m_chipsToMove.Select(_chip => (Vector2)_chip.transform.position).ToArray();
            }
            m_isMovingChips = m_chipsToMove.Length > 0;
        }

        private void ChipDeletedWhileMoving(BaseChip _chip)
        {
            StopMoving();
        }

        private void StopMoving()
        {
            if (!m_isMovingChips) return;

            if (m_chipsToMove != null)
            {
                foreach (BaseChip _chip in m_chipsToMove)
                {
                    _chip.chipDeleted -= ChipDeletedWhileMoving;
                    Vector2 _curPos = _chip.transform.position;
                    _chip.transform.position = _curPos.WithZ(RenderOrder.CHIP);
                }
            }

            m_isMovingChips = false;
            m_chipsToMove = null;
            m_chipStartPositions = null;
        }

        private bool IsValidPostionForMovingChips()
        {
            if (m_chipsToMove == null) return false;
            return m_chipsToMove.All(_chip => m_chipCreator.ChipPlacer.IsValidPlacement(_chip));
        }

        private void CancelMove()
        {
            if (!m_isMovingChips) return;

            if (m_chipsToMove != null)
            {
                for (int i = 0; i < m_chipsToMove.Length; i++)
                {
                    m_chipsToMove[i].transform.position = m_chipStartPositions[i];
                }
            }

            OnChipsMoved(m_chipsToMove);
            StopMoving();
        }

        private void OnChipsMoved(BaseChip[] _chips)
        {
            foreach (var _chip in _chips)
            {
                _chip.NotifyMoved();
            }

            chipsMoved?.Invoke();
        }

        private void OnChipPressed(BaseChip _chip)
        {
            if (!m_chipCreator.CanEdit) return;

            m_readyToStartMovingChips = true;
            m_mouseDragStartPos = MouseHelper.GetMouseWorldPosition();
        }

        private void OnSubChipAdded(BaseChip _chip)
        {
            if (_chip.MouseInteraction == null) return;

            _chip.MouseInteraction.leftMouseDown += OnChipPressed;
        }

        public override void SetUp(ChipCreator _creator)
        {
            base.SetUp(_creator);
            _creator.subChipAdded += OnSubChipAdded;
            _creator = null;
        }

        public override bool IsBusy()
        {
            return m_isMovingChips;
        }
    }
}