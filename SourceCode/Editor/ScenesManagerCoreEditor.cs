using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace IRK.Unity.ScenesManager
{
    [CustomEditor(typeof(ScenesManagerCore))]
    public class ScenesManagerCoreEditor : Editor
    {
        private ScenesManagerCore _core;
        
        private SerializedProperty _pauseInLoading;
        private SerializedProperty _durationAll, _durationValue;
        private SerializedProperty _sleepAll, _sleepValue;

        private SerializedProperty _loadingMode;
        private SerializedProperty _loadingScene;
        private SerializedProperty _objLoading;

        private bool _transitionsShow = false;
        private bool _loadingSettingsShow = true;
        private string _nameLoadingScene;
        private DC _dc;

        //item1 : Scene name 'from'
        //item2 : Scene name 'to'
        private List<System.Tuple<string, string>> _namesTransitionScenes;

        private void OnEnable()
        {
            _dc = new DC();
            _dc.Add("BT_Edit", "Edit","Open the Graph Editor Window");
            _dc.Add("scenes", "Scenes");
            _dc.Add("transitions", "Transitions");
            _dc.Add("TE_durationAll", "DurationEffectsAll","If enabled,the duration value of all transition effects follows the 'ScenesManagerCore.durationTransitionEffect'");
            _dc.Add("TE_sleepAll", "SleepEffectsAll", "If enabled,the sleep value of all transition effects follows the 'ScenesManagerCore.sleepTransitionEffect'");
            _dc.Add("pause", "PauseInLoading","If enabled,the time scale will be zero when the 'ScenesManagerCore' starts loading and will eventually return to its previous value.");
            _dc.Add("loading", "LoadingScreen");
            _dc.Add("loadingMode", "LoadingMode");
            _dc.Add("loadingScene", "LoadingScene");
            _dc.Add("loadingObject", "LoadingGameObject");

            //Get serializedProperties of ScenesManagerCore
            _pauseInLoading = serializedObject.FindProperty("_pauseInLoading");
            _durationAll = serializedObject.FindProperty("_durationTEAll");
            _durationValue = serializedObject.FindProperty("_durationTE");
            _sleepAll = serializedObject.FindProperty("_sleepTEAll");
            _sleepValue = serializedObject.FindProperty("_sleepTE");
            _loadingMode = serializedObject.FindProperty("_loadingMode");
            _loadingScene = serializedObject.FindProperty("_loadingScene");
            _objLoading = serializedObject.FindProperty("_objLoading");

            _core = (ScenesManagerCore)target;
            _nameLoadingScene = BuildIndexToNameScene(_loadingScene.intValue);

            InitializeNamesTransitionScenes();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(_pauseInLoading, _dc["pause"]);

            EditorGUILayout.Space();

            ChangeAllEffectProperty(_durationAll, _durationValue, _dc["TE_durationAll"], "duration");
            ChangeAllEffectProperty(_sleepAll, _sleepValue, _dc["TE_sleepAll"], "sleep");

            EditorGUILayout.Space();

            //LoadingSettings
            _loadingSettingsShow = EditorGUILayout.BeginFoldoutHeaderGroup(_loadingSettingsShow, _dc["loading"]);
            if(_loadingSettingsShow)
            {
                //OnGUILoadingSettings();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            //Button 'Edit'
            if (GUILayout.Button(_dc["BT_Edit"]))
            {
                ScenesManagerWindow.ShowWindow().core = _core;
            }

            EditorGUILayout.Space();

            OnGUIInfo();
        }

        private void ChangeAllEffectProperty(SerializedProperty propertyToggle,SerializedProperty propertyValue,GUIContent content,string name)
        {
            //Begin
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();

            //Toggle property
            EditorGUILayout.PropertyField(propertyToggle, content);

            EditorGUILayout.Space();

            //Value property
            if (propertyToggle.boolValue)
            {
                EditorGUILayout.PropertyField(propertyValue, new GUIContent(""));
            }

            EditorGUILayout.Space();

            //End
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                switch (name)
                {
                    case "duration":
                        _core.controller.SetDurationAllTransitionEffects(propertyValue.floatValue);
                        break;

                    case "sleep":
                        _core.controller.SetSleepAllTransitionEffects(propertyValue.floatValue);
                        break;
                }
            }
        }

        private void OnGUIInfo()
        {
            if (_core.controller == null)
                return;

            //Transitions
            _transitionsShow = EditorGUILayout.BeginFoldoutHeaderGroup(_transitionsShow, _dc["transitions"]);
            if (_transitionsShow)
            {
                GUILayout.Label("[SceneFrom(Index)[Effect] => SceneTo(Index)[Effect]]");
                GUILayout.Label("CountTransitions:" + _core.controller.transitions.Count);

                EditorGUILayout.Space();
                for(int i = 0; i < _core.controller.transitions.Count; i++)
                {
                    var nameTrScenes = _namesTransitionScenes[i];
                    var tr = _core.controller.transitions[i];

                    GUILayout.Label(string.Format("{0} [{1}] => {2} [{3}]",
                        nameTrScenes.Item1,
                        tr.effectUnload.GetType().Name, 
                        nameTrScenes.Item2,
                        tr.effectLoad.GetType().Name));
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void OnGUILoadingSettings()
        {
            EditorGUILayout.PropertyField(_loadingMode, _dc["loadingMode"]);

            switch (_core.loadingMode)
            {
                case ScenesManagerLoadingMode.None:
                    break;

                case ScenesManagerLoadingMode.Scene:
                    {
                        EditorGUILayout.BeginHorizontal();

                        //Index loadingScene
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(_loadingScene, _dc["loadingScene"]);
                        if (EditorGUI.EndChangeCheck())
                        {
                            _nameLoadingScene = BuildIndexToNameScene(_loadingScene.intValue);
                        }

                        EditorGUILayout.Space();

                        //Name loadingScene
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.TextField("", _nameLoadingScene, GUILayout.MinWidth(100.0f));
                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.EndHorizontal();
                        break;
                    }

                case ScenesManagerLoadingMode.GameObject:
                    EditorGUILayout.PropertyField(_objLoading, _dc["loadingObject"]);
                    break;
            }
        }

        private string BuildIndexToNameScene(int index)
        {
            string result = SMEditorUtility.GetNameSceneBuild(index, false);

            return string.IsNullOrEmpty(result) ? "---" : result;
        }

        private void InitializeNamesTransitionScenes()
        {
            _namesTransitionScenes = new List<System.Tuple<string, string>>();

            for (int i = 0; i < _core.controller.transitions.Count;i++)
            {
                string nameSceneFrom = SMEditorUtility.GetNameSceneBuild(_core.controller.transitions[i].fromSceneIndex);
                string nameSceneTo = SMEditorUtility.GetNameSceneBuild(_core.controller.transitions[i].toSceneIndex);
                _namesTransitionScenes.Add(new System.Tuple<string, string>(nameSceneFrom, nameSceneTo));
            }
        }
    }
}