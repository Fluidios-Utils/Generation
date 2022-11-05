using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FluidiousUtils.Generation.NoiseMath
{
    [CustomEditor(typeof(NoiseVisualizer))]
    public class NoiseVisualizerEditor : Editor
    {
        SerializedProperty _mapScale;
        SerializedProperty _noiseScale;
        SerializedProperty _autoUpdate;
        SerializedProperty _settings;
        private List<bool> _settingsElementFoldoutMarkers = new List<bool>();
        private Editor _layerEditor;

        private void OnEnable()
        {
            _mapScale = serializedObject.FindProperty("_mapScale");
            _noiseScale = serializedObject.FindProperty("_noiseScale");
            _autoUpdate = serializedObject.FindProperty("_autoUpdate");
            _settings = serializedObject.FindProperty("_settings");
            for (var i = 0; i < _settings.arraySize; i++)
            {
                _settingsElementFoldoutMarkers.Add(false);
            }
        }

        public override void OnInspectorGUI()
        {
            NoiseVisualizer noiseVis = (NoiseVisualizer)target;
            serializedObject.Update();

            EditorGUILayout.PropertyField(_mapScale);
            EditorGUILayout.PropertyField(_noiseScale);
            EditorGUILayout.PropertyField(_autoUpdate);

            EditorGUI.BeginChangeCheck();
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            _settings.arraySize = EditorGUILayout.IntField("Noise layers", Mathf.Clamp(_settings.arraySize, 0, int.MaxValue));
            if (EditorGUI.EndChangeCheck())
            {
                _settingsElementFoldoutMarkers.Clear();

                for (var i = 0; i < _settings.arraySize; i++)
                {
                    _settingsElementFoldoutMarkers.Add(false);
                }

                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUI.indentLevel++;
            for (var i = 0; i < _settings.arraySize; i++)
            {
                var layer = _settings.GetArrayElementAtIndex(i);
                _settingsElementFoldoutMarkers[i] = EditorGUILayout.Foldout(_settingsElementFoldoutMarkers[i], ("Noise " + (i + 1)));

                EditorGUI.indentLevel++;

                if (_settingsElementFoldoutMarkers[i])
                {
                    EditorGUILayout.PropertyField(layer.FindPropertyRelative("_enabled"));
                    if (i > 0)
                    {
                        EditorGUILayout.PropertyField(layer.FindPropertyRelative("_usePreviousAsMask"));
                    }
                    else
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.PropertyField(layer.FindPropertyRelative("_usePreviousAsMask"));
                        EditorGUI.EndDisabledGroup();
                    }
                    var so = layer.FindPropertyRelative("_noiseSettings");

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(so);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties(); // поскольку изменили значение PropertyField (scriptableObjectData) 
                                                                    // нужно применить изменения и только после этого
                                                                    // перезагрузить Editor scriptableObjectData
                    }
                    _layerEditor = Editor.CreateEditor(so.objectReferenceValue);
                    if (so.objectReferenceValue != null)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUI.BeginDisabledGroup(true);
                        _layerEditor.OnInspectorGUI();
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndVertical();
                        DestroyImmediate(_layerEditor);
                    }
                    else
                    {
                        EditorGUILayout.Separator();
                        EditorGUILayout.HelpBox("NoiseSettings не должен быть пустым!", MessageType.Error);
                    }
                }

                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;

            if (noiseVis.AutoUpdate)
                noiseVis.Regenerate();
            EditorGUILayout.Separator();
            if (GUILayout.Button("Regenerate"))
            {
                noiseVis.Regenerate();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
