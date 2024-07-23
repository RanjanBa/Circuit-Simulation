using System.Collections.Generic;

namespace CircuitSimulation.Utilities
{
    public class ProjectSettings
    {
        private List<string> m_allCreatedChips;
        private List<string> m_starredChips;

        public string ProjectName;
        public string BuildVersion;
        public System.DateTime CreationTime;

        public ProjectSettings(string _projectName)
        {
            ProjectName = _projectName;
        }

        public void AddNewChip(string _chipName, bool _starByDefault = true)
        {
            m_allCreatedChips ??= new List<string>();
            m_allCreatedChips.Add(_chipName);
            if (_starByDefault)
            {
                SetStarredState(_chipName, true, autosave: false);
            }

            Save();
        }
    }
}
