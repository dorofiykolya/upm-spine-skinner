using UnityEditor;
using UnityEngine;

namespace Spine.Unity.Skinner
{
    [CustomEditor(typeof(SpineSkinnerEditComponent))]
    public class SpineSkinnerEditComponentEditor : UnityEditor.Editor
    {
        private SpineSkinnerEditComponent _target;

        private readonly string _applySkinLabel = "Apply skin";

        public void OnEnable()
        {
            if (_target == null)
            {
                _target = target as SpineSkinnerEditComponent;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (GUILayout.Button(_applySkinLabel))
            {
                _target.Apply();
            }
        }
    }
}