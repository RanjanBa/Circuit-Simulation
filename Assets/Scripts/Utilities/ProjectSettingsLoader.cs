using System.IO;
using CircuitSimulation.Utilities;

namespace CircuitSimulation
{
    public static class ProjectSettingsLoader
    {
        static string ProjectSettingsSavePath(string _projectName) => Path.Combine(ApplicationPaths.GetProjectPath(_projectName), "ProjectSettings.json");

        public static bool TryLoadProjectSettings(string _projectName, out ProjectSettings _projectSettings) {
            string path = ProjectSettingsSavePath(_projectName);

            if(File.Exists(path)) {
                using(StreamReader reader = new(path)) {
                    string saveString = reader.ReadToEnd();
                    // _projectSettings = JsonConvert
                }
            }
        }
    }
}
