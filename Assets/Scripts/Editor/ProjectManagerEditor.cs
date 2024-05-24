using UnityEngine;
using UnityEditor;
using CircuitSimulation.Core;
using CircuitSimulation.Utilities;

namespace CircuitSimulation.EditorInspector
{
    [CustomEditor(typeof(ProjectManager))]
    public class ProjectManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Open Save Folder"))
            {
                EditorUtility.RevealInFinder(ApplicationPaths.PERSISTENT_DATA_PATH);
            }

            using EditorGUI.DisabledGroupScope scope = new(!Application.isPlaying);
            if (GUILayout.Button("Resave All"))
            {
                if (Application.isPlaying)
                {
                    (target as ProjectManager).ResaveAll();
                }
            }
        }
    }
}
