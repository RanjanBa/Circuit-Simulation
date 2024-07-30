using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CircuitSimulation.Core
{
    public class ChipDeleter : ControllerBase
    {
        private void Update()
        {
            if (m_chipCreator.CanEdit)
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            if (Keyboard.current.backspaceKey.wasPressedThisFrame)
            {
                DeleteSelectedChips();
            }
        }

        private void DeleteSelectedChips()
        {
            foreach (var _chip in m_chipCreator.ChipSelector.SelectedChips)
            {
                _chip.Delete();
            }
        }
    }
}