using System.Collections.Generic;
using UnityEngine;

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

        private float CalculateSpacing(int _idx, int _count, float _boundsSize)
        {
            return (_boundsSize + m_MULTI_CHIP_SPACING) * (_idx - (_count - 1) / 2f);
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
            OnStarte
        }

        public bool IsValidPlacement(BaseChip _chip)
        {
            Bounds _bounds = _chip.GetBounds();

            // foreach (BaseChip _chip in m_chipCreator.All)
            // {

            // }

            return true;
        }
    }
}
