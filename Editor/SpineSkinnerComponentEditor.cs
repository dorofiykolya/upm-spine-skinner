using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Spine.Unity.Skinner
{
    [CustomEditor(typeof(SpineSkinnerComponent))]
    public class SpineSkinnerComponentEditor : UnityEditor.Editor
    {
        private static bool _expandedSlotsView;
        private static bool _expandedEditor;
        private static string _matcher;
        private static int _editorCurrentIndexInEditorPopup;
        
        private SpineSkinnerComponent _target;
        private SpineSkinnerPreset _visualInfoPreset;

        public string[] _stringOptions;

        private readonly string _boxLayout = "Box";
        private readonly string _slotsArrayName = "_slots";
        private readonly string _slotsArrayRelativeSlotName = "Slot";
        private readonly string _slotsArrayRelativeKeyName = "Key";
        private readonly string _deleteBtnName = "X";
        private readonly string _skinLabelName = "Skin:";
        private readonly string _addNewSlotLabelName = "Add new slot for skinning";

        private void OnEnable()
        {
            _target = target as SpineSkinnerComponent;

            if (_visualInfoPreset == null)
                _visualInfoPreset = _target.SpineSkinner;

            if (_target == null ||
                _visualInfoPreset == null ||
                _visualInfoPreset.Variants == null ||
                _visualInfoPreset.Variants.Count == 0)
            {
                return;
            }

            _stringOptions = _visualInfoPreset.Variants.Select(x => x.Name).ToArray();

            for (int i = 0; i < _target.Slots.Count; i++)
            {
                var slot = _target.Slots[i];

                slot.CurrentIndexInEditorPopup =
                    (int)_visualInfoPreset.Variants?.FindIndex(y => y.Path == slot.CustomPath);

                slot.CustomPath =
                    _visualInfoPreset
                        .Variants[slot.CurrentIndexInEditorPopup > -1 ? slot.CurrentIndexInEditorPopup : 0].Path;
            }
        }

        private void DrawEditor()
        {
            if (_target == null ||
                _stringOptions == null ||
                _stringOptions.Length == 0 ||
                _target.Slots == null ||
                _target.Slots.Count == 0 ||
                _visualInfoPreset == null ||
                _visualInfoPreset.Variants == null)
            {
                return;
            }

            var isChanged = false;
            SerializedProperty listProperty = serializedObject.FindProperty(_slotsArrayName);

            _expandedSlotsView = EditorGUILayout.Foldout(_expandedSlotsView, "Slots-View");
            if (_expandedSlotsView)
            {
                EditorGUILayout.BeginVertical(_boxLayout);
                {
                    for (int i = 0; i < _target.Slots.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.BeginVertical(_boxLayout);
                            {
                                string indexName = (i + 1).ToString();
                                EditorGUILayout.LabelField(indexName, GUILayout.Width(20f));
                            }
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.BeginVertical(_boxLayout);
                            {
                                var slot = _target.Slots[i];
                                var listItemProperty = listProperty.GetArrayElementAtIndex(i);
                                var slotInListProperty =
                                    listItemProperty.FindPropertyRelative(_slotsArrayRelativeSlotName);
                                var keyInListProperty =
                                    listItemProperty.FindPropertyRelative(_slotsArrayRelativeKeyName);

                                slot.CurrentIndexInEditorPopup =
                                    EditorGUILayout.Popup(_skinLabelName, slot.CurrentIndexInEditorPopup,
                                        _stringOptions);

                                EditorGUILayout.PropertyField(slotInListProperty);
                                EditorGUILayout.PropertyField(keyInListProperty);

                                if (slot.CurrentIndexInEditorPopup >= 0)
                                {
                                    slot.CustomPath =
                                        _visualInfoPreset
                                            .Variants[
                                                slot.CurrentIndexInEditorPopup > -1
                                                    ? slot.CurrentIndexInEditorPopup
                                                    : 0]
                                            .Path;
                                }
                            }
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.BeginVertical();
                            {
                                GUI.backgroundColor = Color.red;
                                if (GUILayout.Button(_deleteBtnName, GUILayout.Width(20f), GUILayout.Height(20f)))
                                {
                                    _target.Slots.RemoveAt(index: i);
                                }

                                GUI.backgroundColor = Color.white;
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();

                if (GUILayout.Button(_addNewSlotLabelName))
                {
                    _target.Slots.Add(new SlotSkinData());
                }
            }

            _expandedEditor = EditorGUILayout.Foldout(_expandedEditor, "Editor");
            if (_expandedEditor)
            {
                _matcher = EditorGUILayout.TextField("Match (RegExp)", _matcher ?? "");
                _editorCurrentIndexInEditorPopup =
                    EditorGUILayout.Popup(_skinLabelName, _editorCurrentIndexInEditorPopup,
                        _stringOptions);
                if (GUILayout.Button("Apply"))
                {
                    var reg = new Regex(_matcher);
                    foreach (var slot in _target.Slots)
                    {
                        if (slot != null && slot.Slot != null)
                        {
                            if (reg.IsMatch(slot.Slot))
                            {
                                slot.CurrentIndexInEditorPopup = _editorCurrentIndexInEditorPopup;
                                isChanged = true;
                                if (slot.CurrentIndexInEditorPopup >= 0)
                                {
                                    slot.CustomPath =
                                        _visualInfoPreset
                                            .Variants[
                                                slot.CurrentIndexInEditorPopup > -1
                                                    ? slot.CurrentIndexInEditorPopup
                                                    : 0]
                                            .Path;
                                }
                            }
                        }
                    }
                }
            }

            if (GUI.changed || isChanged)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_target);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawEditor();
        }
    }
}