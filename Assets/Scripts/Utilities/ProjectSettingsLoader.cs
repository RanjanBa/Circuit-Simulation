using System.Collections.Generic;
using System.IO;
using CircuitSimulation.Core;
using CircuitSimulation.Utilities;
using Newtonsoft.Json;
using UnityEngine;

namespace CircuitSimulation
{
    public static class ProjectSettingsLoader
    {
        static string ProjectSettingsSavePath(string _projectName) => Path.Combine(ApplicationPaths.GetProjectPath(_projectName), "ProjectSettings.json");

        static ProjectSettings GetDefaultProjectSettings(string _projectName)
        {
            ProjectSettings settings = new ProjectSettings(_projectName);
            settings.SetStarredState(BuiltinChipNames.AndChip, true, false);
            settings.SetStarredState(BuiltinChipNames.NotChip, true, false);

            DisplayOptions displayOptions = new DisplayOptions()
            {
                mainChipPinNameDisplayMode = DisplayOptions.PinNameDisplayMode.Hover,
                subChipPinNameDisplayMode = DisplayOptions.PinNameDisplayMode.Hover,
                showCursorGuide = DisplayOptions.ToggleState.Off
            };

            settings.creationTime = System.DateTime.Now;
            settings.UpdateDisplayOptions(displayOptions, _autosave: false);
            settings.buildVersion = Application.version;
            return settings;
        }

        public static bool TryLoadProjectSettings(string _projectName, out ProjectSettings _projectSettings)
        {
            string path = ProjectSettingsSavePath(_projectName);

            if (File.Exists(path))
            {
                using StreamReader reader = new(path);
                string saveString = reader.ReadToEnd();
                _projectSettings = JsonConvert.DeserializeObject<ProjectSettings>(saveString);
                return true;
            }
            _projectSettings = GetDefaultProjectSettings(_projectName);

            return false;
        }

        public static ProjectSettings LoadProjectSettings(string _projectName)
        {
            TryLoadProjectSettings(_projectName, out ProjectSettings projectSettings);
            return projectSettings;
        }

        public static ProjectSettings[] LoadAllProjectSettings()
        {
            List<ProjectSettings> allProjectSettings = new List<ProjectSettings>();

            string savePath = ApplicationPaths.PROJECTS_PATH;
            ApplicationPaths.CreateDirectory(savePath);
            string[] projectPaths = Directory.GetDirectories(savePath);

            foreach (var projectPath in projectPaths)
            {
                string projectName = Path.GetFileName(projectPath);

                if (TryLoadProjectSettings(Path.GetFileNameWithoutExtension(projectName), out ProjectSettings projectSettings))
                {
                    allProjectSettings.Add(projectSettings);
                }
            }

            return allProjectSettings.ToArray();
        }

        public static void SaveProjectSettings(ProjectSettings _settings)
        {
            string path = ProjectSettingsSavePath(_settings.projectName);
            string saveString = JsonConvert.SerializeObject(_settings, Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using StreamWriter writer = new StreamWriter(path);
            writer.Write(saveString);
        }

        public static void CreateProject(string _projectName)
        {
            ProjectSettings projectSettings = GetDefaultProjectSettings(_projectName);
            SaveProjectSettings(projectSettings);
        }

        public static void DeleteProject(string _projectName)
        {
            string path = ApplicationPaths.GetProjectPath(_projectName);
            string deleteDirectoryPath = ApplicationPaths.DELETED_PROJECTS_PATH;
            Directory.CreateDirectory(deleteDirectoryPath);

            string moveToPath = FileHelper.EnsureUniqueDirectoryName(Path.Combine(deleteDirectoryPath, _projectName));
            Directory.Move(path, moveToPath);

#if UNITY_EDITOR
            Debug.Log(path);
            Debug.Log(moveToPath);
#endif
        }

        public static void CreateCopyOfProject(string _originalName, string _copyName)
        {
            string newPath = ApplicationPaths.GetProjectPath(_originalName);
            Directory.CreateDirectory(newPath);

            FileHelper.CopyDirectory(ApplicationPaths.GetProjectPath(_originalName), newPath, true);
            ProjectSettings projectSettings = LoadProjectSettings(_copyName);
            projectSettings.creationTime = System.DateTime.Now;
            projectSettings.UpdateProjectName(_copyName, false);
            projectSettings.Save();
        }

        public static void RenameProject(string _originalName, string _newName)
        {
            string oldPath = ApplicationPaths.GetProjectPath(_originalName);
            string newPath = ApplicationPaths.GetProjectPath(_newName);

            Directory.Move(oldPath, newPath);

            ProjectSettings projectSettings = LoadProjectSettings(_newName);
            projectSettings.UpdateProjectName(_newName);
        }
    }
}
