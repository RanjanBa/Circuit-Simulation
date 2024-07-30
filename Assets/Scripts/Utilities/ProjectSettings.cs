using System.Collections.Generic;
using System.Collections.ObjectModel;
using CircuitSimulation.Core;
using Newtonsoft.Json;

namespace CircuitSimulation.Utilities
{
    public class ProjectSettings
    {
        [JsonProperty] private List<string> m_allCreatedChips;
        [JsonProperty] private List<string> m_starredChips;

        [JsonProperty] public string projectName;
        [JsonProperty] public string buildVersion;
        [JsonProperty] public System.DateTime creationTime;
        [JsonProperty] public DisplayOptions displayOptions;

        public ReadOnlyCollection<string> GetStarredChipNames => new ReadOnlyCollection<string>(m_starredChips);
        public ReadOnlyCollection<string> GetAllCreatedChipNames => new ReadOnlyCollection<string>(m_allCreatedChips);

        public ProjectSettings(string _projectName)
        {
            projectName = _projectName;
        }

        public void AddNewChip(string _chipName, bool _starByDefault = true)
        {
            m_allCreatedChips ??= new List<string>();
            m_allCreatedChips.Add(_chipName);
            if (_starByDefault)
            {
                SetStarredState(_chipName, true, _autosave: false);
            }

            Save();
        }

        public void RemoveChip(string _chipName)
        {
            m_starredChips.Remove(_chipName);
            m_allCreatedChips.Remove(_chipName);
            Save();
        }

        public void UpdateProjectName(string _newName, bool _autosave = true)
        {
            projectName = _newName;
            if (_autosave)
            {
                Save();
            }
        }

        public void UpdateChipName(string _oldName, string _newName)
        {
            int index = m_allCreatedChips.IndexOf(_oldName);
            m_allCreatedChips[index] = _newName;

            int starredIndex = m_starredChips.IndexOf(_oldName);
            if (starredIndex >= 0)
            {
                m_starredChips[starredIndex] = _newName;
            }

            Save();
        }

        public void UpdateDisplayOptions(DisplayOptions _displayOptions, bool _autosave = true)
        {
            displayOptions = _displayOptions;
            if (_autosave)
            {
                Save();
            }
        }

        public void SetStarredState(string _chipName, bool _star, bool _autosave = true)
        {
            m_starredChips ??= new List<string>();
            if (_star && !IsStarred(_chipName))
            {
                m_starredChips.Add(_chipName);
            }
            else if (!_star && IsStarred(_chipName))
            {
                m_starredChips.Remove(_chipName);
            }

            if (_autosave)
            {
                Save();
            }
        }

        public bool IsStarred(string _chipName)
        {
            if (m_starredChips is null)
            {
                return false;
            }

            return m_starredChips.Contains(_chipName);
        }

        public void Save()
        {
            ProjectSettingsLoader.SaveProjectSettings(this);
        }
    }
}
