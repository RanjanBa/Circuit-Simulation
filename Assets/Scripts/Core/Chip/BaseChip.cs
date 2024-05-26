using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CircuitSimulation.Plugins;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public abstract class BaseChip : MonoBehaviour
    {
        public event Action<BaseChip> chipDeleted;
        public MouseInteraction<BaseChip> MouseInteraction { get; protected set; }
        public ChipDescription DescriptionOfChip { get; protected set; }
        public string NameOfChip => DescriptionOfChip.name;
        public ReadOnlyCollection<Pin> InputPins { get; private set; }
        public ReadOnlyCollection<Pin> OutputPins { get; private set; }
        public ReadOnlyCollection<Pin> AllPins { get; private set; }
        public int ID { get; private set; }

        public Vector2 Size { get; protected set; }
        
        private Dictionary<int, Pin> m_pinsByID;

        private void Initialize(ChipDescription _description, int _id)
        {
            DescriptionOfChip = _description;
            ID = _id;
            m_pinsByID = new Dictionary<int, Pin>();
        }

        protected void SetPins(Pin[] _inputPins, Pin[] _outputPins)
        {
            InputPins = new ReadOnlyCollection<Pin>(_inputPins);
            OutputPins = new ReadOnlyCollection<Pin>(_outputPins);
            AllPins = new ReadOnlyCollection<Pin>(_inputPins.Concat(_outputPins).ToArray());

            foreach (var _pin in _inputPins)
            {
                m_pinsByID.Add(_pin.ID, _pin);
            }

            foreach (var _pin in _outputPins)
            {
                m_pinsByID.Add(_pin.ID, _pin);
            }
        }

        public Pin GetPinByID(int _id)
        {
            if (m_pinsByID.ContainsKey(_id))
            {
                return m_pinsByID[_id];
            }

            Debug.LogWarning(string.Format("No pin is found with id {0}", _id));
            return null;
        }

        public abstract ChipInfo GetChipInfo();

        public virtual void Load(ChipDescription _description, ChipInfo _chipInfo)
        {
            Initialize(_description, _chipInfo.id);
        }

        public virtual void StartPlacing(ChipDescription _description, int _id)
        {
            Initialize(_description, _id);
        }

        public virtual void FinishPlacing() { }

        public virtual Bounds GetBounds()
        {
            return new Bounds(Vector2.zero, Vector2.one);
        }

        public virtual void Delete()
        {
            if (AllPins != null)
            {
                foreach (var _pin in AllPins)
                {
                    _pin.NotifyOfDeletion();
                }
            }

            chipDeleted?.Invoke(this);
            Destroy(gameObject);
        }

        public virtual void SetHighlightState(bool _isHighlighted) { }

        public virtual void NotifyMoved() { }
    }
}
