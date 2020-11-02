using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IRK.Unity.ScenesManager
{
    [AddComponentMenu("ScenesManager/Loading")]
    public class ScenesManagerLoading : MonoBehaviour
    {
        public Slider slider;
        public Text text;

        private ScenesManagerCore _core;

        private void Awake()
        {
            _core = ScenesManagerCore.core;

            _core.onAction = OnActionSMCore;
        }

        private void Update()
        {
            if (_core == null)
                return;

            if(_core.IsState(ScenesManagerState.LoadingScene))
            {
                float progress = Mathf.Clamp01(_core.asyncOperation.progress / 0.9f);
                slider.value = progress;
                text.text = (progress * 100.0f).ToString() + "%";
            }
        }

        private void OnActionSMCore(ScenesManagerActionType type)
        {
            if(type == ScenesManagerActionType.OnLoadingCoreComplete)
                _core.LoadScene(2);
        }
    }
}