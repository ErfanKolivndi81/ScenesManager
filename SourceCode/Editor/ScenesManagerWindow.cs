using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

namespace IRK.Unity.ScenesManager
{
    public class ScenesManagerWindow : EditorWindow
    {
        public static ScenesManagerWindow ShowWindow()
        {
            if (EditorBuildSettings.scenes.Length == 0)
            {
                Debug.LogError("[ScenesManager]The list of scenes in build is empty.File > BuildSettings > ScenesInBuild");
                
                return null;
            }

            ScenesManagerWindow win = GetWindow<ScenesManagerWindow>();
            win.Show();

            return win;
        }

        public static ScenesManagerCore SMCore { get; private set; }
        public static List<Assembly> assembliesWithTransitionEffect { get; private set; }
        public static List<string> nameTransitionEffects { get; private set; }

        private ScenesManagerGraphView _graphView;
        private ScenesManagerCore _core;
        private DC _dc;
        
        public ScenesManagerCore core
        {
            get { return _core; }
            set
            {
                _core = value;
				SMCore = _core;
				
                _graphView = SaveLoadController.ControllerToGraph(_core.controller, this);

                rootVisualElement.Clear();

                rootVisualElement.Add(_graphView);
                _graphView.StretchToParentSize();

                InitToolbar();
            }
        }

        private void OnEnable()
        {
            Refresh();

            Texture iconTitle = AssetDatabase.LoadAssetAtPath<Texture>(SMEditorUtility.pathResourceFolder + "icon_main.png");

            _dc = new DC();
            _dc.Add("W_Title", "SMGraph",iconTitle);
            _dc.Add("W_Save", "Save");
            _dc.Add("W_Core", "SceneManagerCore");
            _dc.Add("W_Refresh", "Refresh");
            _dc.Add("W_Grid", "Grid");

            titleContent = _dc["W_Title"];

            if (_core != null)
                core = _core;
            else
                InitToolbar();
        }

        private void InitToolbar()
        {
            Toolbar toolbar = new Toolbar();

            #region Button Save
            ToolbarButton buttonSave = new ToolbarButton()
            {
                text = _dc["W_Save"].text,
                tooltip = _dc["W_Save"].tooltip,
            };
            
            buttonSave.clicked += () =>
            {
                if (_core)
                {
                    _core.controller = SaveLoadController.GraphToController(_graphView);
                }
            };
            #endregion

            #region Button Refresh
            ToolbarButton buttonRefresh = new ToolbarButton()
            {
                text = _dc["W_Refresh"].text,
                tooltip = _dc["W_Refresh"].tooltip,
            };

            buttonRefresh.clicked += () => { Refresh(); };
            #endregion

            #region CoreField
            ObjectField coreField = new ObjectField()
            {
                label = _dc["W_Core"].text,
                tooltip = _dc["W_Core"].tooltip,
                objectType = typeof(ScenesManagerCore),
            };
            
            if (_core)
                coreField.value = _core;

            coreField.RegisterValueChangedCallback((obj) =>
            {
                if (coreField.value)
                {
                    core = (ScenesManagerCore)coreField.value;
                    coreField.value = _core;
                }
            });
            #endregion

            #region Toggle Grid
            ToolbarToggle toggleGrid = new ToolbarToggle()
            {
                text = _dc["W_Grid"].text,
                tooltip = _dc["W_Grid"].tooltip,
                value = _graphView != null && _graphView.visibleGrid,
            };
            toggleGrid.RegisterValueChangedCallback((b) => { _graphView.visibleGrid = toggleGrid.value; });
            #endregion

            toolbar.Add(buttonSave);
            toolbar.Add(buttonRefresh);
            toolbar.Add(toggleGrid);
            toolbar.Add(coreField);

            rootVisualElement.Add(toolbar);
        }

        private void Refresh()
        {
            assembliesWithTransitionEffect = SMEditorUtility.GetAssembliesWithType(typeof(TransitionEffect));
            nameTransitionEffects = SMEditorUtility.GetNameTypesInAssemblies(typeof(TransitionEffect), assembliesWithTransitionEffect.ToArray());
        }
    }
}
