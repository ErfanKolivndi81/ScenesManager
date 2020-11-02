using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Type = System.Type;

namespace IRK.Unity.ScenesManager
{ 
    [AddComponentMenu("ScenesManager/Core")]
    public class ScenesManagerCore : MonoBehaviour
    {
        public const string nameAuto = "AUTO";

        public static ScenesManagerCore core { get; private set; }

        public UnityAction<ScenesManagerActionType> onAction;

        #region SerializeFields
        [SerializeField] private ScenesManagerController _controller;
        [SerializeField] private bool _durationTEAll;
        [SerializeField] private float _durationTE;
        [SerializeField] private bool _sleepTEAll;
        [SerializeField] private float _sleepTE;
        [SerializeField] private bool _pauseInLoading;
        #endregion

        #region Fields
        private List<Object> _destroyObjectsOnSceneLoaded;
        private ScenesManagerState _state;
        private TransitionData _transitionPlaying;
        private AsyncOperation _async;
        private Canvas _canvas;
        private Camera _cameraSceneActive;
        #endregion

        #region Properties
        public ScenesManagerController controller { get { return _controller; } set { _controller = value; } }

        /// <summary>
        /// If enabled,the duration value of all transition effects follows the <see cref="ScenesManagerCore.durationTransitionEffect"/>
        /// </summary>
        public bool durationTransitionEffectsAll { get { return _durationTEAll; } set { _durationTEAll = value; } }

        /// <summary>
        /// The duration of all transition effects follows this value(Except those with attribute <see cref="CustomDurationAttribute"/>
        /// </summary>
        public float durationTransitionEffect { get { return _durationTE; } set { _durationTE = value; } }

        /// <summary>
        /// If enabled,the sleep value of all transition effects follows the <see cref="ScenesManagerCore.sleepTransitionEffect"/>
        /// </summary>
        public bool sleepTransitionEffectsAll { get { return _sleepTEAll; } set { _sleepTEAll = value; } }

        /// <summary>
        /// The sleep of all transition effects follows this value(Except those with attribute <see cref="CustomSleepAttribute"/>
        /// </summary>
        public float sleepTransitionEffect { get { return _sleepTE; } set { _sleepTE = value; } }

        /// <summary>
        /// If enabled,the time scale will be zero when the <see cref="ScenesManagerCore"/> starts loading and will eventually return to its previous value.
        /// </summary>
        public bool pauseInLoading { get { return _pauseInLoading; } set { _pauseInLoading = value; } }
        public Canvas canvas { get { return _canvas; } }
        public ScenesManagerState state { get { return _state; } }
        public AsyncOperation asyncOperation { get { return _async; } }
        public TransitionData transitionPlaying { get { return _transitionPlaying; } }
        #endregion

        #region Messages Unity

        #if UNITY_EDITOR
        private void Reset()
        {
            controller = ScenesManagerController.CreateEmptyController();

            durationTransitionEffectsAll = false;
            durationTransitionEffect = 1.0f;

            sleepTransitionEffectsAll = false;
            sleepTransitionEffect = 0.2f;
        }
        #endif

        private void Awake()
        {
            if (core != null && core != this)
            {
                Destroy(gameObject);
            }
            else if (core == null)
            {
                core = this;
            }

            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            SceneManager.sceneLoaded += LoadedScene;

            LoadedScene(SceneManager.GetActiveScene(), LoadSceneMode.Single);

            SetActiveAllChild(false);
            _destroyObjectsOnSceneLoaded = new List<Object>();
        }
        #endregion

        #region Main
        public void LoadScene(string name)
        {
            LoadScene(ScenesManagerUtility.GetBuildIndexScene(name));
        }

        public void LoadScene(int index)
        {
            //Warning
            if (IsState(ScenesManagerState.LoadingCore))
            {
                Debug.LogWarning("[ScenesManagerCore]ScenesManagerCore has loading\nTransitionActive:" + _transitionPlaying.ToString(false), this);
                return;
            }

            int sceneBefore = SceneManager.GetActiveScene().buildIndex;

            if (sceneBefore == index)
                return;

            System.Func<TransitionData, bool> predicate = tr => tr.fromSceneIndex == sceneBefore && tr.toSceneIndex == index;
            if (controller.transitions.Any(predicate))
                _transitionPlaying = controller.transitions.Where(predicate).First();
            else
            {
                Debug.LogErrorFormat
                    (
                    "[ScenesManagerCore]The transition({0}({1}) => {2}({3})) is'n available.",
                    ScenesManagerUtility.GetSceneName(sceneBefore),
                    ScenesManagerUtility.GetSceneName(index)
                    );
                return;
            }

            OnLoadingCoreStart();
                  
            PlayEffect(TransitionEffectType.Unload);
        }

        private void LoadedScene(Scene scene, LoadSceneMode mode)
        {          
            if(_transitionPlaying != null && !_transitionPlaying.isLoadAsync)
            {
                OnLoadingSceneCompleted();
            }

            _cameraSceneActive = scene.GetRootGameObjects().First(obj => obj.GetComponent<Camera>()).GetComponent<Camera>();

            //========Set the sortingOrder of canvas ScenesManagerCore==========
            if (_canvas == null)
                InitializeCanvas();

            var canvasActiveScene = scene.GetRootGameObjects().Where(obj => obj.GetComponent<Canvas>());

            if (canvasActiveScene.Count() == 0)
                _canvas.sortingOrder = 0;
            else
            {
                _canvas.sortingOrder = canvasActiveScene
                    .Select(obj => obj.GetComponent<Canvas>())
                    .Max(cnv => cnv.sortingOrder) + 1;
            }
        }

        internal IEnumerator LoadScene()
        {
            onAction?.Invoke(ScenesManagerActionType.OnLoadingSceneStart);

            //Update the state
            _state &= ~ScenesManagerState.PlayingUnloadTE;
            _state |= ScenesManagerState.LoadingScene;

            //Load scene
            if (_transitionPlaying.isLoadAsync)
            {
                _async = SceneManager.LoadSceneAsync(_transitionPlaying.toSceneIndex, _transitionPlaying.loadSceneParameters);

                while (!_async.isDone)
                {
 
                    yield return null;
                }

                _async.completed += (a) =>
                {
                    OnLoadingSceneCompleted();
                };
            }
            else
            {
                SceneManager.LoadScene(_transitionPlaying.toSceneIndex, _transitionPlaying.loadSceneParameters);
            }
        }

        private void OnLoadingCoreStart()
        {
            onAction?.Invoke(ScenesManagerActionType.OnLoadingCoreStart);

            SetActiveAllChild(true);

            if (_pauseInLoading)
            {
                PlayerPrefs.SetFloat("SM_defualtTimeScale", Time.timeScale);
                Time.timeScale = 0.0f;
            }
            _state = ScenesManagerState.LoadingCore | ScenesManagerState.PlayingUnloadTE;
        }

        private void OnLoadingSceneCompleted()
        {
            onAction?.Invoke(ScenesManagerActionType.OnLoadingSceneComplete);

            _transitionPlaying.effectUnload.OnSceneLoaded();
            DestroyAndClearGameObjectsCore(_transitionPlaying.effectLoad.sleep);
            PlayEffect(TransitionEffectType.Load);

            //Set 'state'
            _state &= ~ScenesManagerState.LoadingScene;
            _state |= ScenesManagerState.PlayingLoadTE;

            _async = null;
        }

        private void OnLoadingCoreCompleted()
        {
            _transitionPlaying = null;
            SetActiveAllChild(false);

            DestroyAndClearGameObjectsCore();

            //Set 'state'
            _state &= ~ScenesManagerState.LoadingCore;
            _state &= ~ScenesManagerState.PlayingLoadTE;

            if (_pauseInLoading)
            {
                Time.timeScale = PlayerPrefs.GetFloat("SM_defaultTimeScale",1.0f);
                PlayerPrefs.DeleteKey("SM_defaultTimeScale");
            }

            onAction?.Invoke(ScenesManagerActionType.OnLoadingCoreComplete);
        }

        private void OnTransitionEffectComplete(TransitionEffectType type)
        {
            switch (type)
            {
                case TransitionEffectType.Load: OnLoadingCoreCompleted(); break;

                case TransitionEffectType.Unload:
                        StartCoroutine(LoadScene());
                    break;
            }
        }

        private void PlayEffect(TransitionEffectType type)
        {
            TransitionEffect effect = _transitionPlaying[type];

            int numberOnAction = (((int)type) + 3) * 10;
            onAction?.Invoke((ScenesManagerActionType)numberOnAction + 1);

            effect.CompleteEffect = () =>
            {
                onAction?.Invoke((ScenesManagerActionType)numberOnAction);
                OnTransitionEffectComplete(type);
            };

            StartCoroutine(effect?.PlayEffect(type));
        }
        #endregion

        #region Utility
        private void DestroyAndClearGameObjectsCore(float t = 0.0f)
        {
            foreach (Object obj in _destroyObjectsOnSceneLoaded)
            {
                if (obj != null)
                {
                    Destroy(obj, t);
                }
            }

            _destroyObjectsOnSceneLoaded.Clear();
        }

        private void InitializeCanvas()
        {
            var coreChilds = transform.Cast<Transform>();
            var canvasFinded = coreChilds.Where(obj => obj.GetComponent<Canvas>());

            if (coreChilds.Count() == 0 || canvasFinded.Count() == 0)
            {
                _canvas = CreateGameObject("SM_Canvas", false, typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler)).GetComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            else
            {
                _canvas = canvasFinded.Select(obj => obj.GetComponent<Canvas>()).First();
            }
        }

        public void SetActiveAllChild(bool active)
        {
            foreach (Transform transform in transform)
            {
                transform.gameObject.SetActive(active);
            }
        }

        public GameObject CreateGameObjectUI(string name, params Type[] components)
        {
            return CreateGameObjectUI(name, true, components);
        }

        public GameObject CreateGameObjectUI(string name, bool destroyOnEndEffect, params Type[] components)
        {
            return CreateGameObject(name, _canvas.transform, destroyOnEndEffect, components);
        }

        public GameObject CreateGameObject(string name,params Type[] components)
        {
            return CreateGameObject(name, true, components);
        }

        public GameObject CreateGameObject(string name,Transform parent,params Type[] components)
        {
            return CreateGameObject(name, parent, true, components);
        }

        public GameObject CreateGameObject(string name,bool destoryOnEndEffect,params Type[] components)
        {
            return CreateGameObject(name, transform, destoryOnEndEffect, components);
        }

        public GameObject CreateGameObject(string name, Transform parent, bool destroyOnEndEffect, params Type[] components)
        {
            //Create
            GameObject result = new GameObject(name, components);
            result.transform.SetParent(parent);

            //NameAuto
            if (name == nameAuto || string.IsNullOrEmpty(name))
            {
                TransitionEffectType TE_type = IsState(ScenesManagerState.PlayingLoadTE) ? 
                    TransitionEffectType.Load : TransitionEffectType.Unload;

                //appanedName : _Load/Unload_Index
                string appendName = "_" + TE_type.ToString() + "_" + (parent.childCount - 1).ToString();
                if (components.Length == 1)
                {
                    //Example : Image_Load_0
                    result.name = components[0].Name + appendName;
                }
                else
                {
                    //Example : smCore_Load_0
                    result.name = "smCore" + appendName;
                }
            }

            //Add to destroy
            if (destroyOnEndEffect)
                _destroyObjectsOnSceneLoaded.Add(result);

            return result;
        }

        public Sprite ScreenshotToSprite()
        {
            Texture2D tex_screenshot = ScreenCapture.CaptureScreenshotAsTexture();

            Rect rect = new Rect()
            {
                x = 0.0f,
                y = 0.0f,
                width = tex_screenshot.width,
                height = tex_screenshot.height
            };
            _canvas.enabled = true;

            return Sprite.Create(tex_screenshot, rect, Vector2.one / 2.0f, 100.0f);
        }

        public bool IsState(ScenesManagerState smState)
        {
            return (this._state & smState) != 0;
        }
        #endregion
    }
}