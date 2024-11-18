using System;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace StateMachine
{
/// <summary>
/// Service to track loading and unloading of content. 
/// At the start of each use, need to set <b>_loadingsCount</b>, it allows to add other loadings during current one 
/// </summary>
public class LoadingProgressService : IServiceLoadingProgress
{
#region Fields

    List<float> _unloadingProgresses = new();
    List<SceneLoadProgress> _loadingProgresses = new();
    static readonly TimeSpan _delayTimeSpan = TimeSpan.FromSeconds(.25f);

    public int LoadProgressCount { get; set; }
    public bool UnloadsAreFinished { get; private set; }
    public event Action<float> OnUpdateProgress;
    public event Action<string> OnUpdateLoadingPrompt;

    float _unloadProgress;

#endregion

#region Public methods

    public LoadingProgressService() => ResetProgress();

    public void ResetProgress()
    {
        LoadProgressCount = 0;
        _loadingProgresses.Clear();
        _unloadingProgresses.Clear();
        UnloadsAreFinished = true;

        OnUpdateProgress?.Invoke(0);
        OnUpdateLoadingPrompt?.Invoke(String.Empty);
    }
    
    public void RegisterLoadingProgress(SceneLoadProgress sceneLoadProgress) => _loadingProgresses.Add(sceneLoadProgress);
    
    public async UniTaskVoid RegisterUnloadingTasks(params UniTask[] unloadingTasks)
    {
        for (int i = 0; i < unloadingTasks.Length; i++)
            _unloadingProgresses.Add(0);

        UnloadsAreFinished = false;
        while (!UnloadsAreFinished) 
            await TrackingUnloadings(unloadingTasks);
    }

    public void UpdateProgress()
    {
        _unloadProgress = _unloadingProgresses.Count() != 0 ? _unloadingProgresses.Average(progress => progress) : 1;
        var loadProgress = LoadProgressCount != 0 ? _loadingProgresses.Sum(progress => progress.GetAvgProgress()) / LoadProgressCount : 0;
        OnUpdateProgress?.Invoke((loadProgress + _unloadProgress) / 2);
    }
    
    public void UpdateLoadingPrompt(string progressPrompt) => OnUpdateLoadingPrompt?.Invoke(progressPrompt);

#endregion

#region Private methods

    /// <summary>
    /// Track UnloadingTasks and update progress each "_delayTimeSpan" to avoid too frequent update
    /// </summary>
    async UniTask TrackingUnloadings(UniTask[] unloadingTasks)
    {
        for (int i = 0; i < unloadingTasks.Length; i++)
            _unloadingProgresses[i] = unloadingTasks[i].GetAwaiter().IsCompleted ? 1 : 0;

        UpdateProgress();
        UnloadsAreFinished = Mathf.Approximately(_unloadProgress, 1);
        
        await UniTask.Delay(_delayTimeSpan, ignoreTimeScale: true);
    }

#endregion
}
}