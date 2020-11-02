namespace IRK.Unity.ScenesManager
{

    /// <summary>
    /// Shows the status of <see cref="ScenesManagerCore"/> at load time.(Flags)
    /// </summary>
    [System.Flags]
    public enum ScenesManagerState
    {
        /*
        * TE : Transition Effect
        *|----------------------------------LoadingCore---------------------------------|
       * | PlayingUnloadTE |      |--------LoadingScene--------|      | PlayingLoadTE |  
       * |PlayEffectUnload| ==> |UnloadScene ==> LoadScene| ==>|PlayEffectLoad|
       */

        /// <summary>
        /// When <see cref="ScenesManagerCore"/> is loading
        /// </summary>
        LoadingCore = 1 << 0,

        /// <summary>
        /// When the load transition effect is running
        /// </summary>
        PlayingLoadTE = 1 << 1,

        /// <summary>
        /// When the unload transition effect is running
        /// </summary>
        PlayingUnloadTE = 1 << 2,

        /// <summary>
        /// When the scene is loading
        /// </summary>
        LoadingScene = 1 << 3,
    }
}