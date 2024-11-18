using System;
using Cysharp.Threading.Tasks;

namespace StateMachine
{
public interface IServiceLoadingProgress
{
    public int LoadProgressCount { get; set; }
    public bool UnloadsAreFinished { get; }
    event Action<float> OnUpdateProgress;
    event Action<string> OnUpdateLoadingPrompt;

    void ResetProgress();
    
    /// <summary>
    /// Store <b>loadings</b> at runtime to have ref to original value during whole loading
    /// Store LoadingProgress to update loading bar. Should be called in <b>StateEnter</b> of each scene
    /// </summary>
    void RegisterLoadingProgress(SceneLoadProgress sceneLoadProgress);

    /// <summary>
    /// Store <b>UnloadingTasks</b> and await till all are finished. Used on StateExit
    /// </summary>
    UniTaskVoid RegisterUnloadingTasks(params UniTask[] unloadingTasks);
    
    /// <summary>
    /// Call when need to calculate the progress <b>Avg(Load+Unload)</b> and updated loading bar
    /// </summary>
    void UpdateProgress();
    
    /// <summary>
    /// Update the hint (prompt) on loading screen
    /// </summary>
    void UpdateLoadingPrompt(string progressPrompt);
}
}