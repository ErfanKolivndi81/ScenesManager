namespace IRK.Unity.ScenesManager
{
    /// <summary>
    /// An helper enum for <see cref="ScenesManagerCore.onAction"/> action
    /// </summary>
	public enum ScenesManagerActionType
	{
        OnLoadingCoreStart = 11,
        OnLoadingCoreComplete = 10,

        OnLoadingSceneStart = 21,
        OnLoadingSceneComplete = 20,

        OnPlayLoadTE = 41,
        OnCompleteLoadTE = 40,

        OnPlayUnloadTE = 31,
        OnCompleteUnloadTE = 30,
    }
}