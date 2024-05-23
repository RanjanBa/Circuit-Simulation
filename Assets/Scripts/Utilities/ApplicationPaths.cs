using System.IO;
using UnityEngine;

namespace CircuitSimulation.Utilities
{
    public static class ApplicationPaths
    {
        private const string MAJOR_VERSION_NAME = "Version 1.0";

        public static string PERSISTENT_DATA_PATH => Application.persistentDataPath;
        public static string PROJECTS_PATH => Path.Combine(PERSISTENT_DATA_PATH, MAJOR_VERSION_NAME);
        public static string DELETED_PROJECTS_PATH => Path.Combine(PERSISTENT_DATA_PATH, MAJOR_VERSION_NAME);
        public static string GetProjectPath(string _projectName) => Path.Combine(PROJECTS_PATH, _projectName);
        public static string GetDeletedProjectPath(string _projectName) => Path.Combine(DELETED_PROJECTS_PATH, _projectName);

        public static void CreateDirectory(string _directoryPath)
        {
            Directory.CreateDirectory(_directoryPath);
        }
    }
}
